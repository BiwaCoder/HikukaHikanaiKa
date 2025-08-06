using UnityEngine;
using UnityEngine.UI;
using HikukaHikanaika.Models;
using HikukaHikanaika.Views;
using HikukaHikanaika.Utils;
using System.Collections.Generic;
using System.Linq;

namespace HikukaHikanaika.Controllers
{
    public class ImprovedRealityShowController : MonoBehaviour
    {
        [Header("MVC Components")]
        public RealityShowView realityShowView;
        public GachaController gachaController;
        
        [Header("API Settings")]
        [SerializeField] private string apiUrl = "http://localhost:5001";
        [SerializeField] private bool useAIMode = true;
        
        private RealityShowModel realityShowModel;
        
        void Start()
        {
            InitializeComponents();
            SetupEventListeners();
            UpdateUI();
            
            // APIクライアントのベースURLを設定
            APIClient.Instance.SetBaseUrl(apiUrl);
            
            // APIサーバーの接続確認
            TestAPIConnection();
        }
        
        private void InitializeComponents()
        {
            realityShowModel = new RealityShowModel();
        }
        
        private void SetupEventListeners()
        {
            // ローカル判定ボタン
            if (realityShowView.judgeButton != null)
                realityShowView.judgeButton.onClick.AddListener(OnLocalJudgeButtonClicked);
                
            // AI判定ボタン
            if (realityShowView.aiJudgeButton != null)
                realityShowView.aiJudgeButton.onClick.AddListener(OnAIJudgeButtonClicked);
        }
        
        private void UpdateUI()
        {
            realityShowView.UpdateRivalDisplay(realityShowModel.Rivals);
        }
        
        private void OnLocalJudgeButtonClicked()
        {
            if (gachaController == null)
            {
                Debug.LogError("GachaController reference is missing!");
                realityShowView.ShowError("GachaControllerが見つかりません");
                return;
            }
            
            PlayerData player = gachaController.PlayerData;
            string result = GenerateLocalJudgeResult(player);
            realityShowView.ShowJudgeResult(result);
        }
        
        private void OnAIJudgeButtonClicked()
        {
            if (gachaController?.PlayerData == null)
            {
                realityShowView.ShowError("プレイヤーデータが見つかりません");
                return;
            }
            
            realityShowView.SetLoading(true);
            GenerateAIRealityShow();
        }
        
        private void GenerateAIRealityShow()
        {
            // プレイヤーとライバルのキャラクター名リスト
            List<string> characterNames = new List<string> { "Player" };
            characterNames.AddRange(realityShowModel.Rivals.Select(r => r.Name));
            
            // 美容ステータスリスト
            List<Dictionary<string, string>> beautyStatsList = new List<Dictionary<string, string>>();
            
            // プレイヤーのステータス
            var playerData = gachaController.PlayerData;
            var playerStats = new Dictionary<string, string>
            {
                {"fashion", playerData.CurrentOutfit?.name ?? "なし"},
                {"family_wealth", playerData.CurrentFamilyWealth?.name ?? "なし"},
                {"personality", playerData.CurrentPersonality?.name ?? "なし"},
                {"total_points", playerData.GetTotalPoints().ToString()}
            };
            beautyStatsList.Add(playerStats);
            
            // ライバルのステータス
            foreach (var rival in realityShowModel.Rivals)
            {
                var rivalStats = new Dictionary<string, string>
                {
                    {"fashion", rival.Outfit?.name ?? "なし"},
                    {"family_wealth", rival.FamilyWealth?.name ?? "なし"},
                    {"personality", rival.Personality?.name ?? "なし"},
                    {"total_points", rival.GetTotalPoints().ToString()}
                };
                beautyStatsList.Add(rivalStats);
            }
            
            // APIクライアントを使って全体シミュレーションを実行
            APIClient.Instance.SimulateRealityShow(characterNames, beautyStatsList, OnRealityShowSimulationComplete);
        }
        
        private void OnRealityShowSimulationComplete(bool success, RealityShowSimulationResponse response, string errorMessage)
        {
            realityShowView.SetLoading(false);
            
            if (success && response != null)
            {
                DisplayAIRealityShowResult(response);
            }
            else
            {
                string error = errorMessage ?? "不明なエラーが発生しました";
                realityShowView.ShowError($"AI判定エラー: {error}");
                Debug.LogError($"Reality show simulation failed: {error}");
            }
        }
        
        private void DisplayAIRealityShowResult(RealityShowSimulationResponse response)
        {
            string fullResult = "🎬 AIリアリティーショー『HikukaHikanaika』\n";
            fullResult += "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n";
            
            // 参加者自己紹介
            fullResult += "👥 参加者自己紹介タイム\n\n";
            foreach (var intro in response.contestant_introductions)
            {
                fullResult += $"【{intro.character_name}】\n";
                fullResult += $"{intro.introduction}\n\n";
                
                // 美容ステータス表示
                fullResult += "💅 ステータス:\n";
                foreach (var stat in intro.beauty_stats)
                {
                    if (stat.Key != "total_points")
                    {
                        fullResult += $"  • {GetStatDisplayName(stat.Key)}: {stat.Value}\n";
                    }
                }
                fullResult += $"  🎆 合計ポイント: {intro.beauty_stats.GetValueOrDefault("total_points", "0")}\n\n";
            }
            
            fullResult += "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n";
            fullResult += "📺 司会者による総合解説\n\n";
            fullResult += response.show_explanation.explanation;
            
            // ローカル判定結果も追加
            fullResult += "\n\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n";
            fullResult += "⚖️ 数値による勝敗判定\n\n";
            fullResult += GenerateLocalJudgeResult(gachaController.PlayerData);
            
            realityShowView.ShowJudgeResult(fullResult);
        }
        
        private string GetStatDisplayName(string statKey)
        {
            switch (statKey)
            {
                case "fashion": return "👗 ファッション";
                case "family_wealth": return "🏠 家柄";
                case "personality": return "😊 性格";
                default: return statKey;
            }
        }
        
        private string GenerateLocalJudgeResult(PlayerData player)
        {
            string result = "🎬 数値判定結果\n\n";
            result += GetPlayerStatusText(player) + "\n\n";
            result += "━━━━━━━━━━━━━━━━\n\n";
            
            int wins = 0;
            int totalRivals = realityShowModel.Rivals.Count;
            
            foreach (var rival in realityShowModel.Rivals)
            {
                JudgeResult battleResult = realityShowModel.JudgeAgainst(player, rival);
                result += GetBattleResultText(player, rival, battleResult) + "\n\n";
                
                if (battleResult == JudgeResult.Win)
                    wins++;
            }
            
            result += "━━━━━━━━━━━━━━━━\n";
            result += GetFinalRanking(wins, totalRivals);
            
            return result;
        }
        
        private string GetPlayerStatusText(PlayerData player)
        {
            string outfitInfo = player.CurrentOutfit != null 
                ? $"👗 {player.CurrentOutfit.name} ({player.CurrentOutfit.points})" 
                : "👗 何も着ていない (0)";
                
            string familyInfo = player.CurrentFamilyWealth != null 
                ? $"🏠 {player.CurrentFamilyWealth.name} ({player.CurrentFamilyWealth.points})" 
                : "🏠 特になし (0)";
                
            string personalityInfo = player.CurrentPersonality != null 
                ? $"😊 {player.CurrentPersonality.name} ({player.CurrentPersonality.points})" 
                : "😊 特になし (0)";
            
            return $"【{player.PlayerName}】\n{outfitInfo}\n{familyInfo}\n{personalityInfo}\n🎆 合計: {player.GetTotalPoints()}";
        }
        
        private string GetBattleResultText(PlayerData player, RivalData rival, JudgeResult result)
        {
            string resultIcon = "";
            string comment = "";
            
            switch (result)
            {
                case JudgeResult.Win:
                    resultIcon = "🏆 勝利";
                    comment = GetWinComment(player, rival);
                    break;
                case JudgeResult.Lose:
                    resultIcon = "😭 敗北";
                    comment = GetLoseComment(player, rival);
                    break;
                case JudgeResult.Draw:
                    resultIcon = "🤝 引き分け";
                    comment = "互角の戦いでした！";
                    break;
            }
            
            return $"VS {rival.Name} ({rival.GetTotalPoints()}pt) → {resultIcon}\n{comment}";
        }
        
        private string GetWinComment(PlayerData player, RivalData rival)
        {
            int scoreDiff = player.GetTotalPoints() - rival.GetTotalPoints();
            if (scoreDiff >= 100)
                return "圧倒的な差で勝利！観客も大興奮です！";
            else if (player.CurrentOutfit?.name.Contains("オートクチュール") ?? false)
                return "やはりオートクチュールの威力は絶大ですね！";
            else if (scoreDiff > 0)
                return "僅差での勝利！ハラハラドキドキの展開でした！";
            else
                return "特殊な魅力で勝利を掴みました！";
        }
        
        private string GetLoseComment(PlayerData player, RivalData rival)
        {
            int scoreDiff = rival.GetTotalPoints() - player.GetTotalPoints();
            if (scoreDiff >= 100)
                return "実力差は歴然...もっとガチャを回しましょう！";
            else if (player.CurrentOutfit?.name.Contains("サイズ合ってない") ?? false)
                return "服のサイズが合わないのが致命的でした...";
            else
                return "惜しい！もう少しで勝てそうでした！";
        }
        
        private string GetFinalRanking(int wins, int totalRivals)
        {
            if (wins == totalRivals)
                return "🥇 完全優勝！\nあなたが真のリアリティーショーチャンピオンです！";
            else if (wins >= totalRivals * 0.7f)
                return "🥈 上位入賞！\n素晴らしい結果です！次は完全優勝を目指しましょう！";
            else if (wins >= totalRivals * 0.3f)
                return "🥉 中位！\nまずまずの結果です。もう少しガチャを回して強化しましょう！";
            else
                return "😢 下位...\nまだまだ実力不足です。ガチャでステータスを上げて再挑戦！";
        }
        
        private void TestAPIConnection()
        {
            APIClient.Instance.GetHealthCheck((success, message) => {
                if (success)
                {
                    Debug.Log($"API Health Check Success: {message}");
                    realityShowView.ShowMessage("APIサーバー接続成功！");
                }
                else
                {
                    Debug.LogWarning($"API Health Check Failed: {message}");
                    realityShowView.ShowError($"APIサーバー接続確認: {message}");
                }
            });
        }
        
        public void ToggleAIMode()
        {
            useAIMode = !useAIMode;
            Debug.Log($"AI Mode: {(useAIMode ? "Enabled" : "Disabled")}");
        }
    }
}
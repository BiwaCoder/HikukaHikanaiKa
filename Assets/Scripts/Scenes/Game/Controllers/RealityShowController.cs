using UnityEngine;
using HikukaHikanaika.Models;
using HikukaHikanaika.Views;
using System.Collections.Generic;

namespace HikukaHikanaika.Controllers
{
    public class RealityShowController : MonoBehaviour
    {
        [Header("MVC Components")]
        public RealityShowView realityShowView;
        public GachaController gachaController;
        
        private RealityShowModel realityShowModel;
        
        void Start()
        {
            InitializeComponents();
            SetupEventListeners();
            UpdateUI();
        }
        
        private void InitializeComponents()
        {
            realityShowModel = new RealityShowModel();
        }
        
        private void SetupEventListeners()
        {
            if (realityShowView.judgeButton != null)
                realityShowView.judgeButton.onClick.AddListener(OnJudgeButtonClicked);
        }
        
        private void UpdateUI()
        {
            realityShowView.UpdateRivalDisplay(realityShowModel.Rivals);
        }
        
        private void OnJudgeButtonClicked()
        {
            if (gachaController == null)
            {
                Debug.LogError("GachaController reference is missing!");
                return;
            }
            
            PlayerData player = gachaController.PlayerData;
            string result = GenerateJudgeResult(player);
            realityShowView.ShowJudgeResult(result);
        }
        
        private string GenerateJudgeResult(PlayerData player)
        {
            string result = "🎬 リアリティーショー結果発表！\n\n";
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
            
            return $"VS {rival.Name} → {resultIcon}\n{comment}";
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
    }
}
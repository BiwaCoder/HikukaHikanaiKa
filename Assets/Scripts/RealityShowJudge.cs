using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealityShowJudge : MonoBehaviour
{
    [Header("UI参照")]
    public Button judgeButton;
    public Text judgeResultText;
    public Text rivalStatusText;
    
    [Header("プレイヤー参照")]
    public BeautyGachaManager gachaManager;
    
    [Header("ライバル設定")]
    private List<PlayerStatus> rivals = new List<PlayerStatus>();
    private PlayerStatus player = new PlayerStatus();
    
    public enum JudgeResult
    {
        Win,
        Lose,
        Draw
    }
    
    void Start()
    {
        InitializeRivals();
        UpdateRivalDisplay();
        
        if (judgeButton != null)
            judgeButton.onClick.AddListener(StartRealityShowJudge);
    }
    
    void InitializeRivals()
    {
        // ライバル1: 完璧お嬢様キャラ
        var rival1 = new PlayerStatus();
        rival1.playerName = "麗華お嬢様";
        rival1.UpdateStatus("オートクチュールのドレス", 150, "一族で日本史に出てくる", 200, "天性の人たらし", 180);
        rivals.Add(rival1);
        
        // ライバル2: 庶民派努力家キャラ  
        var rival2 = new PlayerStatus();
        rival2.playerName = "田中頑張り子";
        rival2.UpdateStatus("制服しか勝たん", 80, "一般庶民", 10, "ツンデレの黄金比", 100);
        rivals.Add(rival2);
        
        // ライバル3: 中途半端キャラ
        var rival3 = new PlayerStatus();
        rival3.playerName = "佐藤普通美";
        rival3.UpdateStatus("清潔感あるけど量販感", 15, "上流階級", 40, "無難すぎて覚えてもらえない", 25);
        rivals.Add(rival3);
    }
    
    void UpdatePlayerStatus()
    {
        if (gachaManager != null)
        {
            string outfit = gachaManager.CurrentOutfit?.name ?? "何も着ていない";
            int beautyPts = gachaManager.CurrentOutfit?.beautyPoints ?? 0;
            
            string family = gachaManager.CurrentFamilyWealth?.name ?? "特になし";
            int familyPts = gachaManager.CurrentFamilyWealth?.beautyPoints ?? 0;
            
            string personality = gachaManager.CurrentPersonality?.name ?? "特になし";
            int personalityPts = gachaManager.CurrentPersonality?.beautyPoints ?? 0;
            
            player.UpdateStatus(outfit, beautyPts, family, familyPts, personality, personalityPts);
        }
    }
    
    void UpdateRivalDisplay()
    {
        if (rivalStatusText != null)
        {
            string rivalInfo = "🎭 今回のライバル達\n\n";
            foreach (var rival in rivals)
            {
                rivalInfo += $"・{rival.playerName}\n";
            }
            rivalInfo += "\n彼女たちの実力は謎に包まれている...";
            rivalStatusText.text = rivalInfo;
        }
    }
    
    void StartRealityShowJudge()
    {
        UpdatePlayerStatus();
        
        string result = "🎬 リアリティーショー結果発表！\n\n";
        result += player.GetDetailedStatus() + "\n\n";
        result += "━━━━━━━━━━━━━━━━\n\n";
        
        // 各ライバルとの対戦結果
        int wins = 0;
        int totalRivals = rivals.Count;
        
        foreach (var rival in rivals)
        {
            JudgeResult battleResult = JudgeAgainstRival(player, rival);
            result += GetBattleResultText(player, rival, battleResult) + "\n\n";
            
            if (battleResult == JudgeResult.Win)
                wins++;
        }
        
        // 最終順位判定
        result += "━━━━━━━━━━━━━━━━\n";
        result += GetFinalRanking(wins, totalRivals);
        
        if (judgeResultText != null)
            judgeResultText.text = result;
    }
    
    JudgeResult JudgeAgainstRival(PlayerStatus player, PlayerStatus rival)
    {
        // 基本的な合計ポイント比較
        if (player.totalPoints > rival.totalPoints)
            return JudgeResult.Win;
        else if (player.totalPoints < rival.totalPoints)
            return JudgeResult.Lose;
        
        // 同点の場合は特殊判定
        return JudgeSpecialCases(player, rival);
    }
    
    JudgeResult JudgeSpecialCases(PlayerStatus player, PlayerStatus rival)
    {
        // オートクチュールのドレス持ちは有利
        if (player.currentOutfit.Contains("オートクチュール") && !rival.currentOutfit.Contains("オートクチュール"))
            return JudgeResult.Win;
        if (!player.currentOutfit.Contains("オートクチュール") && rival.currentOutfit.Contains("オートクチュール"))
            return JudgeResult.Lose;
            
        // 天性の人たらし持ちは有利
        if (player.currentPersonality.Contains("天性") && !rival.currentPersonality.Contains("天性"))
            return JudgeResult.Win;
        if (!player.currentPersonality.Contains("天性") && rival.currentPersonality.Contains("天性"))
            return JudgeResult.Lose;
            
        // 最低ランクアイテムは不利
        if (player.currentOutfit.Contains("サイズ合ってない") && !rival.currentOutfit.Contains("サイズ合ってない"))
            return JudgeResult.Lose;
        if (!player.currentOutfit.Contains("サイズ合ってない") && rival.currentOutfit.Contains("サイズ合ってない"))
            return JudgeResult.Win;
            
        return JudgeResult.Draw;
    }
    
    string GetBattleResultText(PlayerStatus player, PlayerStatus rival, JudgeResult result)
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
        
        return $"VS {rival.playerName} → {resultIcon}\n{comment}";
    }
    
    string GetWinComment(PlayerStatus player, PlayerStatus rival)
    {
        if (player.totalPoints - rival.totalPoints >= 100)
            return "圧倒的な差で勝利！観客も大興奮です！";
        else if (player.currentOutfit.Contains("オートクチュール"))
            return "やはりオートクチュールの威力は絶大ですね！";
        else if (player.totalPoints > rival.totalPoints)
            return "僅差での勝利！ハラハラドキドキの展開でした！";
        else
            return "特殊な魅力で勝利を掴みました！";
    }
    
    string GetLoseComment(PlayerStatus player, PlayerStatus rival)
    {
        if (rival.totalPoints - player.totalPoints >= 100)
            return "実力差は歴然...もっとガチャを回しましょう！";
        else if (player.currentOutfit.Contains("サイズ合ってない"))
            return "服のサイズが合わないのが致命的でした...";
        else
            return "惜しい！もう少しで勝てそうでした！";
    }
    
    string GetFinalRanking(int wins, int totalRivals)
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
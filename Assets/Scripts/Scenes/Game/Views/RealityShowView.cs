using UnityEngine;
using UnityEngine.UI;
using HikukaHikanaika.Models;
using HikukaHikanaika.Controllers;
using System.Collections.Generic;

namespace HikukaHikanaika.Views
{
    public class RealityShowView : MonoBehaviour
    {
        [Header("UI References")]
        public Button judgeButton;
        public Button aiJudgeButton;
        public Text judgeResultText;
        public Text rivalStatusText;
        public Text loadingText;
        public Text errorText;
        public ScrollRect resultScrollRect;
        
        [Header("Loading Animation")]
        public GameObject loadingIndicator;
        
        public void UpdateRivalDisplay(List<RivalData> rivals)
        {
            if (rivalStatusText != null)
            {
                string rivalInfo = "🎭 今回のライバル達\n\n";
                foreach (var rival in rivals)
                {
                    rivalInfo += $"・{rival.Name}\n";
                }
                rivalInfo += "\n彼女たちの実力は謎に包まれている...";
                rivalStatusText.text = rivalInfo;
            }
        }
        
        public void ShowJudgeResult(string result)
        {
            if (judgeResultText != null)
            {
                judgeResultText.text = result;
            }
        }
        
        public void ShowAIRealityShowResult(List<RealityShowAPIController.CharacterIntroduction> introductions, string explanation)
        {
            if (judgeResultText != null)
            {
                string fullResult = "『運命の館 〜HikukaHikanaika〜』\n";
                fullResult += "═══════════════════════════════════════════════════════════\n\n";
                
                // 物語の始まり
                fullResult += "✨ 夕暮れ時の「運命の館」物語\n\n";
                fullResult += "クリスタルのシャンデリアが柔らかな光を放つ円形サロンで、\n";
                fullResult += "美しき参加者たちの運命の物語が今、始まろうとしている...\n\n";
                
                fullResult += "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n";
                
                // 各キャラクターの自己紹介を物語調で表示
                fullResult += "💝 美しき参加者たちの自己紹介\n\n";
                foreach (var intro in introductions)
                {
                    // キャラクター名を美しく装飾
                    fullResult += $"【 {intro.CharacterName} 】\n";
                    fullResult += "～ ～ ～ ～ ～ ～ ～ ～ ～ ～\n\n";
                    
                    // 自己紹介を整理して表示
                    string formattedIntroduction = FormatIntroductionText(intro.Introduction);
                    fullResult += formattedIntroduction;
                    fullResult += "\n\n";
                    
                    // 美容ステータスを詩的に表示
                    if (intro.BeautyStats != null)
                    {
                        fullResult += "💫 彼女の纏う美しさ:\n";
                        foreach (var stat in intro.BeautyStats)
                        {
                            if (stat.Key != "total_points")
                            {
                                fullResult += $"   ✧ {GetPoeticalStatName(stat.Key)}: {stat.Value}\n";
                            }
                        }
                        if (intro.BeautyStats.ContainsKey("total_points"))
                        {
                            fullResult += $"   ⭐ 魅力の総合値: {intro.BeautyStats["total_points"]} points\n";
                        }
                    }
                    
                    fullResult += "\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n";
                }
                
                // 語り部による解説
                fullResult += "📖 語り部による物語の解説\n\n";
                fullResult += "♦ ♦ ♦ ♦ ♦ ♦ ♦ ♦ ♦ ♦ ♦ ♦ ♦ ♦ ♦ ♦ ♦ ♦\n\n";
                
                string formattedExplanation = FormatNarrativeText(explanation);
                fullResult += formattedExplanation;
                
                fullResult += "\n\n♦ ♦ ♦ ♦ ♦ ♦ ♦ ♦ ♦ ♦ ♦ ♦ ♦ ♦ ♦ ♦ ♦ ♦\n\n";
                fullResult += "夕暮れの光が次第に薄れゆく中、\n";
                fullResult += "新たな物語はまさに始まろうとしていた...\n\n";
                fullResult += "～ To be continued... ～";
                
                judgeResultText.text = fullResult;
                
                // スクロールを一番上に戻す
                if (resultScrollRect != null)
                {
                    resultScrollRect.verticalNormalizedPosition = 1f;
                }
            }
        }
        
        private string FormatIntroductionText(string introduction)
        {
            // 自己紹介テキストを読みやすくフォーマット
            string formatted = introduction.Replace("。", "。\n");
            formatted = formatted.Replace("！", "！\n");
            formatted = formatted.Replace("？", "？\n");
            formatted = formatted.Replace("\n\n", "\n");
            formatted = formatted.Trim();
            
            // 行の先頭にインデントを追加
            string[] lines = formatted.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(lines[i]))
                {
                    lines[i] = "   " + lines[i];
                }
            }
            
            return string.Join("\n", lines);
        }
        
        private string FormatNarrativeText(string narrative)
        {
            // ナラティブテキストを段落ごとに整理
            string formatted = narrative.Replace("。", "。\n\n");
            formatted = formatted.Replace("！", "！\n\n");
            formatted = formatted.Replace("？", "？\n\n");
            formatted = formatted.Replace("\n\n\n", "\n\n");
            formatted = formatted.Trim();
            
            return formatted;
        }
        
        private string GetPoeticalStatName(string statKey)
        {
            switch (statKey)
            {
                case "fashion": return "装いの美しさ";
                case "family_wealth": return "家柄の気品";
                case "personality": return "心の輝き";
                default: return statKey;
            }
        }
        
        public void SetLoading(bool isLoading)
        {
            if (loadingIndicator != null)
                loadingIndicator.SetActive(isLoading);
                
            if (loadingText != null)
            {
                loadingText.text = isLoading ? "AI解説生成中..." : "";
                loadingText.gameObject.SetActive(isLoading);
            }
            
            // ボタンを無効化
            if (judgeButton != null)
                judgeButton.interactable = !isLoading;
            if (aiJudgeButton != null)
                aiJudgeButton.interactable = !isLoading;
        }
        
        public void ShowError(string errorMessage)
        {
            if (errorText != null)
            {
                errorText.text = $"❌ エラー: {errorMessage}";
                errorText.gameObject.SetActive(true);
                
                // 3秒後にエラーメッセージを非表示
                Invoke(nameof(HideError), 3f);
            }
            
            Debug.LogError($"RealityShowView Error: {errorMessage}");
        }
        
        public void ShowMessage(string message)
        {
            if (errorText != null)
            {
                errorText.text = $"✅ {message}";
                errorText.gameObject.SetActive(true);
                
                // 3秒後にメッセージを非表示
                Invoke(nameof(HideError), 3f);
            }
        }
        
        private void HideError()
        {
            if (errorText != null)
                errorText.gameObject.SetActive(false);
        }
    }
}
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
                string fullResult = "🎬 AI司会者によるリアリティーショー解説\n";
                fullResult += "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n";
                
                // 各キャラクターの自己紹介を表示
                fullResult += "👥 参加者自己紹介\n\n";
                foreach (var intro in introductions)
                {
                    fullResult += $"【{intro.CharacterName}】\n";
                    fullResult += $"{intro.Introduction}\n\n";
                }
                
                fullResult += "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n";
                fullResult += "📺 司会者解説\n\n";
                fullResult += explanation;
                
                judgeResultText.text = fullResult;
                
                // スクロールを一番上に戻す
                if (resultScrollRect != null)
                {
                    resultScrollRect.verticalNormalizedPosition = 1f;
                }
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
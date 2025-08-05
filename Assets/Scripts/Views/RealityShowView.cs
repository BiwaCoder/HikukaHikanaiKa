using UnityEngine;
using UnityEngine.UI;
using HikukaHikanaika.Models;
using System.Collections.Generic;

namespace HikukaHikanaika.Views
{
    public class RealityShowView : MonoBehaviour
    {
        [Header("UI References")]
        public Button judgeButton;
        public Text judgeResultText;
        public Text rivalStatusText;
        
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
    }
}
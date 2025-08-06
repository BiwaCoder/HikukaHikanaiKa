using UnityEngine;
using UnityEngine.UI;

namespace HikukaHikanaika.Views
{
    public class CharacterJudgmentView : MonoBehaviour
    {
        [Header("UI Controls")]
        public Button judgeButton;
        public GameObject judgingPanel;
        public GameObject resultPanel;
        
        [Header("Result Display")]
        public Text judgmentText;
        public Text analysisText;
        public Text scoreText;
        public Text errorText;
        public Text saveStatusText;
        
        [Header("Character Info Display")]
        public Text characterInfoText;
        
        void Start()
        {
            InitializeUI();
        }
        
        private void InitializeUI()
        {
            if (judgingPanel != null)
                judgingPanel.SetActive(false);
                
            if (resultPanel != null)
                resultPanel.SetActive(false);
                
            if (errorText != null)
                errorText.gameObject.SetActive(false);
                
            if (saveStatusText != null)
                saveStatusText.gameObject.SetActive(false);
        }
        
        public void SetJudging(bool isJudging)
        {
            if (judgeButton != null)
                judgeButton.interactable = !isJudging;
                
            if (judgingPanel != null)
                judgingPanel.SetActive(isJudging);
                
            if (errorText != null)
                errorText.gameObject.SetActive(false);
        }
        
        public void ShowJudgmentResult(string judgment, string analysis, int score)
        {
            if (resultPanel != null)
                resultPanel.SetActive(true);
                
            if (judgmentText != null)
                judgmentText.text = $"総合判定: {judgment}";
                
            if (analysisText != null)
                analysisText.text = analysis;
                
            if (scoreText != null)
                scoreText.text = $"スコア: {score}/100点";
                
            if (errorText != null)
                errorText.gameObject.SetActive(false);
        }
        
        public void ShowError(string message)
        {
            if (errorText != null)
            {
                errorText.text = $"エラー: {message}";
                errorText.gameObject.SetActive(true);
            }
            
            if (resultPanel != null)
                resultPanel.SetActive(false);
        }
        
        public void ShowSaveConfirmation(string filePath)
        {
            if (saveStatusText != null)
            {
                saveStatusText.text = $"結果を保存しました\n{System.IO.Path.GetFileName(filePath)}";
                saveStatusText.gameObject.SetActive(true);
            }
        }
        
        public void UpdateCharacterInfo(string characterData)
        {
            if (characterInfoText != null)
                characterInfoText.text = characterData;
        }
        
        public void ClearResults()
        {
            if (resultPanel != null)
                resultPanel.SetActive(false);
                
            if (errorText != null)
                errorText.gameObject.SetActive(false);
                
            if (saveStatusText != null)
                saveStatusText.gameObject.SetActive(false);
        }
    }
}
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using HikukaHikanaika.Models;
using HikukaHikanaika.Views;
using Newtonsoft.Json.Linq;

namespace HikukaHikanaika.Controllers
{
    public class CharacterJudgmentController : MonoBehaviour
    {
        [Header("MVC Components")]
        public CharacterJudgmentView judgmentView;
        
        [Header("API Settings")]
        [SerializeField] private string apiUrl = "http://localhost:5001";
        
        private CharacterJudgmentModel judgmentModel;
        private GachaController gachaController;
        
        void Start()
        {
            InitializeComponents();
            SetupEventListeners();
        }
        
        private void InitializeComponents()
        {
            judgmentModel = new CharacterJudgmentModel();
            gachaController = FindObjectOfType<GachaController>();
            
            if (gachaController == null)
            {
                Debug.LogError("GachaController not found in scene!");
            }
        }
        
        private void SetupEventListeners()
        {
            if (judgmentView.judgeButton != null)
                judgmentView.judgeButton.onClick.AddListener(OnJudgeButtonClicked);
        }
        
        public void OnJudgeButtonClicked()
        {
            if (gachaController?.PlayerData == null)
            {
                judgmentView.ShowError("プレイヤーデータが見つかりません");
                return;
            }
            
            judgmentModel.SetCharacterData(gachaController.PlayerData);
            
            if (!HasRequiredItems())
            {
                judgmentView.ShowError("判定には服装、家柄、性格のすべてが必要です");
                return;
            }
            
            judgmentView.SetJudging(true);
            StartCoroutine(RequestJudgmentFromLLM());
        }
        
        private bool HasRequiredItems()
        {
            var playerData = gachaController.PlayerData;
            return playerData.CurrentOutfit != null &&
                   playerData.CurrentFamilyWealth != null &&
                   playerData.CurrentPersonality != null;
        }
        
        private IEnumerator RequestJudgmentFromLLM()
        {
            string characterData = judgmentModel.GetCharacterDataForLLM();
            
            JObject requestData = new JObject
            {
                { "character_data", characterData },
                { "prompt", CreateJudgmentPrompt(characterData) }
            };
            
            using (UnityWebRequest www = UnityWebRequest.Post($"{apiUrl}/judge_character", requestData.ToString()))
            {
                www.SetRequestHeader("Content-Type", "application/json");
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestData.ToString());
                using (var uploadHandler = new UploadHandlerRaw(bodyRaw))
                {
                    www.uploadHandler = uploadHandler;
                    yield return www.SendWebRequest();
                    
                    judgmentView.SetJudging(false);
                    
                    if (www.result == UnityWebRequest.Result.Success)
                    {
                        ProcessJudgmentResponse(www.downloadHandler.text);
                    }
                    else
                    {
                        Debug.LogError($"API Error: {www.error}");
                        judgmentView.ShowError($"API通信エラー: {www.error}");
                    }
                    uploadHandler.Dispose();
                }
            }
        }
        
        private string CreateJudgmentPrompt(string characterData)
        {
            return $@"あなたは厳格な審査員です。以下のキャラクター情報を基に、リアリティショーの参加者として評価してください。

{characterData}

以下の形式で回答してください：
1. 総合判定（S/A/B/C/Dの5段階）
2. 詳細な分析（200文字程度）
3. 数値スコア（0-100点）

審査基準：
- 服装の美容レベル
- 家柄の社会的地位
- 性格の魅力度
- 総合的な印象

厳しく、しかし建設的な評価をお願いします。";
        }
        
        private void ProcessJudgmentResponse(string jsonResponse)
        {
            try
            {
                JObject responseData = JObject.Parse(jsonResponse);
                
                string judgment = responseData["judgment"]?.ToString() ?? "評価不明";
                string analysis = responseData["analysis"]?.ToString() ?? "分析データなし";
                int score = responseData["score"]?.ToObject<int>() ?? 0;
                
                judgmentModel.SetJudgmentResult(judgment, analysis, score);
                
                judgmentView.ShowJudgmentResult(judgment, analysis, score);
                
                SaveJudgmentToFile();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Response parsing error: {e.Message}");
                judgmentView.ShowError("レスポンスの解析に失敗しました");
            }
        }
        
        private void SaveJudgmentToFile()
        {
            try
            {
                string fileName = $"judgment_{System.DateTime.Now:yyyyMMdd_HHmmss}.json";
                string filePath = Path.Combine(Application.persistentDataPath, fileName);
                
                string jsonData = judgmentModel.LatestJudgmentResult.ToJsonString();
                File.WriteAllText(filePath, jsonData);
                
                Debug.Log($"Judgment saved to: {filePath}");
                judgmentView.ShowSaveConfirmation(filePath);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"File save error: {e.Message}");
                judgmentView.ShowError("ファイル保存に失敗しました");
            }
        }
        
        public CharacterJudgmentModel JudgmentModel => judgmentModel;
    }
}
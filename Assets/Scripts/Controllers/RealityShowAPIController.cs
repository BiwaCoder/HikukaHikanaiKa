using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using HikukaHikanaika.Models;
using HikukaHikanaika.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HikukaHikanaika.Controllers
{
    public class RealityShowAPIController : MonoBehaviour
    {
        [Header("API Settings")]
        [SerializeField] private string apiUrl = "http://localhost:5001";
        
        [Header("MVC Components")]
        public RealityShowView realityShowView;
        public GachaController gachaController;
        
        private RealityShowModel realityShowModel;
        
        void Start()
        {
            InitializeComponents();
            SetupEventListeners();
        }
        
        private void InitializeComponents()
        {
            realityShowModel = new RealityShowModel();
        }
        
        private void SetupEventListeners()
        {
            if (realityShowView.judgeButton != null)
                realityShowView.judgeButton.onClick.AddListener(OnJudgeWithAIButtonClicked);
        }
        
        public void OnJudgeWithAIButtonClicked()
        {
            if (gachaController?.PlayerData == null)
            {
                realityShowView.ShowError("プレイヤーデータが見つかりません");
                return;
            }
            
            StartCoroutine(SimulateRealityShowWithAI());
        }
        
        private IEnumerator SimulateRealityShowWithAI()
        {
            realityShowView.SetLoading(true);
            
            // 1. まず各キャラクターの自己紹介を取得
            List<CharacterIntroduction> introductions = new List<CharacterIntroduction>();
            
            // プレイヤーの自己紹介
            yield return StartCoroutine(GetCharacterIntroduction("Player", gachaController.PlayerData, (result) => {
                if (result != null) introductions.Add(result);
            }));
            
            // ライバルキャラクターの自己紹介
            foreach (var rival in realityShowModel.Rivals)
            {
                yield return StartCoroutine(GetRivalIntroduction(rival, (result) => {
                    if (result != null) introductions.Add(result);
                }));
            }
            
            // 2. リアリティーショーの解説を取得
            if (introductions.Count > 0)
            {
                yield return StartCoroutine(GetRealityShowExplanation(introductions));
            }
            
            realityShowView.SetLoading(false);
        }
        
        private IEnumerator GetCharacterIntroduction(string characterName, PlayerData playerData, System.Action<CharacterIntroduction> callback)
        {
            // プレイヤーデータを美容ステータス形式に変換
            var beautyStats = new Dictionary<string, string>
            {
                {"fashion", playerData.CurrentOutfit?.name ?? "なし"},
                {"family_wealth", playerData.CurrentFamilyWealth?.name ?? "なし"},
                {"personality", playerData.CurrentPersonality?.name ?? "なし"},
                {"total_points", playerData.GetTotalPoints().ToString()}
            };
            
            JObject requestData = new JObject
            {
                {"character_name", characterName},
                {"beauty_stats", JObject.FromObject(beautyStats)}
            };
            
            using (UnityWebRequest www = UnityWebRequest.Post($"{apiUrl}/character/introduce", requestData.ToString()))
            {
                www.SetRequestHeader("Content-Type", "application/json");
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestData.ToString());
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                
                yield return www.SendWebRequest();
                
                if (www.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        JObject responseData = JObject.Parse(www.downloadHandler.text);
                        var introduction = new CharacterIntroduction
                        {
                            CharacterName = responseData["character_name"]?.ToString() ?? characterName,
                            Introduction = responseData["introduction"]?.ToString() ?? "自己紹介データなし",
                            BeautyStats = beautyStats
                        };
                        callback(introduction);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Introduction response parsing error: {e.Message}");
                        callback(null);
                    }
                }
                else
                {
                    Debug.LogError($"Character introduction API error: {www.error}");
                    callback(null);
                }
            }
        }
        
        private IEnumerator GetRivalIntroduction(RivalData rival, System.Action<CharacterIntroduction> callback)
        {
            // ライバルデータを美容ステータス形式に変換
            var beautyStats = new Dictionary<string, string>
            {
                {"fashion", rival.Outfit?.name ?? "なし"},
                {"family_wealth", rival.FamilyWealth?.name ?? "なし"},
                {"personality", rival.Personality?.name ?? "なし"},
                {"total_points", rival.GetTotalPoints().ToString()}
            };
            
            JObject requestData = new JObject
            {
                {"character_name", rival.Name},
                {"beauty_stats", JObject.FromObject(beautyStats)}
            };
            
            using (UnityWebRequest www = UnityWebRequest.Post($"{apiUrl}/character/introduce", requestData.ToString()))
            {
                www.SetRequestHeader("Content-Type", "application/json");
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestData.ToString());
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                
                yield return www.SendWebRequest();
                
                if (www.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        JObject responseData = JObject.Parse(www.downloadHandler.text);
                        var introduction = new CharacterIntroduction
                        {
                            CharacterName = responseData["character_name"]?.ToString() ?? rival.Name,
                            Introduction = responseData["introduction"]?.ToString() ?? "自己紹介データなし",
                            BeautyStats = beautyStats
                        };
                        callback(introduction);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Rival introduction response parsing error: {e.Message}");
                        callback(null);
                    }
                }
                else
                {
                    Debug.LogError($"Rival introduction API error: {www.error}");
                    callback(null);
                }
            }
        }
        
        private IEnumerator GetRealityShowExplanation(List<CharacterIntroduction> contestants)
        {
            // 参加者情報をAPI形式に変換
            JArray contestantArray = new JArray();
            foreach (var contestant in contestants)
            {
                JObject contestantObj = new JObject
                {
                    {"character_name", contestant.CharacterName},
                    {"introduction", contestant.Introduction},
                    {"beauty_stats", JObject.FromObject(contestant.BeautyStats)}
                };
                contestantArray.Add(contestantObj);
            }
            
            JObject requestData = new JObject
            {
                {"contestants", contestantArray}
            };
            
            using (UnityWebRequest www = UnityWebRequest.Post($"{apiUrl}/reality_show/explain", requestData.ToString()))
            {
                www.SetRequestHeader("Content-Type", "application/json");
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestData.ToString());
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                
                yield return www.SendWebRequest();
                
                if (www.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        JObject responseData = JObject.Parse(www.downloadHandler.text);
                        string explanation = responseData["explanation"]?.ToString() ?? "解説データなし";
                        
                        // 結果をUIに表示
                        realityShowView.ShowAIRealityShowResult(contestants, explanation);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Reality show explanation response parsing error: {e.Message}");
                        realityShowView.ShowError("リアリティーショー解説の解析に失敗しました");
                    }
                }
                else
                {
                    Debug.LogError($"Reality show explanation API error: {www.error}");
                    realityShowView.ShowError($"リアリティーショー解説API エラー: {www.error}");
                }
            }
        }
        
        public void TestAPIConnection()
        {
            StartCoroutine(TestHealthCheck());
        }
        
        private IEnumerator TestHealthCheck()
        {
            using (UnityWebRequest www = UnityWebRequest.Get($"{apiUrl}/health"))
            {
                yield return www.SendWebRequest();
                
                if (www.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"API Health Check Success: {www.downloadHandler.text}");
                    realityShowView.ShowMessage("APIサーバー接続成功！");
                }
                else
                {
                    Debug.LogError($"API Health Check Failed: {www.error}");
                    realityShowView.ShowError($"APIサーバー接続失敗: {www.error}");
                }
            }
        }
        
        [System.Serializable]
        public class CharacterIntroduction
        {
            public string CharacterName;
            public string Introduction;
            public Dictionary<string, string> BeautyStats;
        }
    }
}
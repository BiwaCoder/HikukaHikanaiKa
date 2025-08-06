using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using HikukaHikanaika.Models;
using Newtonsoft.Json;
using System;

namespace HikukaHikanaika.Utils
{
    public class APIClient : MonoBehaviour
    {
        private static APIClient _instance;
        public static APIClient Instance 
        { 
            get 
            {
                if (_instance == null)
                {
                    GameObject apiClientObject = new GameObject("APIClient");
                    _instance = apiClientObject.AddComponent<APIClient>();
                    DontDestroyOnLoad(apiClientObject);
                }
                return _instance;
            }
        }
        
        [SerializeField] private string baseUrl = "http://localhost:5001";
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        public void SetBaseUrl(string url)
        {
            baseUrl = url;
        }
        
        public void GetHealthCheck(Action<bool, string> callback)
        {
            StartCoroutine(GetHealthCheckCoroutine(callback));
        }
        
        private IEnumerator GetHealthCheckCoroutine(Action<bool, string> callback)
        {
            using (UnityWebRequest www = UnityWebRequest.Get($"{baseUrl}/health"))
            {
                yield return www.SendWebRequest();
                
                if (www.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        var response = JsonConvert.DeserializeObject<HealthCheckResponse>(www.downloadHandler.text);
                        callback(true, response.message);
                    }
                    catch (Exception e)
                    {
                        callback(false, $"Response parsing error: {e.Message}");
                    }
                }
                else
                {
                    callback(false, $"Request failed: {www.error}");
                }
            }
        }
        
        public void GetCharacterList(Action<bool, List<string>, string> callback)
        {
            StartCoroutine(GetCharacterListCoroutine(callback));
        }
        
        private IEnumerator GetCharacterListCoroutine(Action<bool, List<string>, string> callback)
        {
            using (UnityWebRequest www = UnityWebRequest.Get($"{baseUrl}/characters"))
            {
                yield return www.SendWebRequest();
                
                if (www.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        var response = JsonConvert.DeserializeObject<CharacterListResponse>(www.downloadHandler.text);
                        callback(true, response.characters, null);
                    }
                    catch (Exception e)
                    {
                        callback(false, null, $"Response parsing error: {e.Message}");
                    }
                }
                else
                {
                    callback(false, null, $"Request failed: {www.error}");
                }
            }
        }
        
        public void GetCharacterIntroduction(string characterName, Dictionary<string, string> beautyStats, Action<bool, CharacterIntroductionResponse, string> callback)
        {
            StartCoroutine(GetCharacterIntroductionCoroutine(characterName, beautyStats, callback));
        }
        
        private IEnumerator GetCharacterIntroductionCoroutine(string characterName, Dictionary<string, string> beautyStats, Action<bool, CharacterIntroductionResponse, string> callback)
        {
            var requestData = new
            {
                character_name = characterName,
                beauty_stats = beautyStats
            };
            
            string jsonData = JsonConvert.SerializeObject(requestData);
            
            using (UnityWebRequest www = UnityWebRequest.Post($"{baseUrl}/character/introduce", jsonData))
            {
                www.SetRequestHeader("Content-Type", "application/json");
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                
                yield return www.SendWebRequest();
                
                if (www.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        var response = JsonConvert.DeserializeObject<CharacterIntroductionResponse>(www.downloadHandler.text);
                        callback(true, response, null);
                    }
                    catch (Exception e)
                    {
                        callback(false, null, $"Response parsing error: {e.Message}");
                    }
                }
                else
                {
                    callback(false, null, $"Request failed: {www.error}");
                }
            }
        }
        
        public void GetRealityShowExplanation(List<ContestantData> contestants, Action<bool, RealityShowExplanationResponse, string> callback)
        {
            StartCoroutine(GetRealityShowExplanationCoroutine(contestants, callback));
        }
        
        private IEnumerator GetRealityShowExplanationCoroutine(List<ContestantData> contestants, Action<bool, RealityShowExplanationResponse, string> callback)
        {
            var requestData = new
            {
                contestants = contestants
            };
            
            string jsonData = JsonConvert.SerializeObject(requestData);
            
            using (UnityWebRequest www = UnityWebRequest.Post($"{baseUrl}/reality_show/explain", jsonData))
            {
                www.SetRequestHeader("Content-Type", "application/json");
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                
                yield return www.SendWebRequest();
                
                if (www.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        var response = JsonConvert.DeserializeObject<RealityShowExplanationResponse>(www.downloadHandler.text);
                        callback(true, response, null);
                    }
                    catch (Exception e)
                    {
                        callback(false, null, $"Response parsing error: {e.Message}");
                    }
                }
                else
                {
                    callback(false, null, $"Request failed: {www.error}");
                }
            }
        }
        
        public void SimulateRealityShow(List<string> characters, List<Dictionary<string, string>> beautyStatsList, Action<bool, RealityShowSimulationResponse, string> callback)
        {
            StartCoroutine(SimulateRealityShowCoroutine(characters, beautyStatsList, callback));
        }
        
        private IEnumerator SimulateRealityShowCoroutine(List<string> characters, List<Dictionary<string, string>> beautyStatsList, Action<bool, RealityShowSimulationResponse, string> callback)
        {
            var requestData = new RealityShowSimulationRequest
            {
                characters = characters,
                beauty_stats_list = beautyStatsList
            };
            
            string jsonData = JsonConvert.SerializeObject(requestData);
            
            using (UnityWebRequest www = UnityWebRequest.Post($"{baseUrl}/reality_show/simulate", jsonData))
            {
                www.SetRequestHeader("Content-Type", "application/json");
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                
                yield return www.SendWebRequest();
                
                if (www.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        var response = JsonConvert.DeserializeObject<RealityShowSimulationResponse>(www.downloadHandler.text);
                        callback(true, response, null);
                    }
                    catch (Exception e)
                    {
                        callback(false, null, $"Response parsing error: {e.Message}");
                    }
                }
                else
                {
                    callback(false, null, $"Request failed: {www.error}");
                }
            }
        }
    }
}
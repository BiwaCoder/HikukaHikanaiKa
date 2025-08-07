using System;
using System.Collections.Generic;
using UnityEngine;

namespace HikukaHikanaika.Models
{
    [System.Serializable]
    public class CharacterJudgmentData
    {
        public string playerName;
        public string outfit;
        public int outfitPoints;
        public string familyWealth;
        public int familyWealthPoints;
        public string personality;
        public int personalityPoints;
        public int totalPoints;
        public DateTime createdAt;
        
        public CharacterJudgmentData(PlayerData playerData)
        {
            playerName = playerData.PlayerName;
            outfit = playerData.CurrentOutfit?.name ?? "何も着ていない";
            outfitPoints = playerData.CurrentOutfit?.points ?? 0;
            familyWealth = playerData.CurrentFamilyWealth?.name ?? "特になし";
            familyWealthPoints = playerData.CurrentFamilyWealth?.points ?? 0;
            personality = playerData.CurrentPersonality?.name ?? "特になし";
            personalityPoints = playerData.CurrentPersonality?.points ?? 0;
            totalPoints = playerData.GetTotalPoints();
            createdAt = DateTime.Now;
        }
        
        public string ToJsonString()
        {
            return JsonUtility.ToJson(this, true);
        }
    }
    
    [System.Serializable]
    public class JudgmentResult
    {
        public string judgment;
        public string analysis;
        public int score;
        public DateTime judgedAt;
        public CharacterJudgmentData characterData;
        
        public JudgmentResult()
        {
            judgedAt = DateTime.Now;
        }
        
        public string ToJsonString()
        {
            return JsonUtility.ToJson(this, true);
        }
    }
    
    public class CharacterJudgmentModel
    {
        public CharacterJudgmentData CurrentCharacterData { get; private set; }
        public JudgmentResult LatestJudgmentResult { get; private set; }
        public List<JudgmentResult> JudgmentHistory { get; private set; }
        
        public CharacterJudgmentModel()
        {
            JudgmentHistory = new List<JudgmentResult>();
        }
        
        public void SetCharacterData(PlayerData playerData)
        {
            CurrentCharacterData = new CharacterJudgmentData(playerData);
        }
        
        public void SetJudgmentResult(string judgment, string analysis, int score)
        {
            LatestJudgmentResult = new JudgmentResult
            {
                judgment = judgment,
                analysis = analysis,
                score = score,
                characterData = CurrentCharacterData
            };
            
            JudgmentHistory.Add(LatestJudgmentResult);
        }
        
        public string GetCharacterDataForLLM()
        {
            if (CurrentCharacterData == null) return string.Empty;
            
            return $@"プレイヤー名: {CurrentCharacterData.playerName}
服装: {CurrentCharacterData.outfit} (美容ポイント: {CurrentCharacterData.outfitPoints})
家柄: {CurrentCharacterData.familyWealth} (家柄ポイント: {CurrentCharacterData.familyWealthPoints})
性格: {CurrentCharacterData.personality} (性格ポイント: {CurrentCharacterData.personalityPoints})
総合ポイント: {CurrentCharacterData.totalPoints}";
        }
    }
}
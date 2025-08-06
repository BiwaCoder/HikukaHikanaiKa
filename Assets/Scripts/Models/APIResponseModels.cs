using System;
using System.Collections.Generic;

namespace HikukaHikanaika.Models
{
    [Serializable]
    public class CharacterIntroductionResponse
    {
        public string character_name;
        public CharacterInfo character_info;
        public Dictionary<string, string> beauty_stats;
        public string introduction;
    }
    
    [Serializable]
    public class CharacterInfo
    {
        public string name;
        public string personality;
        public string traits;
        public string speech_style;
        public string background;
        public string beauty_philosophy;
        public string catchphrase;
    }
    
    [Serializable]
    public class RealityShowExplanationResponse
    {
        public List<ContestantData> contestants;
        public string explanation;
        public string show_title;
    }
    
    [Serializable]
    public class ContestantData
    {
        public string character_name;
        public CharacterInfo character_info;
        public Dictionary<string, string> beauty_stats;
        public string introduction;
    }
    
    [Serializable]
    public class CharacterListResponse
    {
        public List<string> characters;
    }
    
    [Serializable]
    public class CharacterDetailResponse
    {
        public string character_name;
        public CharacterInfo character_info;
    }
    
    [Serializable]
    public class APIErrorResponse
    {
        public string error;
    }
    
    [Serializable]
    public class HealthCheckResponse
    {
        public string status;
        public string message;
    }
    
    [Serializable]
    public class RealityShowSimulationRequest
    {
        public List<string> characters;
        public List<Dictionary<string, string>> beauty_stats_list;
    }
    
    [Serializable]
    public class RealityShowSimulationResponse
    {
        public List<CharacterIntroductionResponse> contestant_introductions;
        public RealityShowExplanationResponse show_explanation;
    }
}
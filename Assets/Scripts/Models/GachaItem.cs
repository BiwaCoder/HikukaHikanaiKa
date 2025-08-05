using System;

namespace HikukaHikanaika.Models
{
    [Serializable]
    public class GachaItem
    {
        public string name;
        public float probability;
        public int points;
        public GachaType type;
        
        public GachaItem(string name, float probability, int points, GachaType type)
        {
            this.name = name;
            this.probability = probability;
            this.points = points;
            this.type = type;
        }
    }
    
    public enum GachaType
    {
        Beauty,
        FamilyWealth,
        Personality
    }
}
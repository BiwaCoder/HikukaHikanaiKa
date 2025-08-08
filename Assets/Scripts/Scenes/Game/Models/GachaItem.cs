using System;

namespace HikukaHikanaika.Models
{
    [Serializable]
    public class GachaItem
    {
        public string name;
        public float probability;
        public int points; // 既存の外見ポイント
        public int luck; // 運
        public int concentration; // 集中力
        public int kindness; // 優しさ
        public GachaType type;
        
        public GachaItem(string name, float probability, int points, int luck, int concentration, int kindness, GachaType type)
        {
            this.name = name;
            this.probability = probability;
            this.points = points;
            this.luck = luck;
            this.concentration = concentration;
            this.kindness = kindness;
            this.type = type;
        }
    }
    
    public enum GachaType
    {
        Beauty,
        FamilyWealth,
        Personality,
        Luck,
        Concentration,
        Kindness
    }
}
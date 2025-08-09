using System;

namespace HikukaHikanaika.Models
{
    [Serializable]
    public class GachaItem
    {
        public string name;
        public float probability;
        public int points; // 既存の外見片
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
        Kindness,
        // ライフステージ限定ガチャ
        Childhood,      // 幼児期限定 (0-10歳)
        Youth,          // 青春期限定 (15-25歳)
        Career,         // 就職期限定 (20-40歳)
        Mature,         // 成熟期限定 (40-60歳)
        Senior,         // シニア期限定 (60歳以上)
        // 特別解放ガチャ
        Legendary,      // 大成功で解放
        Redemption,     // 大失敗で解放
        Love,           // 恋愛イベント成功で解放
        // 30歳限定逆転ガチャ
        Reversal,       // 30歳で自動解放される逆転ガチャ
        // 追加の特殊ガチャ
        Cursed,         // 呪いガチャ（ネガティブ特化）
        Academic,       // 学術ガチャ（集中力特化・高コスト）
        Social,         // 社交ガチャ（優しさ特化・高コスト）
        Gambler         // ギャンブラーガチャ（運特化・高コスト）
    }
    
    public enum GachaUnlockResult
    {
        GreatFailure,   // 大失敗
        Failure,        // 失敗
        Success,        // 成功
        GreatSuccess    // 大成功
    }
}
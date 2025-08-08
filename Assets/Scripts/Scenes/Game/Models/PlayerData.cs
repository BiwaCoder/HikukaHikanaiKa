using System.Collections.Generic;
using UnityEngine;

namespace HikukaHikanaika.Models
{
    public class PlayerData
    {
        private static PlayerData instance;
        public static PlayerData Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PlayerData(); // デフォルト人生開始
                }
                return instance;
            }
        }
        
        public string PlayerName { get; set; } = "あなた";
        public int CurrentAge { get; set; } = 0;
        public int RemainingLifespan { get; set; } = 80; // 残り寿命（年）
        public int LifeCycle { get; set; } = 0; // 現在のライフサイクル（0-15）
        
        // 所持アイテム
        private HashSet<string> ownedBeautyItems = new HashSet<string>();
        private HashSet<string> ownedFamilyWealthItems = new HashSet<string>();
        private HashSet<string> ownedPersonalityItems = new HashSet<string>();
        
        // 現在の装備
        public GachaItem CurrentOutfit { get; set; }
        public GachaItem CurrentFamilyWealth { get; set; }
        public GachaItem CurrentPersonality { get; set; }
        
        private PlayerData()
        {
            CurrentAge = 0;
            RemainingLifespan = 80;
            LifeCycle = 0;
        }
        
        public static void Initialize()
        {
            instance = new PlayerData();
        }
        
        public static void Reset()
        {
            instance = null;
        }
        
        public bool HasItem(string itemName, GachaType type)
        {
            return GetOwnedItemsSet(type).Contains(itemName);
        }
        
        public void AddItem(string itemName, GachaType type)
        {
            GetOwnedItemsSet(type).Add(itemName);
        }
        
        public HashSet<string> GetOwnedItems(GachaType type)
        {
            return GetOwnedItemsSet(type);
        }
        
        public int GetTotalPoints()
        {
            int total = 0;
            total += CurrentOutfit?.points ?? 0;
            total += CurrentFamilyWealth?.points ?? 0;
            total += CurrentPersonality?.points ?? 0;
            return total;
        }
        
        public void SpendLifespan(int years)
        {
            RemainingLifespan -= years;
            if (RemainingLifespan < 0) RemainingLifespan = 0;
        }
        
        public bool CanAffordLifespan(int years)
        {
            return RemainingLifespan >= years;
        }
        
        public void AdvanceAge()
        {
            LifeCycle++;
            CurrentAge = LifeCycle * 5; // 5年毎
        }
        
        public bool IsAlive()
        {
            return RemainingLifespan > 0 && CurrentAge < 80;
        }
        
        public bool IsGameOver()
        {
            return RemainingLifespan <= 0 || CurrentAge >= 80;
        }
        
        public string GetLifeStage()
        {
            switch (LifeCycle)
            {
                case 0: return "幼児期 (0-5歳)";
                case 1: return "児童期 (5-10歳)";
                case 2: return "思春期 (10-15歳)";
                case 3: return "青春期 (15-20歳)";
                case 4: return "就職期 (20-25歳)";
                case 5: return "成人期 (25-30歳)";
                case 6: return "責任期 (30-35歳)";
                case 7: return "充実期 (35-40歳)";
                case 8: return "中年期 (40-45歳)";
                case 9: return "成熟期 (45-50歳)";
                case 10: return "転換期 (50-55歳)";
                case 11: return "安定期 (55-60歳)";
                case 12: return "準備期 (60-65歳)";
                case 13: return "シニア期 (65-70歳)";
                case 14: return "長老期 (70-75歳)";
                case 15: return "晩年期 (75-80歳)";
                default: return "人生終了";
            }
        }
        
        private HashSet<string> GetOwnedItemsSet(GachaType type)
        {
            switch (type)
            {
                case GachaType.Beauty: return ownedBeautyItems;
                case GachaType.FamilyWealth: return ownedFamilyWealthItems;
                case GachaType.Personality: return ownedPersonalityItems;
                default: return ownedBeautyItems;
            }
        }
    }
}
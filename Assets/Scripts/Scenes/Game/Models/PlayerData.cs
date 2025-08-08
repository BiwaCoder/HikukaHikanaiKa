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
                    instance = new PlayerData(300000000); // デフォルト3億円
                }
                return instance;
            }
        }
        
        public string PlayerName { get; set; } = "あなた";
        public int Money { get; set; }
        
        // 所持アイテム
        private HashSet<string> ownedBeautyItems = new HashSet<string>();
        private HashSet<string> ownedFamilyWealthItems = new HashSet<string>();
        private HashSet<string> ownedPersonalityItems = new HashSet<string>();
        
        // 現在の装備
        public GachaItem CurrentOutfit { get; set; }
        public GachaItem CurrentFamilyWealth { get; set; }
        public GachaItem CurrentPersonality { get; set; }
        
        private PlayerData(int initialMoney)
        {
            Money = initialMoney;
        }
        
        public static void Initialize(int initialMoney = 300000000)
        {
            instance = new PlayerData(initialMoney);
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
        
        public void SpendMoney(int amount)
        {
            Money -= amount;
        }
        
        public bool CanAfford(int amount)
        {
            return Money >= amount;
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
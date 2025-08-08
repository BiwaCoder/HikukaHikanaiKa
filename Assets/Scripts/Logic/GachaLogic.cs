using UnityEngine;
using HikukaHikanaika.Models;

namespace HikukaHikanaika.Logic
{
    public class GachaLogic
    {
        private static GachaLogic instance;
        public static GachaLogic Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GachaLogic();
                }
                return instance;
            }
        }
        
        private GachaModel gachaModel;
        
        private GachaLogic()
        {
            gachaModel = new GachaModel();
        }
        
        public static void Initialize()
        {
            instance = new GachaLogic();
        }
        
        public static void Reset()
        {
            instance = null;
        }
        
        public PlayerData PlayerData => PlayerData.Instance;
        
        public bool CanPerformGacha(GachaType gachaType)
        {
            int cost = gachaModel.GetGachaCost(gachaType);
            return PlayerData.CanAfford(cost);
        }
        
        public string PerformGacha(GachaType gachaType)
        {
            int cost = gachaModel.GetGachaCost(gachaType);
            
            if (!PlayerData.CanAfford(cost))
            {
                return "💸 お金が足りません！";
            }
            
            PlayerData.SpendMoney(cost);
            GachaItem pulledItem = gachaModel.PullGacha(gachaType);
            return ProcessGachaResult(pulledItem, gachaType);
        }
        
        private string ProcessGachaResult(GachaItem item, GachaType gachaType)
        {
            if (PlayerData.HasItem(item.name, gachaType))
            {
                return $"🎴 結果: {item.name}\n🔄 既に所持しています。";
            }
            
            PlayerData.AddItem(item.name, gachaType);
            return UpdateCurrentEquipment(item, gachaType);
        }
        
        private string UpdateCurrentEquipment(GachaItem item, GachaType gachaType)
        {
            switch (gachaType)
            {
                case GachaType.Beauty:
                    if (PlayerData.CurrentOutfit == null || item.points > PlayerData.CurrentOutfit.points)
                    {
                        string previous = PlayerData.CurrentOutfit?.name ?? "何も着ていない";
                        PlayerData.CurrentOutfit = item;
                        return $"🎴 結果: {item.name}\n👗 新しい服を装備！\n✨ 美容: {item.points} (前: {previous})";
                    }
                    else
                    {
                        return $"🎴 結果: {item.name}\n📦 ワードローブに追加。\n現在の装備の方が良いので着替えません。";
                    }
                
                case GachaType.FamilyWealth:
                    if (PlayerData.CurrentFamilyWealth == null || item.points > PlayerData.CurrentFamilyWealth.points)
                    {
                        string previous = PlayerData.CurrentFamilyWealth?.name ?? "特になし";
                        PlayerData.CurrentFamilyWealth = item;
                        return $"🎴 結果: {item.name}\n🏠 新しい家柄を設定！\n💰 家柄: {item.points} (前: {previous})";
                    }
                    else
                    {
                        return $"🎴 結果: {item.name}\n💼 プロフィールに追加。\n現在の家柄の方が良いので変更しません。";
                    }
                
                case GachaType.Personality:
                    if (PlayerData.CurrentPersonality == null || item.points > PlayerData.CurrentPersonality.points)
                    {
                        string previous = PlayerData.CurrentPersonality?.name ?? "特になし";
                        PlayerData.CurrentPersonality = item;
                        return $"🎴 結果: {item.name}\n😊 新しい性格を設定！\n🎆 性格: {item.points} (前: {previous})";
                    }
                    else
                    {
                        return $"🎴 結果: {item.name}\n📝 プロフィールに追加。\n現在の性格の方が良いので変更しません。";
                    }
                
                default:
                    return $"🎴 結果: {item.name}";
            }
        }
    }
}
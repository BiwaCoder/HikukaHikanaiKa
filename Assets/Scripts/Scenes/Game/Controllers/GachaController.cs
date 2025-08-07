using UnityEngine;
using HikukaHikanaika.Models;
using HikukaHikanaika.Views;

namespace HikukaHikanaika.Controllers
{
    public class GachaController : MonoBehaviour
    {
        [Header("MVC Components")]
        public GachaView gachaView;
        
        private GachaModel gachaModel;
        private PlayerData playerData;
        
        void Start()
        {
            InitializeComponents();
            SetupEventListeners();
            UpdateUI();
        }
        
        private void InitializeComponents()
        {
            gachaModel = new GachaModel();
            playerData = new PlayerData(300000000); // 3億円
        }
        
        private void SetupEventListeners()
        {
            if (gachaView.beautyGachaButton != null)
                gachaView.beautyGachaButton.onClick.AddListener(() => OnGachaButtonClicked(GachaType.Beauty));
            if (gachaView.familyWealthGachaButton != null)
                gachaView.familyWealthGachaButton.onClick.AddListener(() => OnGachaButtonClicked(GachaType.FamilyWealth));
            if (gachaView.personalityGachaButton != null)
                gachaView.personalityGachaButton.onClick.AddListener(() => OnGachaButtonClicked(GachaType.Personality));
        }
        
        private void OnGachaButtonClicked(GachaType gachaType)
        {
            int cost = gachaModel.GetGachaCost(gachaType);
            
            if (!playerData.CanAfford(cost))
            {
                gachaView.ShowInsufficientFunds();
                return;
            }
            
            playerData.SpendMoney(cost);
            GachaItem pulledItem = gachaModel.PullGacha(gachaType);
            string resultMessage = ProcessGachaResult(pulledItem, gachaType);
            
            gachaView.ShowGachaResult(resultMessage);
            UpdateUI();
        }
        
        private string ProcessGachaResult(GachaItem item, GachaType gachaType)
        {
            if (playerData.HasItem(item.name, gachaType))
            {
                return $"🎴 結果: {item.name}\n🔄 既に所持しています。";
            }
            
            playerData.AddItem(item.name, gachaType);
            return UpdateCurrentEquipment(item, gachaType);
        }
        
        private string UpdateCurrentEquipment(GachaItem item, GachaType gachaType)
        {
            switch (gachaType)
            {
                case GachaType.Beauty:
                    if (playerData.CurrentOutfit == null || item.points > playerData.CurrentOutfit.points)
                    {
                        string previous = playerData.CurrentOutfit?.name ?? "何も着ていない";
                        playerData.CurrentOutfit = item;
                        return $"🎴 結果: {item.name}\n👗 新しい服を装備！\n✨ 美容: {item.points} (前: {previous})";
                    }
                    else
                    {
                        return $"🎴 結果: {item.name}\n📦 ワードローブに追加。\n現在の装備の方が良いので着替えません。";
                    }
                
                case GachaType.FamilyWealth:
                    if (playerData.CurrentFamilyWealth == null || item.points > playerData.CurrentFamilyWealth.points)
                    {
                        string previous = playerData.CurrentFamilyWealth?.name ?? "特になし";
                        playerData.CurrentFamilyWealth = item;
                        return $"🎴 結果: {item.name}\n🏠 新しい家柄を設定！\n💰 家柄: {item.points} (前: {previous})";
                    }
                    else
                    {
                        return $"🎴 結果: {item.name}\n💼 プロフィールに追加。\n現在の家柄の方が良いので変更しません。";
                    }
                
                case GachaType.Personality:
                    if (playerData.CurrentPersonality == null || item.points > playerData.CurrentPersonality.points)
                    {
                        string previous = playerData.CurrentPersonality?.name ?? "特になし";
                        playerData.CurrentPersonality = item;
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
        
        private void UpdateUI()
        {
            gachaView.UpdateMoneyDisplay(playerData.Money);
            gachaView.UpdateStatusDisplay(playerData);
            gachaView.UpdateOwnedItemsDisplay(playerData);
        }
        
        // 他のコントローラーからプレイヤーデータにアクセスするためのプロパティ
        public PlayerData PlayerData => playerData;
    }
}
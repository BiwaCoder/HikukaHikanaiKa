using UnityEngine;
using UnityEngine.UI;
using HikukaHikanaika.Models;

namespace HikukaHikanaika.Views
{
    public class GachaView : MonoBehaviour
    {
        [Header("UI References")]
        public Button beautyGachaButton;
        public Button familyWealthGachaButton;
        public Button personalityGachaButton;
        public Text resultText;
        public Text moneyText;
        public Text statusText;
        public Text ownedItemsText;
        
        public void UpdateMoneyDisplay(int money)
        {
            if (moneyText != null)
            {
                moneyText.text = $"💰 所持金：{money:N0} 円";
            }
        }
        
        public void UpdateStatusDisplay(PlayerData player)
        {
            if (statusText != null)
            {
                string outfitInfo = player.CurrentOutfit != null 
                    ? $"👗 装備: {player.CurrentOutfit.name} (美容{player.CurrentOutfit.points})" 
                    : "👗 装備: 何も着ていない (美容 0)";
                    
                string familyInfo = player.CurrentFamilyWealth != null 
                    ? $"🏠 家柄: {player.CurrentFamilyWealth.name} (ポイント{player.CurrentFamilyWealth.points})" 
                    : "🏠 家柄: 特になし (ポイント 0)";
                    
                string personalityInfo = player.CurrentPersonality != null 
                    ? $"😊 性格: {player.CurrentPersonality.name} (ポイント{player.CurrentPersonality.points})" 
                    : "😊 性格: 特になし (ポイント 0)";
                
                statusText.text = $"📊 プレイヤーステータス\n{outfitInfo}\n{familyInfo}\n{personalityInfo}\n\n🎆 合計ポイント: {player.GetTotalPoints()}";
            }
        }
        
        public void UpdateOwnedItemsDisplay(PlayerData player)
        {
            if (ownedItemsText != null)
            {
                string beautyInfo = player.CurrentOutfit != null 
                    ? $"👗 服: {player.CurrentOutfit.name} ({player.CurrentOutfit.points})" 
                    : "👗 服: 何も着ていない";
                    
                string familyInfo = player.CurrentFamilyWealth != null 
                    ? $"🏠 家柄: {player.CurrentFamilyWealth.name} ({player.CurrentFamilyWealth.points})" 
                    : "🏠 家柄: 特になし";
                    
                string personalityInfo = player.CurrentPersonality != null 
                    ? $"😊 性格: {player.CurrentPersonality.name} ({player.CurrentPersonality.points})" 
                    : "😊 性格: 特になし";
                
                ownedItemsText.text = $"📋 現在の設定\n{beautyInfo}\n{familyInfo}\n{personalityInfo}";
            }
        }
        
        public void ShowGachaResult(string message)
        {
            if (resultText != null)
            {
                resultText.text = message;
            }
        }
        
        public void ShowInsufficientFunds()
        {
            if (resultText != null)
            {
                resultText.text = "💸 お金が足りません……";
            }
        }
    }
}
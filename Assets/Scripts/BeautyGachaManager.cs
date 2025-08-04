using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeautyGachaManager : MonoBehaviour
{
    [Serializable]
    public class GachaEntry
    {
        public string name;
        public float probability;
        public int beautyPoints;
    }

    [Header("UI 参照")]
    public Button beautyGachaButton;
    public Button familyWealthGachaButton;
    public Button personalityGachaButton;
    public Text resultText;
    public Text moneyText;

    [Header("ガチャテーブル")]
    private List<GachaEntry> beautyTable = new List<GachaEntry>()
    {
        new GachaEntry { name = "オートクチュールのドレス", probability = 10f, beautyPoints = 150 },  // 最強
        new GachaEntry { name = "制服しか勝たん", probability = 20f, beautyPoints = 80 },    // 一段階落ち
        new GachaEntry { name = "量産型ガーリー", probability = 20f, beautyPoints = 35 },     // あまり強くない
        new GachaEntry { name = "清潔感あるけど量販感", probability = 30f, beautyPoints = 15 },  // 気休め
        new GachaEntry { name = "サイズ合ってない", probability = 20f, beautyPoints = 3 },     // ほぼ変わらない
    };

    public List<GachaEntry> familyWealthTable = new List<GachaEntry>()
    {
        new GachaEntry { name = "一族で日本史に出てくる", probability = 10f, beautyPoints = 200 },
        new GachaEntry { name = "親が社長", probability = 20f, beautyPoints = 120 },
        new GachaEntry { name = "TVでよく見る", probability = 20f, beautyPoints = 80 },
        new GachaEntry { name = "上流階級", probability = 30f, beautyPoints = 40 },
        new GachaEntry { name = "一般席民", probability = 20f, beautyPoints = 10 },
    };

    public List<GachaEntry> personalityTable = new List<GachaEntry>()
    {
        new GachaEntry { name = "天性の人たらし", probability = 10f, beautyPoints = 180 },
        new GachaEntry { name = "ツンデレの黄金比", probability = 20f, beautyPoints = 100 },
        new GachaEntry { name = "笑顔が多すぎて怖しい", probability = 20f, beautyPoints = 60 },
        new GachaEntry { name = "無難すぎて覚えてもらえない", probability = 30f, beautyPoints = 25 },
        new GachaEntry { name = "第一印象が眠そう", probability = 20f, beautyPoints = 5 },
    };

    [Header("資金設定")]
    public int initialMoney = 300000000; // 3億
    public int beautyCostPerPull = 30000000;   // 3000万
    public int familyWealthCostPerPull = 60000000; // 6000万
    public int personalityCostPerPull = 20000000;  // 2000万

    private int currentMoney;

    [Header("プレイヤーステータス")]
    public Text statusText;
    public Text ownedItemsText;

    [Header("所持アイテム")]
    private HashSet<string> ownedBeautyItems = new HashSet<string>();
    private HashSet<string> ownedFamilyWealthItems = new HashSet<string>();
    private HashSet<string> ownedPersonalityItems = new HashSet<string>();
    
    private GachaEntry currentOutfit = null; // 現在装備中の服
    private GachaEntry currentFamilyWealth = null; // 現在の家柄
    private GachaEntry currentPersonality = null; // 現在の性格
    
    // パブリックアクセサー
    public GachaEntry CurrentOutfit => currentOutfit;
    public GachaEntry CurrentFamilyWealth => currentFamilyWealth;
    public GachaEntry CurrentPersonality => currentPersonality;

    void Start()
    {
        currentMoney = initialMoney;
        UpdateMoneyDisplay();
        UpdateStatusDisplay();
        UpdateOwnedItemsDisplay();

        if (beautyGachaButton != null)
            beautyGachaButton.onClick.AddListener(() => OnGachaButtonClicked(GachaType.Beauty));
        if (familyWealthGachaButton != null)
            familyWealthGachaButton.onClick.AddListener(() => OnGachaButtonClicked(GachaType.FamilyWealth));
        if (personalityGachaButton != null)
            personalityGachaButton.onClick.AddListener(() => OnGachaButtonClicked(GachaType.Personality));
    }

    public enum GachaType
    {
        Beauty,
        FamilyWealth,
        Personality
    }

    void OnGachaButtonClicked(GachaType gachaType)
    {
        int cost = GetGachaCost(gachaType);
        if (currentMoney < cost)
        {
            resultText.text = "💸 お金が足りません……";
            return;
        }

        currentMoney -= cost;
        UpdateMoneyDisplay();

        GachaEntry pulledItem = PullGachaWithStats(gachaType);
        string resultMessage = AddItemToCollection(pulledItem, gachaType);
        
        resultText.text = resultMessage;
        UpdateStatusDisplay();
        UpdateOwnedItemsDisplay();
    }

    int GetGachaCost(GachaType gachaType)
    {
        switch (gachaType)
        {
            case GachaType.Beauty: return beautyCostPerPull;
            case GachaType.FamilyWealth: return familyWealthCostPerPull;
            case GachaType.Personality: return personalityCostPerPull;
            default: return beautyCostPerPull;
        }
    }

    void UpdateMoneyDisplay()
    {
        moneyText.text = $"💰 所持金：{currentMoney:N0} 円";
    }

    public GachaEntry PullGachaWithStats(GachaType gachaType = GachaType.Beauty)
    {
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks & 0x0000FFFF);
        float roll = UnityEngine.Random.Range(0f, 100f);
        float cumulative = 0f;

        List<GachaEntry> table = GetGachaTable(gachaType);
        foreach (var entry in table)
        {
            cumulative += entry.probability;
            if (roll <= cumulative)
            {
                return entry;
            }
        }

        return table[table.Count - 1]; // フォールバック
    }

    List<GachaEntry> GetGachaTable(GachaType gachaType)
    {
        switch (gachaType)
        {
            case GachaType.Beauty: return beautyTable;
            case GachaType.FamilyWealth: return familyWealthTable;
            case GachaType.Personality: return personalityTable;
            default: return beautyTable;
        }
    }

    public string PullGacha() // 後方互換性のため残す
    {
        return PullGachaWithStats().name;
    }

    string AddItemToCollection(GachaEntry item, GachaType gachaType)
    {
        HashSet<string> ownedItems = GetOwnedItemsSet(gachaType);
        
        if (ownedItems.Contains(item.name))
        {
            return $"🎴 結果: {item.name}\n🔄 既に所持しています。";
        }
        
        ownedItems.Add(item.name);
        
        return UpdateCurrentEquipment(item, gachaType);
    }

    HashSet<string> GetOwnedItemsSet(GachaType gachaType)
    {
        switch (gachaType)
        {
            case GachaType.Beauty: return ownedBeautyItems;
            case GachaType.FamilyWealth: return ownedFamilyWealthItems;
            case GachaType.Personality: return ownedPersonalityItems;
            default: return ownedBeautyItems;
        }
    }

    string UpdateCurrentEquipment(GachaEntry item, GachaType gachaType)
    {
        switch (gachaType)
        {
            case GachaType.Beauty:
                if (currentOutfit == null || item.beautyPoints > currentOutfit.beautyPoints)
                {
                    string previousOutfit = currentOutfit?.name ?? "何も着ていない";
                    currentOutfit = item;
                    return $"🎴 結果: {item.name}\n👗 新しい服を装備！\n✨ 美容: {item.beautyPoints} (前: {previousOutfit})";
                }
                else
                {
                    return $"🎴 結果: {item.name}\n📦 ワードローブに追加。\n現在の装備の方が良いので着替えません。";
                }
            
            case GachaType.FamilyWealth:
                if (currentFamilyWealth == null || item.beautyPoints > currentFamilyWealth.beautyPoints)
                {
                    string previous = currentFamilyWealth?.name ?? "特になし";
                    currentFamilyWealth = item;
                    return $"🎴 結果: {item.name}\n🏠 新しい家柄を設定！\n💰 家柄: {item.beautyPoints} (前: {previous})";
                }
                else
                {
                    return $"🎴 結果: {item.name}\n💼 プロフィールに追加。\n現在の家柄の方が良いので変更しません。";
                }
            
            case GachaType.Personality:
                if (currentPersonality == null || item.beautyPoints > currentPersonality.beautyPoints)
                {
                    string previous = currentPersonality?.name ?? "特になし";
                    currentPersonality = item;
                    return $"🎴 結果: {item.name}\n😊 新しい性格を設定！\n🎆 性格: {item.beautyPoints} (前: {previous})";
                }
                else
                {
                    return $"🎴 結果: {item.name}\n📝 プロフィールに追加。\n現在の性格の方が良いので変更しません。";
                }
            
            default:
                return $"🎴 結果: {item.name}";
        }
    }

    void UpdateStatusDisplay()
    {
        if (statusText != null)
        {
            string outfitInfo = currentOutfit != null ? $"👗 装備: {currentOutfit.name} (美容{currentOutfit.beautyPoints})" : "👗 装備: 何も着ていない (美容 0)";
            string familyInfo = currentFamilyWealth != null ? $"🏠 家柄: {currentFamilyWealth.name} (ポイント{currentFamilyWealth.beautyPoints})" : "🏠 家柄: 特になし (ポイント 0)";
            string personalityInfo = currentPersonality != null ? $"😊 性格: {currentPersonality.name} (ポイント{currentPersonality.beautyPoints})" : "😊 性格: 特になし (ポイント 0)";
            
            int totalPoints = (currentOutfit?.beautyPoints ?? 0) + (currentFamilyWealth?.beautyPoints ?? 0) + (currentPersonality?.beautyPoints ?? 0);
            
            statusText.text = $"📊 プレイヤーステータス\n{outfitInfo}\n{familyInfo}\n{personalityInfo}\n\n🎆 合計ポイント: {totalPoints}";
        }
    }

    void UpdateOwnedItemsDisplay()
    {
        if (ownedItemsText != null)
        {
            string beautyInfo = currentOutfit != null ? $"👗 服: {currentOutfit.name} ({currentOutfit.beautyPoints})" : "👗 服: 何も着ていない";
            string familyInfo = currentFamilyWealth != null ? $"🏠 家柄: {currentFamilyWealth.name} ({currentFamilyWealth.beautyPoints})" : "🏠 家柄: 特になし";
            string personalityInfo = currentPersonality != null ? $"😊 性格: {currentPersonality.name} ({currentPersonality.beautyPoints})" : "😊 性格: 特になし";
            
            ownedItemsText.text = $"📋 現在の設定\n{beautyInfo}\n{familyInfo}\n{personalityInfo}";
        }
    }
}

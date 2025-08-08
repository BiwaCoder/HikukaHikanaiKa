using UnityEngine;
using UnityEngine.UI;
using HikukaHikanaika.Logic;
using HikukaHikanaika.Models;

public class GachaUIController : MonoBehaviour
{
    public Text descriptionText;
    public Text priceText;
    public Button gachaButton;
    public Button nextButton;
    public Text resultText;
    public Text moneyText;
    public Text statusText;

    public HikukaHikanaika.Models.GachaType currentGachaType = HikukaHikanaika.Models.GachaType.FamilyWealth;

    private string[] gachaDescriptions = {
        "💄 美貌を手に入れるガチャ 🌹\n\n絶世の美女か、平凡な容姿か──\n\n運命の分かれ道はここに！",
        "🏰 家柄を手に入れるガチャ 🎲\n\n歴史に残る一族か、ただの庶民か──\n\nすべては引きどころ次第！",
        "✨ 性格を手に入れるガチャ 💫\n\n魅力的な人格か、個性的な性格か──\n\nあなたの魅力が決まる瞬間！"
    };

    private int currentIndex = 0;
    private GachaLogic gachaLogic;
    private bool isProcessing = false;

    void Start()
    {
        gachaLogic = GachaLogic.Instance;
        ShowCurrentGacha();
        UpdateMoneyDisplay();
        UpdateStatusDisplay();
        
        // 既存のリスナーをクリアしてから追加（重複防止）
        gachaButton.onClick.RemoveAllListeners();
        gachaButton.onClick.AddListener(OnGachaClick);
        
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(OnNextClick);
    }

    void ShowCurrentGacha()
    {
        if (descriptionText != null)
        {
            descriptionText.text = gachaDescriptions[(int)currentGachaType];
        }

        if (priceText != null)
        {
            switch (currentGachaType)
            {
                case HikukaHikanaika.Models.GachaType.Beauty:
                    priceText.text = "価格: 3000万円";
                    break;
                case HikukaHikanaika.Models.GachaType.FamilyWealth:
                    priceText.text = "価格: 6000万円";
                    break;
                case HikukaHikanaika.Models.GachaType.Personality:
                    priceText.text = "価格: 2000万円";
                    break;
            }
        }
    }

    public void OnGachaClick()
    {
        if (isProcessing)
        {
            Debug.Log("ガチャ処理中のため、連打を無視します");
            return;
        }
        
        isProcessing = true;
        gachaButton.interactable = false;
        
        try
        {
            string result = gachaLogic.PerformGacha(currentGachaType);

            if (resultText != null)
            {
                resultText.text = result;
            }

            UpdateMoneyDisplay();
            UpdateStatusDisplay();
        }
        finally
        {
            isProcessing = false;
            gachaButton.interactable = true;
        }
    }

    //テキストの変更
    public void OnNextClick()
    {
        currentGachaType = (HikukaHikanaika.Models.GachaType)(((int)currentGachaType + 1) % 3);
        ShowCurrentGacha();
    }

    private void UpdateMoneyDisplay()
    {
        if (moneyText != null && gachaLogic != null)
        {
            moneyText.text = "所持金: " + gachaLogic.PlayerData.Money.ToString("N0") + "円";
        }
    }

    private void UpdateStatusDisplay()
    {
        if (statusText != null && gachaLogic != null)
        {
            var playerData = gachaLogic.PlayerData;
            string status = "💄 美貌: " + (playerData.CurrentOutfit?.name ?? "なし") +
                           (playerData.CurrentOutfit != null ? " (" + playerData.CurrentOutfit.points + ")" : "") + "\n" +
                           "🏰 家柄: " + (playerData.CurrentFamilyWealth?.name ?? "なし") +
                           (playerData.CurrentFamilyWealth != null ? " (" + playerData.CurrentFamilyWealth.points + ")" : "") + "\n" +
                           "✨ 性格: " + (playerData.CurrentPersonality?.name ?? "なし") +
                           (playerData.CurrentPersonality != null ? " (" + playerData.CurrentPersonality.points + ")" : "");

            statusText.text = status;
        }
    }
    
    public void OnClickNextScene()
    {
        // 次のシーンに遷移する処理
        UnityEngine.SceneManagement.SceneManager.LoadScene("BeforeGacha");
    }
}

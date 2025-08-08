using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
        "💄 美貌ガチャ 🌹\n\n寿命2年を消費して、あなたの外見を決める運命の瞬間...\n\n美しさは時に人生を変える。",
        "🏰 家柄ガチャ 🎲\n\n寿命2年を消費して、あなたの生まれを決める宿命のルーレット...\n\n血筋が示す、人生の道筋。",
        "✨ 性格ガチャ 💫\n\n寿命2年を消費して、あなたの内面を形作る魂の選択...\n\n人格こそが、真の運命を決める。"
    };

    private int currentIndex = 0;
    private GachaLogic gachaLogic;
    private bool isProcessing = false;
    private bool fromEventScene = false;

    void Start()
    {
        gachaLogic = GachaLogic.Instance;
        
        // イベントシーンから来たかどうかをチェック
        fromEventScene = PlayerPrefs.GetInt("FromEventScene", 0) == 1;
        if (fromEventScene)
        {
            PlayerPrefs.DeleteKey("FromEventScene");
        }
        
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
            priceText.text = "コスト: 寿命2年";
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
            
            // ガチャ後の処理
            HandlePostGachaFlow();
        }
        finally
        {
            isProcessing = false;
            gachaButton.interactable = true;
        }
    }

    void HandlePostGachaFlow()
    {
        var playerData = PlayerData.Instance;
        
        // ゲームオーバーチェック
        if (playerData.IsGameOver())
        {
            // ゲームオーバーシーンへ遷移
            StartCoroutine(TransitionToGameOver());
            return;
        }
        
        // イベントシーンから来た場合は、イベントシーンに戻る
        if (fromEventScene)
        {
            StartCoroutine(TransitionToEventScene());
            return;
        }
        
        // 通常のガチャ進行：各ガチャ後にイベントへ移行
        StartCoroutine(TransitionToEventScene());
    }
    
    
    System.Collections.IEnumerator TransitionToEventScene()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("LifeStageEventScene");
    }
    
    System.Collections.IEnumerator TransitionToGameOver()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("GameOver");
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
            var playerData = gachaLogic.PlayerData;
            moneyText.text = $"寿命: {playerData.RemainingLifespan}年 | {playerData.GetLifeStage()}";
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

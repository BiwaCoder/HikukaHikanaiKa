using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using HikukaHikanaika.Logic;
using HikukaHikanaika.Models;

public class GachaUIController : MonoBehaviour
{
    [Header("UI References")]
    public Text descriptionText;
    public Text priceText;
    public Button gachaButton;
    public Button nextGachaButton; // ガチャ切り替えボタン
    public Button toNextEventButton; // 次のイベントへ進むボタン
    public Text resultText;
    public Text statusText;
    public Text lifeStatusText;

    private GachaType[] allGachaTypes = 
    {
        GachaType.Beauty,
        GachaType.FamilyWealth,
        GachaType.Personality,
        GachaType.Luck,
        GachaType.Concentration,
        GachaType.Kindness
    };

    private string[] gachaDescriptions = 
    {
        "💄 美貌ガチャ 🌹\n\n寿命2年を消費して、あなたの外見を決める運命の瞬間...\n\n美しさは時に人生を変える。",
        "🏰 家柄ガチャ 🎲\n\n寿命2年を消費して、あなたの生まれを決める宿命のルーレット...\n\n血筋が示す、人生の道筋。",
        "✨ 性格ガチャ 💫\n\n寿命2年を消費して、あなたの内面を形作る魂の選択...\n\n人格こそが、真の運命を決める。",
        "🍀 運ガチャ 🌟\n\n寿命2年を消費して、幸運を引き寄せる...\n\n見えざる力が、あなたの道を照らす。",
        "🎯 集中力ガチャ 🧠\n\n寿命2年を消費して、精神を研ぎ澄ます...\n\n一点を見つめる力が、未来を切り開く。",
        "💖 優しさガチャ 🥰\n\n寿命2年を消費して、心を温める...\n\n愛する心が、世界を優しく包む。"
    };

    private int currentGachaIndex = 0;
    private GachaLogic gachaLogic;
    private bool isProcessing = false;

    void Start()
    {
        gachaLogic = GachaLogic.Instance;
        
        UpdateUI();
        
        gachaButton.onClick.AddListener(OnGachaClick);
        nextGachaButton.onClick.AddListener(OnNextGachaClick);
        toNextEventButton.onClick.AddListener(OnClickNextScene);
    }

    void UpdateUI()
    {
        descriptionText.text = gachaDescriptions[currentGachaIndex];
        priceText.text = "コスト: 寿命2年";
        UpdateStatusDisplay();
    }

    public void OnGachaClick()
    {
        if (isProcessing) return;
        
        isProcessing = true;
        gachaButton.interactable = false;
        
        try
        {
            GachaType selectedGacha = allGachaTypes[currentGachaIndex];
            string result = gachaLogic.PerformGacha(selectedGacha);
            if (resultText != null) resultText.text = result;

            UpdateStatusDisplay();
            
            if (PlayerData.Instance.IsGameOver())
            {
                StartCoroutine(TransitionToGameOver());
            }
        }
        finally
        {
            isProcessing = false;
            gachaButton.interactable = true;
        }
    }

    public void OnNextGachaClick()
    {
        currentGachaIndex = (currentGachaIndex + 1) % allGachaTypes.Length;
        UpdateUI();
    }

    private void UpdateStatusDisplay()
    {
        if (gachaLogic == null) return;

        var playerData = gachaLogic.PlayerData;
        
        if (lifeStatusText != null)
        {
            lifeStatusText.text = $"寿命: {playerData.RemainingLifespan}年 | {playerData.GetLifeStage()}";
        }

        if (statusText != null)
        {
            var allStatus = playerData.GetAllStatus();
            statusText.text = $"💄 美貌: {allStatus["外見"]} | 🏠 家柄: {allStatus["外見"]} | ✨ 性格: {allStatus["外見"]}\n" +
                              $"🍀 運: {allStatus["運"]} | 🎯 集中力: {allStatus["集中力"]} | 💖 優しさ: {allStatus["優しさ"]}";
        }
    }
    
    System.Collections.IEnumerator TransitionToGameOver()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("GameOver");
    }

    public void OnClickNextScene()
    {
        SceneManager.LoadScene("LifeStageEventScene");
    }
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using HikukaHikanaika.Logic;
using HikukaHikanaika.Models;
using System.Collections.Generic;

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

    private List<GachaType> availableGachaTypes = new List<GachaType>();
    private Dictionary<GachaType, string> gachaDescriptions = new Dictionary<GachaType, string>
    {
        [GachaType.Beauty] = "💄 美貌ガチャ 🌹\n\n魂片2片を消費して、あなたの外見を決める運命の瞬間...\n\n美しさは時に人生を変える。",
        [GachaType.FamilyWealth] = "🏰 家柄ガチャ 🎲\n\n魂片2片を消費して、あなたの生まれを決める宿命のルーレット...\n\n血筋が示す、人生の道筋。",
        [GachaType.Personality] = "✨ 性格ガチャ 💫\n\n魂片2片を消費して、あなたの内面を形作る魂の選択...\n\n人格こそが、真の運命を決める。",
        [GachaType.Luck] = "🍀 運ガチャ 🌟\n\n魂片2片を消費して、幸運を引き寄せる...\n\n見えざる力が、あなたの道を照らす。",
        [GachaType.Concentration] = "🎯 集中力ガチャ 🧠\n\n魂片2片を消費して、精神を研ぎ澄ます...\n\n一点を見つめる力が、未来を切り開く。",
        [GachaType.Kindness] = "💖 優しさガチャ 🥰\n\n魂片2片を消費して、心を温める...\n\n愛する心が、世界を優しく包む。",
        
        // ライフステージ限定ガチャ
        [GachaType.Childhood] = "🧸 童心ガチャ 🎪\n\n魂片3片を消費して、永遠の純粋さを手に入れる...\n\n子供の心は、全てを可能にする。",
        [GachaType.Youth] = "🌟 青春ガチャ ⚡\n\n魂片3片を消費して、情熱の炎を燃やす...\n\n若さこそが最大の武器。",
        [GachaType.Career] = "💼 キャリアガチャ 📈\n\n魂片3片を消費して、プロの風格を身につける...\n\n社会で勝ち抜く力を。",
        [GachaType.Mature] = "🍷 円熟ガチャ 🎭\n\n魂片3片を消費して、人生経験の深みを得る...\n\n年齢を重ねた者だけの特権。",
        [GachaType.Senior] = "🌸 長老ガチャ 🔮\n\n魂片3片を消費して、老獪な知恵を授かる...\n\n長い人生の集大成。",
        
        // 特別ガチャ
        [GachaType.Legendary] = "👑 伝説ガチャ ⭐\n\n魂片5片を消費して、運命を決定づける...\n\n【一回限り】大成功の褒美として、特別に。\n\n一度しか引けない貴重なガチャです。",
        [GachaType.Redemption] = "🔥 逆転ガチャ 💪\n\n魂片4片を消費して、失敗を力に変える...\n\n挫折こそが真の教師。",
        [GachaType.Love] = "💕 恋愛ガチャ 💒\n\n魂片4片を消費して、愛の力を得る...\n\n恋する心が世界を変える。",
        [GachaType.Reversal] = "🎯 人生逆転ガチャ ⚡\n\n魂片10片を消費して、全てを賭ける...\n\n【警告】失敗時は更に魂片15片を失います\n\n30歳という人生の分岐点。これまでがうまくいかなくても、ここから全てを変えられる。最後の大勝負！",
        
        // 使えない（超損）ガチャ
        [GachaType.Cursed] = "💀 呪いガチャ 🌑\n\n魂片1片で引ける格安ガチャ...\n\n【危険】全てのステータスが下がる可能性があります\n\n安物買いの銭失い。絶対に引かないでください。",
        [GachaType.Academic] = "📚 学術ガチャ 🎓\n\n魂片20片の超高コストガチャ\n\n集中力のみ大幅上昇\n\n効率が悪すぎます。普通のガチャを引いた方がマシです。",
        [GachaType.Social] = "🎭 社交ガチャ 👑\n\n魂片25片の超高コストガチャ\n\n優しさのみ大幅上昇\n\n コスパが最悪です。他のガチャを検討してください。",
        [GachaType.Gambler] = "🎰 ギャンブラーガチャ 💰\n\n魂片30片の最高コストガチャ\n\n運のみ大幅上昇\n\n資源の無駄遣いです。引く意味がありません。"
    };

    private int currentGachaIndex = 0;
    private GachaLogic gachaLogic;
    private GachaModel gachaModel;
    private bool isProcessing = false;

    void Start()
    {
        gachaLogic = GachaLogic.Instance;
        gachaModel = new GachaModel();
        
        RefreshAvailableGachas();
        UpdateUI();
        
        gachaButton.onClick.AddListener(OnGachaClick);
        nextGachaButton.onClick.AddListener(OnNextGachaClick);
        toNextEventButton.onClick.AddListener(OnClickNextScene);
    }

    void RefreshAvailableGachas()
    {
        availableGachaTypes.Clear();
        var playerData = PlayerData.Instance;
        
        // 全ガチャタイプをチェック
        foreach (GachaType gachaType in System.Enum.GetValues(typeof(GachaType)))
        {
            if (gachaLogic.CanPerformGacha(gachaType) || playerData.IsGachaUnlocked(gachaType))
            {
                availableGachaTypes.Add(gachaType);
            }
        }
        
        // インデックスを修正
        if (currentGachaIndex >= availableGachaTypes.Count)
        {
            currentGachaIndex = 0;
        }
    }

    void UpdateUI()
    {
        if (availableGachaTypes.Count == 0)
        {
            descriptionText.text = "利用可能なガチャがありません";
            priceText.text = "";
            gachaButton.interactable = false;
            return;
        }
        
        GachaType currentGacha = availableGachaTypes[currentGachaIndex];
        descriptionText.text = gachaDescriptions[currentGacha];
        
        // コスト表示を動的に更新
        int cost = gachaModel.GetGachaCost(currentGacha);
        priceText.text = $"コスト: 魂片{cost}片";
        
        // ボタンの利用可能性をチェック
        gachaButton.interactable = gachaLogic.CanPerformGacha(currentGacha);
        
        UpdateStatusDisplay();
    }

    public void OnGachaClick()
    {
        if (isProcessing || availableGachaTypes.Count == 0) return;
        
        isProcessing = true;
        gachaButton.interactable = false;
        
        try
        {
            GachaType selectedGacha = availableGachaTypes[currentGachaIndex];
            string result = gachaLogic.PerformGacha(selectedGacha);
            if (resultText != null) resultText.text = result;

            // ガチャ実行後にリストを更新（新しいガチャが解放されたり、制限に引っかかったりする可能性があるため）
            RefreshAvailableGachas();
            UpdateStatusDisplay();
            
            if (PlayerData.Instance.IsGameOver())
            {
                StartCoroutine(TransitionToGameOver());
            }
        }
        finally
        {
            isProcessing = false;
            gachaButton.interactable = gachaLogic.CanPerformGacha(availableGachaTypes[currentGachaIndex]);
        }
    }

    public void OnNextGachaClick()
    {
        if (availableGachaTypes.Count == 0) return;
        
        currentGachaIndex = (currentGachaIndex + 1) % availableGachaTypes.Count;
        UpdateUI();
    }

    private void UpdateStatusDisplay()
    {
        if (gachaLogic == null) return;

        var playerData = gachaLogic.PlayerData;
        
        if (lifeStatusText != null)
        {
            lifeStatusText.text = $"魂片: {playerData.RemainingTenmei}片 \n {playerData.GetLifeStage()}";
        }

        if (statusText != null)
        {
            statusText.text = $"💄 美貌: {(playerData.CurrentOutfit != null ? $"{playerData.CurrentOutfit.name} ({playerData.CurrentOutfit.points})" : "なし (0)")}\n" +
                              $"🏠 家柄: {(playerData.CurrentFamilyWealth != null ? $"{playerData.CurrentFamilyWealth.name} ({playerData.CurrentFamilyWealth.points})" : "なし (0)")}\n" +
                              $"✨ 性格: {(playerData.CurrentPersonality != null ? $"{playerData.CurrentPersonality.name} ({playerData.CurrentPersonality.points})" : "なし (0)")}\n" +
                              $"🍀 運: {playerData.LastLuckItemName} ({playerData.Luck})\n" +
                              $"🎯 集中力: {playerData.LastConcentrationItemName} ({playerData.Concentration})\n" +
                              $"💖 優しさ: {playerData.LastKindnessItemName} ({playerData.Kindness})";
        }
    }
    
    System.Collections.IEnumerator TransitionToGameOver()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("BadEndingScene");
    }

    public void OnClickNextScene()
    {
        SceneManager.LoadScene("LifeStageEventScene");
    }
    
    // シーンに戻ってきた時などに利用可能ガチャを更新する用
    void OnEnable()
    {
        if (gachaLogic != null)
        {
            RefreshAvailableGachas();
            UpdateUI();
        }
    }
}
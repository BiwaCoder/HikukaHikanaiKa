using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using HikukaHikanaika.Models;
using System.Linq;

/*
 * LifeStageEventScene セットアップガイド:
 * 
 * Unity エディターで新しいシーンを作成し、"LifeStageEventScene" として保存してください。
 * 以下のUI要素を配置し、このスクリプトにアサインしてください:
 * 
 * 必要なUI要素:
 * - Canvas (Screen Space - Overlay)
 *   - titleText: Text - イベントタイトル表示用
 *   - descriptionText: Text - イベント説明表示用  
 *   - statusText: Text - プレイヤーステータス表示用
 *   - challengeButton: Button - "現在のステータスで挑戦"
 *   - gachaButton: Button - "ガチャを引いてから挑戦"
 *   - challengeButtonText: Text - challengeButtonの子テキスト
 *   - gachaButtonText: Text - gachaButtonの子テキスト
 *   - resultText: Text - 結果表示用（初期状態で非表示）
 * 
 * レイアウト推奨:
 * - 背景は暗めの色
 * - タイトルは大きく上部に配置
 * - 説明文は中央に配置
 * - ステータス情報は左上に小さく配置
 * - ボタンは下部に横並びで配置
 * - 結果テキストは中央下部に配置
 */

public class LifeStageEventController : MonoBehaviour
{
    [Header("UI References")]
    public Text titleText;
    public Text descriptionText;
    public Text statusText;
    public Button challengeButton;
    public Button gachaButton;
    public Text challengeButtonText;
    public Text gachaButtonText;
    public Text resultText;
    
    [Header("Settings")]
    public float textSpeed = 0.05f;
    
    private PlayerData playerData;
    private LifeStageEvent currentEvent;
    private bool eventCompleted = false;
    
    void Start()
    {
        playerData = PlayerData.Instance;
        LoadCurrentEvent();
        SetupUI();
    }
    
    void LoadCurrentEvent()
    {
        // 神の道ルートチェック（上位5%プレイヤー専用）
        if (playerData.CanAccessDivinePath())
        {
            currentEvent = CheckForDivinePathEvent();
        }
        
        // 神の道イベントがなければ高基準限定イベントをチェック
        if (currentEvent == null)
        {
            currentEvent = CheckForHighStatEvent();
        }
        
        if (currentEvent == null)
        {
            // 通常イベントを取得
            var allEvents = LifeStageEventData.GetAllEvents();
            currentEvent = allEvents.FirstOrDefault(e => e.lifeCycle == playerData.LifeCycle);
        }
        
        if (currentEvent == null)
        {
            // イベントが見つからない場合は次のライフサイクルへ
            Debug.LogWarning($"No event found for life cycle {playerData.LifeCycle}");
            AdvanceToNextStage();
            return;
        }
    }
    
    LifeStageEvent CheckForHighStatEvent()
    {
        var highStatEvents = LifeStageEventData.GetHighStatEvents();
        var candidateEvent = highStatEvents.FirstOrDefault(e => e.lifeCycle == playerData.LifeCycle);
        
        if (candidateEvent == null) return null;
        
        // 特別な判定: 完璧超人イベント（全ステータス合計で判定）
        if (candidateEvent.eventTitle == "👑 完璧超人への招待")
        {
            int totalStats = playerData.Luck + playerData.Concentration + playerData.Kindness + 
                           (playerData.CurrentOutfit?.points ?? 0) + 
                           (playerData.CurrentFamilyWealth?.points ?? 0) + 
                           (playerData.CurrentPersonality?.points ?? 0);
            
            if (totalStats >= candidateEvent.difficultyThreshold)
            {
                return candidateEvent;
            }
        }
        else
        {
            // 通常の高ステータス判定
            int requiredStat = GetRequiredStatusValueForEvent(candidateEvent);
            if (requiredStat >= candidateEvent.difficultyThreshold)
            {
                return candidateEvent;
            }
        }
        
        return null;
    }
    
    LifeStageEvent CheckForDivinePathEvent()
    {
        var divineEvents = LifeStageEventData.GetDivinePathEvents();
        var candidateEvent = divineEvents.FirstOrDefault(e => e.lifeCycle == playerData.LifeCycle);
        
        if (candidateEvent == null) return null;
        
        // 最終審判の特別判定（全ステータス合計）
        if (candidateEvent.eventTitle.Contains("最終審判"))
        {
            int totalStats = playerData.Luck + playerData.Concentration + playerData.Kindness + 
                           (playerData.CurrentOutfit?.points ?? 0) + 
                           (playerData.CurrentFamilyWealth?.points ?? 0) + 
                           (playerData.CurrentPersonality?.points ?? 0);
            
            if (totalStats >= 1000) // 最終審判の基準
            {
                return candidateEvent;
            }
        }
        else
        {
            // 通常の神の道審判
            int requiredStat = GetRequiredStatusValueForEvent(candidateEvent);
            if (requiredStat >= candidateEvent.difficultyThreshold)
            {
                return candidateEvent;
            }
        }
        
        return null;
    }
    
    int GetRequiredStatusValueForEvent(LifeStageEvent targetEvent)
    {
        switch (targetEvent.requiredStatus)
        {
            case BattleStatusType.Family:
                return playerData.CurrentFamilyWealth?.points ?? 0;
            case BattleStatusType.Appearance:
                return playerData.CurrentOutfit?.points ?? 0;
            case BattleStatusType.Personality:
                return playerData.CurrentPersonality?.points ?? 0;
            case BattleStatusType.Luck:
                return playerData.Luck;
            case BattleStatusType.Concentration:
                return playerData.Concentration;
            case BattleStatusType.Kindness:
                return playerData.Kindness;
            default:
                return 0;
        }
    }
    
    void SetupUI()
    {
        if (currentEvent == null) return;
        
        // イベント情報表示
        if (titleText != null)
            titleText.text = currentEvent.eventTitle;
            
        if (descriptionText != null)
            descriptionText.text = currentEvent.eventDescription;
        
        UpdateStatusDisplay();
        
        // ボタン設定
        if (challengeButton != null)
        {
            challengeButton.onClick.AddListener(OnChallengeClicked);
            if (challengeButtonText != null)
                challengeButtonText.text = "現在のステータスで挑戦";
        }
        
        if (gachaButton != null)
        {
            gachaButton.onClick.AddListener(OnGachaClicked);
            if (gachaButtonText != null)
                gachaButtonText.text = "ガチャを引いてから挑戦 (-2年)";
                
            // 魂片不足の場合はボタン無効化
            gachaButton.interactable = playerData.CanAffordTenmei(2);
        }
        
        if (resultText != null)
            resultText.gameObject.SetActive(false);
    }
    
    void UpdateStatusDisplay()
    {
        if (statusText == null) return;
        
        int requiredStat = GetRequiredStatusValue();
        string statusName = GetStatusName(currentEvent.requiredStatus);
        
        statusText.text = $"現在の{statusName}: {requiredStat}pt\n" +
                         $"魂片: {playerData.RemainingTenmei}片 \n {playerData.GetLifeStage()}";
    }
    
    int GetRequiredStatusValue()
    {
        switch (currentEvent.requiredStatus)
        {
            case BattleStatusType.Family:
                return playerData.CurrentFamilyWealth?.points ?? 0;
            case BattleStatusType.Appearance:
                return playerData.CurrentOutfit?.points ?? 0;
            case BattleStatusType.Personality:
                return playerData.CurrentPersonality?.points ?? 0;
            case BattleStatusType.Luck:
                return playerData.Luck;
            case BattleStatusType.Concentration:
                return playerData.Concentration;
            case BattleStatusType.Kindness:
                return playerData.Kindness;
            default:
                return 0;
        }
    }
    
    string GetStatusName(BattleStatusType statusType)
    {
        switch (statusType)
        {
            case BattleStatusType.Family: return "家柄";
            case BattleStatusType.Appearance: return "容姿";
            case BattleStatusType.Personality: return "性格";
            case BattleStatusType.Luck: return "運";
            case BattleStatusType.Concentration: return "集中力";
            case BattleStatusType.Kindness: return "優しさ";
            default: return "不明";
        }
    }
    
    public void OnChallengeClicked()
    {
        if (eventCompleted) return; 
        
        StartCoroutine(ProcessChallenge());
    }
    
    public void OnGachaClicked()
    {
        if (eventCompleted) return; 
        
        if (!playerData.CanAffordTenmei(2))
        {
            ShowResult("⏳ 魂片が足りません！");
            return;
        }
        
        // ガチャシーンに遷移（イベントフラグを設定）
        PlayerPrefs.SetInt("FromEventScene", 1);
        PlayerPrefs.SetInt("CurrentEventCycle", playerData.LifeCycle);
        SceneManager.LoadScene("FamiryGachaScene");
    }
    
    IEnumerator ProcessChallenge()
    {
        eventCompleted = true;
        challengeButton.interactable = false;
        gachaButton.interactable = false;

        if (resultText != null)
            resultText.gameObject.SetActive(true);

        // 1. 結果を判定
        int playerStat = GetRequiredStatusValue();
        int threshold = currentEvent.difficultyThreshold;
        EventResult eventResult;

        if (playerStat >= threshold * 1.5f) {
            eventResult = currentEvent.greatSuccess;
        } else if (playerStat >= threshold) {
            eventResult = currentEvent.success;
        } else if (playerStat >= threshold * 0.5f) {
            eventResult = currentEvent.failure;
        } else {
            eventResult = currentEvent.greatFailure;
        }

        // 2. 魂片を変動させる
        playerData.RemainingTenmei += eventResult.tenmeiChange;

        // ★★★ イベント結果によるガチャ解放処理 ★★★
        GachaUnlockResult gachaUnlockResult = ConvertToGachaUnlockResult(eventResult, threshold, playerStat);
        string eventType = DetermineEventType(currentEvent.eventTitle);
        playerData.ProcessEventResult(gachaUnlockResult, eventType);

        // 3. 表示する最終的なメッセージを一度に組み立てる
        string statusInfluence = $"({GetStatusName(currentEvent.requiredStatus)}が影響しました)";
        string changeDescription = eventResult.tenmeiChange >= 0 ? "🎉" : "💔";
        string finalMessage = $"{eventResult.text}\n{statusInfluence}\n\n{changeDescription} {eventResult.description}";
        
        // 新しいガチャが解放された場合の通知追加
        string gachaNotification = GetGachaUnlockNotification();
        if (!string.IsNullOrEmpty(gachaNotification))
        {
            finalMessage += "\n\n" + gachaNotification;
        }

        // ★★★ 結果を履歴に記録 ★★★
        playerData.AddEventRecord(playerData.GetLifeStage(), currentEvent.eventTitle, eventResult.text);

        // 4. 最終的なメッセージを一度だけ表示する
        yield return StartCoroutine(TypeText(finalMessage));

        // ゲームオーバーチェック
        if (playerData.IsGameOver())
        {
            yield return StartCoroutine(ShowAngelGameOver());
            yield break;
        }

        yield return new WaitForSeconds(3f);
        AdvanceToNextStage();
    }
    
    IEnumerator TypeText(string text)
    {
        if (resultText == null) yield break;
        
        resultText.text = "";
        foreach (char c in text)
        {
            resultText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }
    
    void ShowResult(string message)
    {
        if (resultText != null)
        {
            resultText.gameObject.SetActive(true);
            resultText.text = message;
        }
    }
    
    // イベント結果をGachaUnlockResultに変換
    GachaUnlockResult ConvertToGachaUnlockResult(EventResult eventResult, int threshold, int playerStat)
    {
        if (playerStat >= threshold * 1.5f) {
            return GachaUnlockResult.GreatSuccess;
        } else if (playerStat >= threshold) {
            return GachaUnlockResult.Success;
        } else if (playerStat >= threshold * 0.5f) {
            return GachaUnlockResult.Failure;
        } else {
            return GachaUnlockResult.GreatFailure;
        }
    }
    
    // イベントタイトルからイベントタイプを判定
    string DetermineEventType(string eventTitle)
    {
        if (eventTitle.Contains("恋") || eventTitle.Contains("愛") || eventTitle.Contains("告白") || eventTitle.Contains("結婚"))
        {
            return "恋愛";
        }
        return "通常";
    }
    
    // 新しく解放されたガチャの通知メッセージを生成
    string GetGachaUnlockNotification()
    {
        if (playerData.NewlyUnlockedGachaTypes.Count == 0)
            return "";
            
        string notification = "✨ 新しいガチャが解放されました！ ✨\n";
        
        foreach (var gachaType in playerData.NewlyUnlockedGachaTypes)
        {
            string gachaName = GetGachaDisplayName(gachaType);
            string description = GetGachaDescription(gachaType);
            notification += $"🎰 {gachaName}: {description}\n";
        }
        
        // 通知をクリア
        playerData.ClearNewUnlockNotifications();
        
        return notification;
    }
    
    // ガチャタイプの表示名を取得
    string GetGachaDisplayName(GachaType gachaType)
    {
        switch (gachaType)
        {
            case GachaType.Childhood: return "童心ガチャ";
            case GachaType.Youth: return "青春ガチャ";
            case GachaType.Career: return "キャリアガチャ";
            case GachaType.Mature: return "円熟ガチャ";
            case GachaType.Senior: return "長老ガチャ";
            case GachaType.Legendary: return "伝説ガチャ";
            case GachaType.Redemption: return "逆転ガチャ";
            case GachaType.Love: return "恋愛ガチャ";
            case GachaType.Reversal: return "人生逆転ガチャ";
            default: return gachaType.ToString();
        }
    }
    
    // ガチャの説明を取得
    string GetGachaDescription(GachaType gachaType)
    {
        switch (gachaType)
        {
            case GachaType.Childhood: return "子供時代だけの特別な経験";
            case GachaType.Youth: return "青春時代限定の輝き";
            case GachaType.Career: return "社会人としての武器";
            case GachaType.Mature: return "人生経験が生む深み";
            case GachaType.Senior: return "老獪な知恵と余裕";
            case GachaType.Legendary: return "大成功の報酬として";
            case GachaType.Redemption: return "失敗からの学び";
            case GachaType.Love: return "愛の力で特別な何かを";
            case GachaType.Reversal: return "30歳の人生逆転チャンス";
            default: return "特別なガチャ";
        }
    }
    
    void AdvanceToNextStage()
    {
        playerData.AdvanceAge();
        
        if (playerData.LifeCycle >= 20 || playerData.IsGameOver())
        {
            // 神になった場合の特別エンディング
            if (playerData.LifeCycle >= 20)
            {
                StartCoroutine(ShowDivineEnding());
            }
            else
            {
                // 人生の軌跡を表示
                StartCoroutine(ShowLifeHistory());
            }
        }
        else if (playerData.LifeCycle >= 16)
        {
            // 神の道ルートに入った場合の通知
            if (playerData.CanAccessDivinePath() && playerData.LifeCycle == 16)
            {
                StartCoroutine(ShowDivinePathOpening());
            }
            else
            {
                // 通常の進行
                SceneManager.LoadScene("FamiryGachaScene");
            }
        }
        else
        {
            // 次のガチャフェーズへ
            SceneManager.LoadScene("FamiryGachaScene");
        }
    }
    
    // 人生の軌跡を表示する
    IEnumerator ShowLifeHistory()
    {
        yield return StartCoroutine(TypeText("\n\n========== あなたの人生の軌跡 ==========\n"));
        yield return new WaitForSeconds(1f);
        
        foreach (var record in playerData.EventHistory)
        {
            string historyLine = $"【{record.LifeStage}】 {record.EventTitle} -> {record.ResultText}";
            yield return StartCoroutine(TypeText(historyLine + "\n"));
            yield return new WaitForSeconds(0.5f);
        }
        
        yield return StartCoroutine(TypeText("======================================\n"));
        yield return new WaitForSeconds(2f);
        
        // デバッグログにも出力
        Debug.Log("========== あなたの人生の軌跡 ==========");
        foreach (var record in playerData.EventHistory)
        {
            Debug.Log($"【{record.LifeStage}】 {record.EventTitle} -> {record.ResultText}");
        }
        Debug.Log("======================================");
        
        if (playerData.IsGameOver())
        {
            yield return StartCoroutine(TypeText("\n💀 GAME OVER\n\nお疲れさまでした..."));
        }
        else
        {
            yield return StartCoroutine(TypeText("\n🎉 人生完走！\n\nお疲れさまでした！"));
        }
        
        yield return new WaitForSeconds(5f);
        
        // 最初のシーンに戻る
        SceneManager.LoadScene("FamiryGachaScene");
    }
    
    // 小悪魔天使のゲームオーバー演出
    IEnumerator ShowAngelGameOver()
    {
        yield return StartCoroutine(TypeText("\n\n💀 魂片がゼロになりました...\n"));
        yield return new WaitForSeconds(1f);
        
        yield return StartCoroutine(TypeText("😈 \"あらあら♡ とうとう魂片がゼロになっちゃったのね\"\n"));
        yield return new WaitForSeconds(1.5f);
        
        yield return StartCoroutine(TypeText("👼 \"約束は約束よ。あなたの人生、全部わたしがいただくの♪\"\n"));
        yield return new WaitForSeconds(1.5f);
        
        yield return StartCoroutine(TypeText("✨ \"でも心配しないで。とっても美味しい人生だったわ♡\"\n"));
        yield return new WaitForSeconds(1.5f);
        
        yield return StartCoroutine(TypeText("🌟 \"また新しい子羊さんを探しに行かなくちゃ...ふふふ\"\n"));
        yield return new WaitForSeconds(2f);
        
        // 人生の軌跡を表示
        yield return StartCoroutine(ShowLifeHistory());
    }
    
    // 神の道開放演出
    IEnumerator ShowDivinePathOpening()
    {
        yield return StartCoroutine(TypeText("\n\n✨ 人間の域を超越した力を感じます...\n"));
        yield return new WaitForSeconds(1f);
        
        yield return StartCoroutine(TypeText("🌟 \"素晴らしい...あなたはわたしの予想をも超えた存在ね♡\"\n"));
        yield return new WaitForSeconds(1.5f);
        
        yield return StartCoroutine(TypeText("👼 \"実は...神への道があるの。でも、とても険しい道よ？\"\n"));
        yield return new WaitForSeconds(1.5f);
        
        yield return StartCoroutine(TypeText("⚡ 「天界の門が開かれました...神の審判を受ける覚悟はありますか？」\n"));
        yield return new WaitForSeconds(2f);
        
        yield return StartCoroutine(TypeText("🔥 【神の道ルート開放】上位5%のプレイヤーのみが挑戦できる特別ルートです\n"));
        yield return new WaitForSeconds(3f);
        
        SceneManager.LoadScene("FamiryGachaScene");
    }
    
    // 神エンディング演出
    IEnumerator ShowDivineEnding()
    {
        yield return StartCoroutine(TypeText("\n\n🌟 全ての審判を乗り越えました...\n"));
        yield return new WaitForSeconds(1f);
        
        if (playerData.RemainingTenmei >= 1000)
        {
            // 完全神エンディング
            yield return StartCoroutine(TypeText("👑 \"完璧なる神として昇天なさいましたね♡\"\n"));
            yield return new WaitForSeconds(1.5f);
            yield return StartCoroutine(TypeText("✨ \"わたしも、あなたに仕える天使として永遠に従います\"\n"));
            yield return new WaitForSeconds(1.5f);
            yield return StartCoroutine(TypeText("🌟 あなたは新たな宇宙の創造主となりました\n"));
            yield return new WaitForSeconds(2f);
            yield return StartCoroutine(TypeText("🎉 【TRUE END：神への昇格】おめでとうございます！\n"));
        }
        else if (playerData.RemainingTenmei >= 500)
        {
            // 准神エンディング
            yield return StartCoroutine(TypeText("⭐ \"準神として天界の一角をお与えします\"\n"));
            yield return new WaitForSeconds(1.5f);
            yield return StartCoroutine(TypeText("👼 \"いつか完全なる神になる日を楽しみにしていますね♡\"\n"));
            yield return new WaitForSeconds(2f);
            yield return StartCoroutine(TypeText("🎊 【GOOD END：准神】素晴らしい結果です！\n"));
        }
        else
        {
            // 審判失敗エンディング
            yield return StartCoroutine(TypeText("😔 \"残念...神の資格には届きませんでしたね\"\n"));
            yield return new WaitForSeconds(1.5f);
            yield return StartCoroutine(TypeText("👼 \"でも大丈夫！また挑戦すればいいのよ♡\"\n"));
            yield return new WaitForSeconds(2f);
            yield return StartCoroutine(TypeText("💫 【NORMAL END：人間界復帰】また挑戦してくださいね！\n"));
        }
        
        yield return new WaitForSeconds(3f);
        
        // 人生の軌跡を表示
        yield return StartCoroutine(ShowLifeHistory());
    }
}
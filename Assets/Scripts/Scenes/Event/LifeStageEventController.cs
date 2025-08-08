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
        var allEvents = LifeStageEventData.GetAllEvents();
        currentEvent = allEvents.FirstOrDefault(e => e.lifeCycle == playerData.LifeCycle);
        
        if (currentEvent == null)
        {
            // イベントが見つからない場合は次のライフサイクルへ
            Debug.LogWarning($"No event found for life cycle {playerData.LifeCycle}");
            AdvanceToNextStage();
            return;
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
                
            // 天命不足の場合はボタン無効化
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
                         $"天命: {playerData.RemainingTenmei}ポイント | {playerData.GetLifeStage()}";
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
            ShowResult("⏳ 天命が足りません！");
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
        EventResult result;

        if (playerStat >= threshold * 1.5f) {
            result = currentEvent.greatSuccess;
        } else if (playerStat >= threshold) {
            result = currentEvent.success;
        } else if (playerStat >= threshold * 0.5f) {
            result = currentEvent.failure;
        } else {
            result = currentEvent.greatFailure;
        }

        // 2. 天命を変動させる
        playerData.RemainingTenmei += result.tenmeiChange;

        // 3. 表示する最終的なメッセージを一度に組み立てる
        string statusInfluence = $"({GetStatusName(currentEvent.requiredStatus)}が影響しました)";
        string changeDescription = result.tenmeiChange >= 0 ? "🎉" : "💔";
        string finalMessage = $"{result.text}\n{statusInfluence}\n\n{changeDescription} {result.description}";

        // ★★★ 結果を履歴に記録 ★★★
        playerData.AddEventRecord(playerData.GetLifeStage(), currentEvent.eventTitle, result.text);

        // 4. 最終的なメッセージを一度だけ表示する
        yield return StartCoroutine(TypeText(finalMessage));

        // ゲームオーバーチェック
        if (playerData.IsGameOver())
        {
            yield return StartCoroutine(TypeText("\n\n💀 あなたの天命は尽きました..."));
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene("GameOver");
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
    
    void AdvanceToNextStage()
    {
        playerData.AdvanceAge();
        
        if (playerData.LifeCycle >= 16 || playerData.IsGameOver())
        {
            // ★★★ ゲーム終了時に履歴をログに出力 ★★★
            Debug.Log("========== あなたの人生の軌跡 ==========");
            foreach (var record in playerData.EventHistory)
            {
                Debug.Log($"【{record.LifeStage}】 {record.EventTitle} -> {record.ResultText}");
            }
            Debug.Log("======================================");

            // ゲーム終了
            SceneManager.LoadScene("GameEnd");
        }
        else
        {
            // 次のガチャフェーズへ
            SceneManager.LoadScene("FamiryGachaScene");
        }
    }
}
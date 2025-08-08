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
                
            // 寿命不足の場合はボタン無効化
            gachaButton.interactable = playerData.CanAffordLifespan(2);
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
                         $"必要ポイント: {currentEvent.difficultyThreshold}pt\n" +
                         $"寿命: {playerData.RemainingLifespan}年 | {playerData.GetLifeStage()}";
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
        
        if (!playerData.CanAffordLifespan(2))
        {
            ShowResult("⏰ 寿命が足りません！");
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
        
        // チャレンジ処理
        int playerStat = GetRequiredStatusValue();
        bool success = playerStat >= currentEvent.difficultyThreshold;
        
        string resultMessage = success ? currentEvent.successText : currentEvent.failureText;
        
        // 結果表示
        yield return StartCoroutine(TypeText(resultMessage));
        
        // 報酬/ペナルティ適用
        if (success)
        {
            playerData.RemainingLifespan += currentEvent.successReward.lifespanBonus;
            resultMessage += "\n\n🎉 " + currentEvent.successReward.description;
        }
        else
        {
            playerData.SpendLifespan(currentEvent.failurePenalty.lifespanLoss);
            resultMessage += "\n\n💔 " + currentEvent.failurePenalty.description;
        }
        
        yield return StartCoroutine(TypeText(resultMessage));
        
        // ゲームオーバーチェック
        if (playerData.IsGameOver())
        {
            yield return StartCoroutine(TypeText("\n\n💀 あなたの人生は終了しました..."));
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene("GameOver"); // ゲームオーバーシーンへ
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
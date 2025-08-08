using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using HikukaHikanaika.Models;

public class RealityBattle : MonoBehaviour
{
    [Header("UI References")]
    public Text dialogueText;
    public Image clickableArea;
    public Button buttonA;
    public Button buttonB;
    public Button buttonC;
    public Text insightText;
    
    [Header("Settings")]
    public float textSpeed = 0.05f;
    string nextSceneName = "RealityBattle";
    
    [Header("Scene Transition")]
    public float transitionDelay = 1.0f;
    
    // Game State
    private PlayerData playerData;
    private RivalData reika;
    private GamePhase currentPhase = GamePhase.Introduction;
    private BattleStatusType targetStatus;
    private PlayerChoice playerChoice;

    private string[] lines = {
        "▼対戦者",    
        "麗華 (Reika)\n * 家柄: 日本史に名を刻む名家\n * 容姿: オートクチュールを纏う芸術品 \n * 性格: 天性の支配者（人たらし）",
        "『麗華』\n\n ごきげんよう。\n わたくしとゲームができるなんて、光栄に思いなさいな\n\n",
        "さあ、あなたの選択肢は。",
        "*A:親しみやすさで勝負する\n*B:知性で興味を引く\n*C:庇護欲をくすぐる",
    };

    // Dialogue State
    private int currentLine = 0;
    private bool isTyping = false;
    private bool isWaitingForNextLine = false;
    private bool isWaitingForChoice = false;
    private bool gameEnded = false;
    
    // Battle System Enums
    public enum GamePhase { Introduction, Observation, Choice, Result }
    public enum BattleStatusType { Family, Appearance, Personality }
    public enum PlayerChoice { Friendly, Intelligence, Protection }
    public enum BattleResult { GreatVictory, Victory, NarrowVictory, Defeat, GreatDefeat }

    void Start()
    {
        InitializeGame();
        
        if (dialogueText == null || clickableArea == null)
        {
            Debug.LogError("TextかImageが未設定です！");
            return;
        }

        clickableArea.GetComponent<Image>().raycastTarget = true;
        clickableArea.GetComponent<Button>()?.onClick.AddListener(OnClick);
        
        SetupButtons();
        HideChoiceButtons();
        
        // 最初のセリフ表示
        StartCoroutine(TypeLine(lines[currentLine]));
    }

    public void OnClick()
    {
        Debug.Log($"=== OnClick === Line: {currentLine}, Phase: {currentPhase}, Typing: {isTyping}, WaitNext: {isWaitingForNextLine}, WaitChoice: {isWaitingForChoice}, GameEnded: {gameEnded}");

        // ゲーム終了後は何もしない
        if (gameEnded)
        {
            Debug.Log(">> Game ended - click ignored");
            return;
        }

        // タイピング中はスキップ
        if (isTyping)
        {
            Debug.Log(">> Skipping typing");
            StopAllCoroutines();
            CompleteCurrentLine();
            return;
        }

        // 選択肢待ち中はクリック無視
        if (isWaitingForChoice)
        {
            Debug.Log(">> Waiting for choice - click ignored");
            return;
        }

        // 次の行待ち中のみ進行
        if (isWaitingForNextLine)
        {
            Debug.Log(">> Proceeding to next line");
            ProceedToNextLine();
        }
    }
    
    IEnumerator StartObservationPhase()
    {
        Debug.Log(">> Starting observation phase");
        currentPhase = GamePhase.Observation;
        
        yield return new WaitForSeconds(1f);
        
        // TARGET INSIGHTを表示
        string insightMessage = GetTargetInsightMessage();
        if (insightText != null)
        {
            insightText.text = insightMessage;
            insightText.gameObject.SetActive(true);
            Debug.Log(">> TARGET INSIGHT displayed");
        }
        else
        {
            Debug.LogError(">> insightText is null!");
        }
        
        yield return new WaitForSeconds(2f);
        
        // 選択ボタンを表示
        currentPhase = GamePhase.Choice;
        isWaitingForChoice = true;
        ShowChoiceButtons();
        Debug.Log(">> Choice buttons displayed, waiting for selection");
    }
    
    string GetTargetInsightMessage()
    {
        switch (targetStatus)
        {
            case BattleStatusType.Family:
                return "【TARGET INSIGHT】\n\n彼の視線は、あなたと麗華様の立ち居振る舞いや言葉遣いに注目している。\n彼が今、最も重視しているのは...【家柄】かもしれない。";
            case BattleStatusType.Appearance:
                return "【TARGET INSIGHT】\n\n彼の視線は、あなたの洗練された立ち居振る舞いや、麗華様の華やかなドレスに、素直に惹かれているようだ。\n彼が今、最も重視しているのは...【容姿】かもしれない。";
            case BattleStatusType.Personality:
                return "【TARGET INSIGHT】\n\n彼は会話の内容や、あなたたちの人柄に興味を示している。表面的な美しさより、内面を重視するタイプのようだ。\n彼が今、最も重視しているのは...【性格】かもしれない。";
            default:
                return "【TARGET INSIGHT】\n\n彼の心理は読み取れない...";
        }
    }
    
    void InitializeGame()
    {
        // プレイヤーデータをシングルトンから取得
        playerData = PlayerData.Instance;
        
        // 麗華のデータ設定
        reika = new RivalData(
            "麗華",
            new GachaItem("オートクチュールのドレス", 0.1f, 150, 0, 0, 0, GachaType.Beauty),
            new GachaItem("日本史レベルの名家", 0.05f, 200, 0, 0, 0, GachaType.FamilyWealth),
            new GachaItem("天性の人たらし", 0.08f, 180, 0, 0, 0, GachaType.Personality)
        );
        
        // 男性が重視するステータスをランダム決定
        targetStatus = (BattleStatusType)UnityEngine.Random.Range(0, 3);
    }
    
    void SetupButtons()
    {
        if (buttonA != null) buttonA.onClick.AddListener(() => OnChoiceSelected(PlayerChoice.Friendly));
        if (buttonB != null) buttonB.onClick.AddListener(() => OnChoiceSelected(PlayerChoice.Intelligence));
        if (buttonC != null) buttonC.onClick.AddListener(() => OnChoiceSelected(PlayerChoice.Protection));
    }
    
    void HideChoiceButtons()
    {
        if (buttonA != null) buttonA.gameObject.SetActive(false);
        if (buttonB != null) buttonB.gameObject.SetActive(false);
        if (buttonC != null) buttonC.gameObject.SetActive(false);
    }
    
    void ShowChoiceButtons()
    {
        Debug.Log("ShowChoiceButtons called");
        if (buttonA != null) 
        {
            buttonA.gameObject.SetActive(true);
            Debug.Log("Button A activated");
        }
        else Debug.LogError("Button A is null!");
        
        if (buttonB != null) 
        {
            buttonB.gameObject.SetActive(true);
            Debug.Log("Button B activated");
        }
        else Debug.LogError("Button B is null!");
        
        if (buttonC != null) 
        {
            buttonC.gameObject.SetActive(true);
            Debug.Log("Button C activated");
        }
        else Debug.LogError("Button C is null!");
    }
    
    void OnChoiceSelected(PlayerChoice choice)
    {
        Debug.Log($">> Choice selected: {choice}");
        playerChoice = choice;
        isWaitingForChoice = false;
        HideChoiceButtons();
        
        // insightTextを非表示
        if (insightText != null)
        {
            insightText.gameObject.SetActive(false);
        }
        
        StartCoroutine(ProcessBattleResult());
    }
    
    IEnumerator ProcessBattleResult()
    {
        Debug.Log(">> Processing battle result");
        currentPhase = GamePhase.Result;
        
        BattleResult result = CalculateBattleResult();
        string resultText = GetResultDescription(result);
        
        // 結果をdialogueTextに表示
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in resultText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        isTyping = false;
        
        Debug.Log(">> Battle result displayed - game ended");
        gameEnded = true; // ゲーム終了フラグ
        
        // シーン遷移しない
    }
    
    BattleResult CalculateBattleResult()
    {
        bool playerAdvantage = GetPlayerStatusAdvantage();
        
        // ステータス優劣と選択肢による結果判定表
        switch (targetStatus)
        {
            case BattleStatusType.Family:
                return CalculateFamilyBattleResult(playerAdvantage, playerChoice);
            case BattleStatusType.Appearance:
                return CalculateAppearanceBattleResult(playerAdvantage, playerChoice);
            case BattleStatusType.Personality:
                return CalculatePersonalityBattleResult(playerAdvantage, playerChoice);
            default:
                return BattleResult.Defeat;
        }
    }
    
    bool GetPlayerStatusAdvantage()
    {
        int playerStatusValue = GetPlayerStatusValue();
        int reikaStatusValue = GetReikaStatusValue();
        return playerStatusValue > reikaStatusValue;
    }
    
    int GetPlayerStatusValue()
    {
        switch (targetStatus)
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
    
    int GetReikaStatusValue()
    {
        switch (targetStatus)
        {
            case BattleStatusType.Family:
                return reika.FamilyWealth?.points ?? 0;
            case BattleStatusType.Appearance:
                return reika.Outfit?.points ?? 0;
            case BattleStatusType.Personality:
                return reika.Personality?.points ?? 0;
            default:
                return 0;
        }
    }
    
    BattleResult CalculateFamilyBattleResult(bool playerAdvantage, PlayerChoice choice)
    {
        if (playerAdvantage)
        {
            switch (choice)
            {
                case PlayerChoice.Friendly: return BattleResult.GreatVictory;
                case PlayerChoice.Intelligence: return BattleResult.Victory;
                case PlayerChoice.Protection: return BattleResult.Defeat;
            }
        }
        else
        {
            switch (choice)
            {
                case PlayerChoice.Friendly: return BattleResult.Victory;
                case PlayerChoice.Intelligence: return BattleResult.Defeat;
                case PlayerChoice.Protection: return BattleResult.NarrowVictory;
            }
        }
        return BattleResult.Defeat;
    }
    
    BattleResult CalculateAppearanceBattleResult(bool playerAdvantage, PlayerChoice choice)
    {
        if (playerAdvantage)
        {
            switch (choice)
            {
                case PlayerChoice.Friendly: return BattleResult.GreatVictory;
                case PlayerChoice.Intelligence: return BattleResult.Victory;
                case PlayerChoice.Protection: return BattleResult.GreatVictory;
            }
        }
        else
        {
            switch (choice)
            {
                case PlayerChoice.Friendly: return BattleResult.Victory;
                case PlayerChoice.Intelligence: return BattleResult.NarrowVictory;
                case PlayerChoice.Protection: return BattleResult.GreatDefeat;
            }
        }
        return BattleResult.Defeat;
    }
    
    BattleResult CalculatePersonalityBattleResult(bool playerAdvantage, PlayerChoice choice)
    {
        if (playerAdvantage)
        {
            switch (choice)
            {
                case PlayerChoice.Friendly: return BattleResult.GreatVictory;
                case PlayerChoice.Intelligence: return BattleResult.Victory;
                case PlayerChoice.Protection: return BattleResult.Victory;
            }
        }
        else
        {
            switch (choice)
            {
                case PlayerChoice.Friendly: return BattleResult.Defeat;
                case PlayerChoice.Intelligence: return BattleResult.Victory;
                case PlayerChoice.Protection: return BattleResult.GreatDefeat;
            }
        }
        return BattleResult.Defeat;
    }
    
    string GetResultDescription(BattleResult result)
    {
        switch (result)
        {
            case BattleResult.GreatVictory:
                return "【大勝利！】\n\nあなたの戦略は完璧でした！彼は完全にあなたの魅力に心を奪われ、麗華のことは忘れてしまったようです。";
            case BattleResult.Victory:
                return "【勝利！】\n\nあなたの選択は正しかったです。彼はあなたに強い関心を示し、麗華よりもあなたを選びました。";
            case BattleResult.NarrowVictory:
                return "【辛勝】\n\n危険な賭けでしたが、なんとか彼の心を掴むことができました。運も味方したようです。";
            case BattleResult.Defeat:
                return "【敗北】\n\n残念ながら、あなたの戦略は彼には響きませんでした。麗華の魅力の前に敗れてしまいました。";
            case BattleResult.GreatDefeat:
                return "【大敗北】\n\n最悪の選択でした。彼はあなたに失望し、麗華の元へ走って行ってしまいました...";
            default:
                return "結果不明";
        }
    }

    IEnumerator TypeLine(string line)
    {
        Debug.Log($">> Starting to type line {currentLine}: {line}");
        isTyping = true;
        isWaitingForNextLine = false;
        dialogueText.text = "";
        
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        
        CompleteCurrentLine();
    }
    
    void CompleteCurrentLine()
    {
        isTyping = false;
        dialogueText.text = lines[currentLine];
        
        Debug.Log($">> Line {currentLine} completed");
        
        // 最後の選択肢行の場合
        if (currentLine == 4)
        {
            Debug.Log(">> Last line completed - starting observation phase");
            StartCoroutine(StartObservationPhase());
        }
        else
        {
            isWaitingForNextLine = true;
        }
    }
    
    void ProceedToNextLine()
    {
        isWaitingForNextLine = false;
        currentLine++;
        
        if (currentLine < lines.Length)
        {
            StartCoroutine(TypeLine(lines[currentLine]));
        }
        else
        {
            Debug.LogWarning("No more lines to display");
        }
    }
    
    IEnumerator TransitionToNextScene()
    {
        Debug.Log("プロローグ終了。シーン遷移開始...");
        
        // 終了メッセージを表示
        dialogueText.text = "戦いの舞台へ...";
        
        // 指定された時間待機
        yield return new WaitForSeconds(transitionDelay);
        
        // 次のシーンに遷移
        try
        {
            SceneManager.LoadScene(nextSceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"シーン '{nextSceneName}' の読み込みに失敗しました: {e.Message}");
            Debug.LogError("Build Settings に GameProtoScene が追加されているか確認してください。");
            
            // フォールバック: インデックスでの遷移を試行
            try
            {
                SceneManager.LoadScene(1); // 通常、インデックス0はプロローグ、1がゲーム本編
            }
            catch (System.Exception fallbackE)
            {
                Debug.LogError($"フォールバックシーン遷移も失敗: {fallbackE.Message}");
                dialogueText.text = "シーン遷移エラー。ゲームを再起動してください。";
            }
        }
    }
}

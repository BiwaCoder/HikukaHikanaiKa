using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using HikukaHikanaika.Models;
using HikukaHikanaika.Logic;
#if UNITY_EDITOR
using UnityEditor;
#endif

// 天使はささやく「ガチャを引け」と。 or  ガチャを引いたら人生変わった件
public class PrologueSerif : MonoBehaviour
{
    [Header("UI References")]
    public Text dialogueText;
    public Image clickableArea;

    [Header("Settings")]
    public float textSpeed = 0.05f;
    string nextSceneName = "FamiryGachaScene";

    [Header("Scene Transition")]
    public float transitionDelay = 1.0f;

    [Header("Debug Settings (Editor Only)")]
    [SerializeField] private bool debugMaxStats = false;
    [SerializeField] private bool debugStartAt70 = false;

    private string[] lines = {
        "あら♡ 迷い込んできたのね、可愛い子羊さん。",
        "わたしは天使。この世界の運命をいじれるの。",
        "『魂片（ソウルピース）』っていう……あなたの魂の欠片。",
        "それをちょっとくれれば、ガチャを退かせてあげるよ♡",
        "美貌も、才能も、恋も、幸運も……ぜんぶ選び放題♡",
        "でもね、魂片がゼロになったら――全部、わたしのもの。",
        "さぁ……禁断のガチャ、引いてみる？"
    };

    private int currentLine = 0;
    private bool isTyping = false;
    private bool canProceed = false;

    void Start()
    {
        // シングルトンを初期化
        PlayerData.Initialize();
        GachaLogic.Initialize();


        if (dialogueText == null || clickableArea == null)
        {
            Debug.LogError("TextかImageが未設定です！");
            return;
        }

        clickableArea.GetComponent<Image>().raycastTarget = true;
        clickableArea.GetComponent<Button>()?.onClick.AddListener(OnClick);

        // 最初のセリフ表示
        StartCoroutine(TypeLine(lines[currentLine]));
    }

    public void OnClick()
    {
        Debug.Log("クリックされました");

        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = lines[currentLine];
            isTyping = false;
            canProceed = true;
        }
        else if (canProceed)
        {
            currentLine++;
            if (currentLine < lines.Length)
            {
                StartCoroutine(TypeLine(lines[currentLine]));
            }
            else
            {
                // 全てのセリフが終了したらシーン遷移
                StartCoroutine(TransitionToNextScene());
            }
            canProceed = false;
        }
    }

    IEnumerator TypeLine(string line)
    {
        Debug.Log("描画開始");
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        isTyping = false;
        canProceed = true;
    }

    IEnumerator TransitionToNextScene()
    {
        Debug.Log("プロローグ終了。シーン遷移開始...");

#if UNITY_EDITOR
        // デバッグ機能を適用
        ApplyDebugSettings();
#endif

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

#if UNITY_EDITOR
    private void ApplyDebugSettings()
    {
        PlayerData playerData = PlayerData.Instance;
        
        // ステータスMAX化
        if (debugMaxStats)
        {
            Debug.Log("[Debug] ステータスを最大値に設定");
            playerData.Luck = 999;
            playerData.Concentration = 999;
            playerData.Kindness = 999;
            
            // 美貌、家柄、性格の装備アイテムも最高値に設定
            playerData.CurrentOutfit = new GachaItem("デバッグ美貌装備", 1.0f, 999, 0, 0, 0, GachaType.Beauty);
            playerData.CurrentFamilyWealth = new GachaItem("デバッグ家柄装備", 1.0f, 999, 0, 0, 0, GachaType.FamilyWealth);
            playerData.CurrentPersonality = new GachaItem("デバッグ性格装備", 1.0f, 999, 0, 0, 0, GachaType.Personality);
        }
        
        // 70歳スタート
        if (debugStartAt70)
        {
            Debug.Log("[Debug] 70歳からスタート");
            playerData.LifeCycle = 14; // 70-75歳の長老期
            playerData.CurrentAge = 70;
        }
    }
#endif
}

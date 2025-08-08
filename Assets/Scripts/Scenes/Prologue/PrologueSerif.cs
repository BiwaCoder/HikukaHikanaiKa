using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using HikukaHikanaika.Models;
using HikukaHikanaika.Logic;

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

    private string[] lines = {
        "生まれたばかりの魂よ、これから始まる長い旅路を歩む準備はできているか？",
        "あなたには80年という限られた時間が与えられた。",
        "しかし、運命は固定されたものではない……",
        "寿命を削り、ガチャを引くことで、あなたの人生は劇的に変わる。",
        "美貌、家柄、性格──すべてはあなたの選択次第。",
        "ただし、時は無情に過ぎ去る。寿命が尽きればゲームオーバーだ。",
        "さあ、あなたの人生を賭けた壮大なガチャゲームの始まりです。"
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
        
        // 終了メッセージを表示
        dialogueText.text = "人生という名のガチャゲームへ...";
        
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

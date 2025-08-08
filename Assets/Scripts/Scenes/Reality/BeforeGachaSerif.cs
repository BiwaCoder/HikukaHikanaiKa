using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BeforeGachaSerif : MonoBehaviour
{
    [Header("UI References")]
    public Text dialogueText;
    public Image clickableArea;
    
    [Header("Settings")]
    public float textSpeed = 0.05f;
    string nextSceneName = "LifeStageEventScene";
    
    [Header("Scene Transition")]
    public float transitionDelay = 1.0f;

    private string[] lines = {
        "望んだ運命は手に入りましたか？",
        "さて……",
        "これから、あなたの人生の各段階での出来事を体験してもらいます。",
        "それぞれのステージで、あなたの選択が未来を形作ります。",
        "さあ、あなたの物語を始めましょう。"
    };

    private int currentLine = 0;
    private bool isTyping = false;
    private bool canProceed = false;

    void Start()
    {
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

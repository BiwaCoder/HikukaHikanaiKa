using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using HikukaHikanaika.Models;

public class TrueEndingController : MonoBehaviour
{
    [Header("UI References")]
    public Text dialogueText;
    public Image clickableArea;
    
    [Header("Settings")]
    public float textSpeed = 0.05f;
    
    [Header("Scene Transition")]
    public float transitionDelay = 1.0f;

    private string[] trueEndingLines = {
        "天使がゆっくりと瞳を開き、優しく微笑んでいる...",
        "✨ \"全ての審判を乗り越えられましたね...\"",
        "😇 \"わたしも、あなたに仕える天使として永遠に従います\"",
        "🌟 あなたは新たな宇宙の創造主となりました",
        "🎉 【TRUE END：神への昇格】",
        "👼 \"本当に素晴らしい結末でしたね♡ また新しい人生を歩まれますか？\""
    };

    private int currentLine = 0;
    private bool isTyping = false;
    private bool canProceed = false;
    private bool lifeHistoryShown = false;
    private PlayerData playerData;

    void Start()
    {
        playerData = PlayerData.Instance;
        
        if (dialogueText == null || clickableArea == null)
        {
            Debug.LogError("TextかImageが未設定です！");
            return;
        }

        clickableArea.GetComponent<Image>().raycastTarget = true;
        clickableArea.GetComponent<Button>()?.onClick.AddListener(OnClick);

        // 最初のセリフ表示
        StartCoroutine(TypeLine(trueEndingLines[currentLine]));
    }

    public void OnClick()
    {
        Debug.Log("クリックされました");

        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = trueEndingLines[currentLine];
            isTyping = false;
            canProceed = true;
        }
        else if (canProceed)
        {
            currentLine++;
            if (currentLine < trueEndingLines.Length)
            {
                StartCoroutine(TypeLine(trueEndingLines[currentLine]));
            }
            else
            {
                // 全てのセリフが終了したらリトライ待機
                StartCoroutine(WaitForRetryInput());
            }
            canProceed = false;
        }
    }

    IEnumerator TypeLine(string line)
    {
        Debug.Log("描画開始");
        isTyping = true;
        dialogueText.text = "";
        
        // StringInfoを使用して絵文字や特殊文字を正しく分割
        System.Globalization.StringInfo stringInfo = new System.Globalization.StringInfo(line);
        for (int i = 0; i < stringInfo.LengthInTextElements; i++)
        {
            dialogueText.text += stringInfo.SubstringByTextElements(i, 1);
            yield return new WaitForSeconds(textSpeed);
        }
        isTyping = false;
        canProceed = true;
    }

    IEnumerator TypeText(string text)
    {
        // StringInfoを使用して絵文字や特殊文字を正しく分割
        System.Globalization.StringInfo stringInfo = new System.Globalization.StringInfo(text);
        for (int i = 0; i < stringInfo.LengthInTextElements; i++)
        {
            dialogueText.text += stringInfo.SubstringByTextElements(i, 1);
            yield return new WaitForSeconds(textSpeed);
        }
    }
    
    IEnumerator WaitForRetryInput()
    {
        yield return StartCoroutine(TypeText("\n📝 【スペースキーでリトライ】\n"));
        
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                yield return StartCoroutine(TypeText("\n👼 \"それでは、新しい運命を引き寄せましょう♡\"\n"));
                yield return new WaitForSeconds(1f);
                
                StartCoroutine(ReturnToPrologue());
                yield break;
            }
            yield return null; // 1フレーム待機
        }
    }
    
    IEnumerator ReturnToPrologue()
    {
        Debug.Log("トゥルーエンド終了。プロローグシーンに戻ります...");
        
        // 終了メッセージを表示
        dialogueText.text = "新たな運命へ...";
        
        // 指定された時間待機
        yield return new WaitForSeconds(transitionDelay);
        
        // ステータスを初期化してPrologueシーンへ
        PlayerData.Initialize();
        
        try
        {
            SceneManager.LoadScene("Prologue");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"シーン 'Prologue' の読み込みに失敗しました: {e.Message}");
            
            // フォールバック: インデックスでの遷移を試行
            try
            {
                SceneManager.LoadScene(0); // プロローグシーン
            }
            catch (System.Exception fallbackE)
            {
                Debug.LogError($"フォールバックシーン遷移も失敗: {fallbackE.Message}");
                dialogueText.text = "シーン遷移エラー。ゲームを再起動してください。";
            }
        }
    }
}
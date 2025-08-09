using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using HikukaHikanaika.Models;

public class BadEndingController : MonoBehaviour
{
    [Header("UI References")]
    public Text dialogueText;
    public Image clickableArea;
    
    [Header("Settings")]
    public float textSpeed = 0.05f;
    
    [Header("Scene Transition")]
    public float transitionDelay = 1.0f;

    private string[] badEndingLines = {
        "ふと気づくと、天使の瞳が小悪魔のように輝いている...",
        "😈 \"あらあら♡ とうとう魂片がゼロになっちゃったのね\"",
        "👼 \"約束は約束よ。あなたの人生、全部わたしがいただくの♪\"",
        "✨ \"でも心配しないで。とっても美味しい人生だったわ♡\"",
        "🌟 魂片が完全に消失し、あなたの体は光の粒となって舞い上がる",
        "😈 \"ふふふ♡ あなたの魂、とても甘くて美味しかったわ\"",
        "✨ 光の粒は天使の手の中に吸い込まれていく...",
        "💀 GAME OVER 💀"
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
        StartCoroutine(TypeLine(badEndingLines[currentLine]));
    }

    public void OnClick()
    {
        Debug.Log("クリックされました");

        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = badEndingLines[currentLine];
            isTyping = false;
            canProceed = true;
        }
        else if (canProceed)
        {
            currentLine++;
            if (currentLine < badEndingLines.Length)
            {
                StartCoroutine(TypeLine(badEndingLines[currentLine]));
            }
            else
            {
                // 全てのセリフが終了したらプロローグシーンに戻る
                StartCoroutine(ReturnToPrologue());
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

    IEnumerator ShowLifeHistory()
    {
        isTyping = true;
        lifeHistoryShown = true;
        
        yield return StartCoroutine(TypeText("\n\n========== あなたの人生の軌跡 ==========\n"));
        yield return new WaitForSeconds(1f);
        
        foreach (var record in playerData.EventHistory)
        {
            string historyLine = $"【{record.LifeStage}】\n{record.EventTitle}\n-> {record.ResultText}";
            yield return StartCoroutine(TypeText(historyLine + "\n\n"));
            yield return new WaitForSeconds(0.8f);
        }
        
        yield return StartCoroutine(TypeText("======================================\n"));
        yield return new WaitForSeconds(2f);
        
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
    
    IEnumerator ReturnToPrologue()
    {
        Debug.Log("バッドエンド終了。プロローグシーンに戻ります...");
        
        // 終了メッセージを表示
        dialogueText.text = "また新しい運命を探しに...";
        
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
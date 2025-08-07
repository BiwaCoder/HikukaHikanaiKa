using UnityEngine;
using UnityEngine.UI;

public class GachaUIController : MonoBehaviour
{
    public Text titleText;       
    public Text descriptionText;
    public Text detailText;
    public Text priceText;
    public Button gachaButton;
    public Button nextButton;

    public GachaData[] gachaDataList;
    private int currentIndex = 0;

    void Start()
    {
        ShowCurrentGacha();
        gachaButton.onClick.AddListener(OnGachaClick);
        nextButton.onClick.AddListener(OnNextClick);
    }

    void ShowCurrentGacha()
    {
        if (gachaDataList.Length == 0 || currentIndex >= gachaDataList.Length) return;

        var data = gachaDataList[currentIndex];
        titleText.text = data.title;
        descriptionText.text = data.description;
        detailText.text = data.detail;
        priceText.text = "価格 " + data.price.ToString("N0") + "万円";
    }

    void OnGachaClick()
    {
        Debug.Log("ガチャを引いた: " + gachaDataList[currentIndex].title);
        // 抽選ロジックをここに
    }

    void OnNextClick()
    {
        currentIndex = (currentIndex + 1) % gachaDataList.Length;
        ShowCurrentGacha();
    }
}

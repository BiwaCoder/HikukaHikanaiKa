using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeController : MonoBehaviour
{
    bool isBeautyGachaActive = true;
    public GameObject beautyGachaManagerObject;
    public GameObject realityShowJudgeObject;

    // Start is called before the first frame
    //  update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void FlipGameMode()
    {
        Debug.Log("Game mode flipped");
        if (isBeautyGachaActive)
        {
            isBeautyGachaActive = false;
            beautyGachaManagerObject.SetActive(false);
            realityShowJudgeObject.SetActive(true);
        }
        else
        {
            isBeautyGachaActive = true;
            beautyGachaManagerObject.SetActive(true);
            realityShowJudgeObject.SetActive(false);
        }
    }
}

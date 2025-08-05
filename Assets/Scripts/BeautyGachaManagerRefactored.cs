using UnityEngine;
using HikukaHikanaika.Controllers;

namespace HikukaHikanaika
{
    /// <summary>
    /// レガシーサポート用のラッパークラス
    /// 既存のUnityシーンとの互換性を保つため、新しいMVCアーキテクチャへの橋渡しを行う
    /// </summary>
    public class BeautyGachaManagerRefactored : MonoBehaviour
    {
        [Header("MVC Controllers")]
        public GachaController gachaController;
        public RealityShowController realityShowController;
        
        void Start()
        {
            if (gachaController == null)
            {
                Debug.LogWarning("GachaController is not assigned. Please assign it in the inspector.");
            }
            
            if (realityShowController == null)
            {
                Debug.LogWarning("RealityShowController is not assigned. Please assign it in the inspector.");
            }
        }
        
        // レガシーコードとの互換性のためのプロパティ
        public Models.GachaItem CurrentOutfit => gachaController?.PlayerData?.CurrentOutfit;
        public Models.GachaItem CurrentFamilyWealth => gachaController?.PlayerData?.CurrentFamilyWealth;
        public Models.GachaItem CurrentPersonality => gachaController?.PlayerData?.CurrentPersonality;
    }
}
using System;
using UnityEngine;

[Serializable]
public class PlayerStatus
{
    [Header("プレイヤー情報")]
    public string playerName = "あなた";
    
    [Header("現在の装備")]
    public string currentOutfit = "何も着ていない";
    public int beautyPoints = 0;
    
    public string currentFamilyWealth = "特になし";
    public int familyWealthPoints = 0;
    
    public string currentPersonality = "特になし";
    public int personalityPoints = 0;
    
    [Header("合計ステータス")]
    public int totalPoints = 0;
    
    public void UpdateStatus(string outfit, int beauty, string family, int familyWealth, string personality, int personalityPts)
    {
        currentOutfit = outfit ?? "何も着ていない";
        beautyPoints = beauty;
        
        currentFamilyWealth = family ?? "特になし";
        familyWealthPoints = familyWealth;
        
        currentPersonality = personality ?? "特になし";
        personalityPoints = personalityPts;
        
        totalPoints = beautyPoints + familyWealthPoints + personalityPoints;
    }
    
    public string GetDetailedStatus()
    {
        return $"【{playerName}】\n👗 {currentOutfit} ({beautyPoints})\n🏠 {currentFamilyWealth} ({familyWealthPoints})\n😊 {currentPersonality} ({personalityPoints})\n🎆 合計: {totalPoints}";
    }
}
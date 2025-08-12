using System.Collections.Generic;

namespace HikukaHikanaika.Models
{
    public class RealityShowModel
    {
        public List<RivalData> Rivals { get; private set; }
        
        public RealityShowModel()
        {
            InitializeRivals();
        }
        
        private void InitializeRivals()
        {
            Rivals = new List<RivalData>
            {
                new RivalData(
                    "麗華お嬢様",
                    new GachaItem("オートクチュールのドレス", 0, 150, 0, 0, 0, GachaType.Beauty),
                    new GachaItem("一族で日本史に出てくる", 0, 200, 0, 0, 0, GachaType.FamilyWealth),
                    new GachaItem("天性の人たらし", 0, 180, 0, 0, 0, GachaType.Personality)
                ),
                new RivalData(
                    "田中頑張り子",
                    new GachaItem("制服しか勝たん", 0, 80, 0, 0, 0, GachaType.Beauty),
                    new GachaItem("一般庶民", 0, 10, 0, 0, 0, GachaType.FamilyWealth),
                    new GachaItem("ツンデ레の黄金比", 0, 100, 0, 0, 0, GachaType.Personality)
                ),
                new RivalData(
                    "佐藤普通美",
                    new GachaItem("清潔感あるけど量販感", 0, 15, 0, 0, 0, GachaType.Beauty),
                    new GachaItem("上流階級", 0, 40, 0, 0, 0, GachaType.FamilyWealth),
                    new GachaItem("無難すぎて覚えてもらえない", 0, 25, 0, 0, 0, GachaType.Personality)
                )
            };
        }
        
        public JudgeResult JudgeAgainst(PlayerData player, RivalData rival)
        {
            int playerTotal = player.GetAppearancePoints();
            int rivalTotal = rival.GetTotalPoints();
            
            if (playerTotal > rivalTotal)
                return JudgeResult.Win;
            else if (playerTotal < rivalTotal)
                return JudgeResult.Lose;
            
            // 同点の場合は特殊判定
            return JudgeSpecialCases(player, rival);
        }
        
        private JudgeResult JudgeSpecialCases(PlayerData player, RivalData rival)
        {
            // オートクチュールのドレス持ちは有利
            bool playerHasAutoCouture = player.CurrentOutfit?.name.Contains("オートクチュール") ?? false;
            bool rivalHasAutoCouture = rival.Outfit?.name.Contains("オートクチュール") ?? false;
            
            if (playerHasAutoCouture && !rivalHasAutoCouture)
                return JudgeResult.Win;
            if (!playerHasAutoCouture && rivalHasAutoCouture)
                return JudgeResult.Lose;
                
            // 天性の人たらし持ちは有利
            bool playerHasNaturalCharmer = player.CurrentPersonality?.name.Contains("天性") ?? false;
            bool rivalHasNaturalCharmer = rival.Personality?.name.Contains("天性") ?? false;
            
            if (playerHasNaturalCharmer && !rivalHasNaturalCharmer)
                return JudgeResult.Win;
            if (!playerHasNaturalCharmer && rivalHasNaturalCharmer)
                return JudgeResult.Lose;
                
            // 最低ランクアイテムは不利
            bool playerHasBadSize = player.CurrentOutfit?.name.Contains("サイズ合ってない") ?? false;
            bool rivalHasBadSize = rival.Outfit?.name.Contains("サイズ合ってない") ?? false;
            
            if (playerHasBadSize && !rivalHasBadSize)
                return JudgeResult.Lose;
            if (!playerHasBadSize && rivalHasBadSize)
                return JudgeResult.Win;
                
            return JudgeResult.Draw;
        }
    }
    
    public class RivalData
    {
        public string Name { get; }
        public GachaItem Outfit { get; }
        public GachaItem FamilyWealth { get; }
        public GachaItem Personality { get; }
        
        public RivalData(string name, GachaItem outfit, GachaItem familyWealth, GachaItem personality)
        {
            Name = name;
            Outfit = outfit;
            FamilyWealth = familyWealth;
            Personality = personality;
        }
        
        public int GetTotalPoints()
        {
            return (Outfit?.points ?? 0) + (FamilyWealth?.points ?? 0) + (Personality?.points ?? 0);
        }
    }
    
    public enum JudgeResult
    {
        Win,
        Lose,
        Draw
    }
}
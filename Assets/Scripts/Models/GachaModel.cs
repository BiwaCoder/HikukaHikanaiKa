using System;
using System.Collections.Generic;
using UnityEngine;

namespace HikukaHikanaika.Models
{
    public class GachaModel
    {
        private Dictionary<GachaType, List<GachaItem>> gachaTables;
        private Dictionary<GachaType, int> gachaCosts;
        
        public GachaModel()
        {
            InitializeGachaTables();
            InitializeGachaCosts();
        }
        
        private void InitializeGachaTables()
        {
            gachaTables = new Dictionary<GachaType, List<GachaItem>>();
            
            // 服ガチャテーブル
            gachaTables[GachaType.Beauty] = new List<GachaItem>
            {
                new GachaItem("オートクチュールのドレス", 10f, 150, GachaType.Beauty),
                new GachaItem("制服しか勝たん", 20f, 80, GachaType.Beauty),
                new GachaItem("量産型ガーリー", 20f, 35, GachaType.Beauty),
                new GachaItem("清潔感あるけど量販感", 30f, 15, GachaType.Beauty),
                new GachaItem("サイズ合ってない", 20f, 3, GachaType.Beauty)
            };
            
            // 家柄ガチャテーブル
            gachaTables[GachaType.FamilyWealth] = new List<GachaItem>
            {
                new GachaItem("一族で日本史に出てくる", 10f, 200, GachaType.FamilyWealth),
                new GachaItem("親が社長", 20f, 120, GachaType.FamilyWealth),
                new GachaItem("TVでよく見る", 20f, 80, GachaType.FamilyWealth),
                new GachaItem("上流階級", 30f, 40, GachaType.FamilyWealth),
                new GachaItem("一般庶民", 20f, 10, GachaType.FamilyWealth)
            };
            
            // 性格ガチャテーブル
            gachaTables[GachaType.Personality] = new List<GachaItem>
            {
                new GachaItem("天性の人たらし", 10f, 180, GachaType.Personality),
                new GachaItem("ツンデレの黄金比", 20f, 100, GachaType.Personality),
                new GachaItem("笑顔が多すぎて怖い", 20f, 60, GachaType.Personality),
                new GachaItem("無難すぎて覚えてもらえない", 30f, 25, GachaType.Personality),
                new GachaItem("第一印象が眠そう", 20f, 5, GachaType.Personality)
            };
        }
        
        private void InitializeGachaCosts()
        {
            gachaCosts = new Dictionary<GachaType, int>
            {
                [GachaType.Beauty] = 30000000,      // 3000万
                [GachaType.FamilyWealth] = 60000000, // 6000万
                [GachaType.Personality] = 20000000   // 2000万
            };
        }
        
        public GachaItem PullGacha(GachaType type)
        {
            var table = gachaTables[type];
            UnityEngine.Random.InitState((int)DateTime.Now.Ticks & 0x0000FFFF);
            float roll = UnityEngine.Random.Range(0f, 100f);
            float cumulative = 0f;
            
            foreach (var entry in table)
            {
                cumulative += entry.probability;
                if (roll <= cumulative)
                {
                    return entry;
                }
            }
            
            return table[table.Count - 1]; // フォールバック
        }
        
        public int GetGachaCost(GachaType type)
        {
            return gachaCosts[type];
        }
        
        public List<GachaItem> GetGachaTable(GachaType type)
        {
            return gachaTables[type];
        }
    }
}
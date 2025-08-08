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
            
            // 服ガチャテーブル (外見)
            gachaTables[GachaType.Beauty] = new List<GachaItem>
            {
                new GachaItem("オートクチュールのドレス", 5f, 150, 0, 0, 0, GachaType.Beauty),
                new GachaItem("制服しか勝たん", 15f, 80, 0, 0, 0, GachaType.Beauty),
                new GachaItem("量産型ガーリー", 30f, 40, 0, 0, 0, GachaType.Beauty),
                new GachaItem("清潔感あるけど量販感", 40f, 20, 0, 0, 0, GachaType.Beauty),
                new GachaItem("サイズ合ってない", 10f, 5, 0, 0, 0, GachaType.Beauty)
            };
            
            // 家柄ガチャテーブル (外見)
            gachaTables[GachaType.FamilyWealth] = new List<GachaItem>
            {
                new GachaItem("一族で日本史に出てくる", 5f, 200, 0, 0, 0, GachaType.FamilyWealth),
                new GachaItem("親が社長", 15f, 120, 0, 0, 0, GachaType.FamilyWealth),
                new GachaItem("TVでよく見る", 30f, 70, 0, 0, 0, GachaType.FamilyWealth),
                new GachaItem("上流階級", 40f, 30, 0, 0, 0, GachaType.FamilyWealth),
                new GachaItem("一般庶民", 10f, 10, 0, 0, 0, GachaType.FamilyWealth)
            };
            
            // 性格ガチャテーブル (外見)
            gachaTables[GachaType.Personality] = new List<GachaItem>
            {
                new GachaItem("天性の人たらし", 5f, 180, 0, 0, 0, GachaType.Personality),
                new GachaItem("ツンデレの黄金比", 15f, 100, 0, 0, 0, GachaType.Personality),
                new GachaItem("笑顔が多すぎて怖い", 30f, 50, 0, 0, 0, GachaType.Personality),
                new GachaItem("無難すぎて覚えてもらえない", 40f, 25, 0, 0, 0, GachaType.Personality),
                new GachaItem("第一印象が眠そう", 10f, 5, 0, 0, 0, GachaType.Personality)
            };

            // 運ガチャテーブル
            gachaTables[GachaType.Luck] = new List<GachaItem>
            {
                new GachaItem("女神の祝福", 5f, 0, 50, 0, 0, GachaType.Luck),
                new GachaItem("四葉のクローバー", 25f, 0, 15, 0, 0, GachaType.Luck),
                new GachaItem("幸運のコイン", 70f, 0, 5, 0, 0, GachaType.Luck)
            };

            // 集中力ガチャテーブル
            gachaTables[GachaType.Concentration] = new List<GachaItem>
            {
                new GachaItem("ゾーン体験", 5f, 0, 0, 60, 0, GachaType.Concentration),
                new GachaItem("精神統一の書", 25f, 0, 0, 20, 0, GachaType.Concentration),
                new GachaItem("アロマキャンドル", 70f, 0, 0, 8, 0, GachaType.Concentration)
            };

            // 優しさガチャテーブル
            gachaTables[GachaType.Kindness] = new List<GachaItem>
            {
                new GachaItem("聖母の微笑み", 5f, 0, 0, 0, 70, GachaType.Kindness),
                new GachaItem("天使の羽", 25f, 0, 0, 0, 25, GachaType.Kindness),
                new GachaItem("小さな親切", 70f, 0, 0, 0, 10, GachaType.Kindness)
            };
        }
        
        private void InitializeGachaCosts()
        {
            gachaCosts = new Dictionary<GachaType, int>
            {
                [GachaType.Beauty] = 30000000,      // 3000万
                [GachaType.FamilyWealth] = 60000000, // 6000万
                [GachaType.Personality] = 20000000,   // 2000万
                [GachaType.Luck] = 10000000,         // 1000万
                [GachaType.Concentration] = 10000000, // 1000万
                [GachaType.Kindness] = 10000000      // 1000万
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
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
                new GachaItem("オートクチュールスーツ", 5f, 150, 0, 0, 0, GachaType.Beauty),
                new GachaItem("ミニマルファッション", 15f, 80, 0, 0, 0, GachaType.Beauty),
                new GachaItem("カジュアルコーデ", 30f, 40, 0, 0, 0, GachaType.Beauty),
                new GachaItem("ベーシックな服装", 40f, 20, 0, 0, 0, GachaType.Beauty),
                new GachaItem("サイズ合ってない", 10f, 5, 0, 0, 0, GachaType.Beauty)
            };
            
            // 家柄ガチャテーブル
            gachaTables[GachaType.FamilyWealth] = new List<GachaItem>
            {
                new GachaItem("一族で日本史に出てくる", 5f, 200, 0, 0, 0, GachaType.FamilyWealth),
                new GachaItem("親が社長", 15f, 120, 0, 0, 0, GachaType.FamilyWealth),
                new GachaItem("TVでよく見る", 30f, 70, 0, 0, 0, GachaType.FamilyWealth),
                new GachaItem("上流階級", 40f, 30, 0, 0, 0, GachaType.FamilyWealth),
                new GachaItem("一般庶民", 10f, 10, 0, 0, 0, GachaType.FamilyWealth)
            };
            
            // 性格ガチャテーブル
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

            // 集中力ガチャテーブル - バズり要素追加
            gachaTables[GachaType.Concentration] = new List<GachaItem>
            {
                new GachaItem("神集中モード", 5f, 0, 0, 60, 0, GachaType.Concentration),
                new GachaItem("ポモドーロ最強", 25f, 0, 0, 20, 0, GachaType.Concentration),
                new GachaItem("ながらスマホ", 70f, 0, 0, 8, 0, GachaType.Concentration)
            };

            // 優しさガチャテーブル - バズり要素追加
            gachaTables[GachaType.Kindness] = new List<GachaItem>
            {
                new GachaItem("優しさの化身", 5f, 0, 0, 0, 70, GachaType.Kindness),
                new GachaItem("推しを守りたい", 25f, 0, 0, 0, 25, GachaType.Kindness),
                new GachaItem("普通にいい人", 70f, 0, 0, 0, 10, GachaType.Kindness)
            };

            // 童心ガチャテーブル (幼児期限定) - 優しさ特化
            gachaTables[GachaType.Childhood] = new List<GachaItem>
            {
                new GachaItem("永遠の純粋さ", 10f, 0, 0, 0, 80, GachaType.Childhood),        // 大当たり
                new GachaItem("天真爛漫", 25f, 0, 0, 0, 40, GachaType.Childhood),           // 高品質
                new GachaItem("好奇心旺盛", 35f, 0, 0, 0, 20, GachaType.Childhood),         // 普通
                new GachaItem("無邪気すぎる", 30f, 0, 0, 0, 5, GachaType.Childhood)          // 低品質
            };

            // 青春ガチャテーブル (青春期限定) - 運特化
            gachaTables[GachaType.Youth] = new List<GachaItem>
            {
                new GachaItem("青春の輝き", 10f, 0, 90, 0, 0, GachaType.Youth),              // 大当たり
                new GachaItem("情熱の炎", 25f, 0, 50, 0, 0, GachaType.Youth),               // 高品質
                new GachaItem("若さゆえの過ち", 35f, 0, 25, 0, 0, GachaType.Youth),          // 普通
                new GachaItem("青春の迷い", 30f, 0, 8, 0, 0, GachaType.Youth)               // 低品質
            };

            // キャリアガチャテーブル (就職期限定) - 集中力特化
            gachaTables[GachaType.Career] = new List<GachaItem>
            {
                new GachaItem("エリートの風格", 10f, 0, 0, 95, 0, GachaType.Career),           // 大当たり
                new GachaItem("プロフェッショナル", 25f, 0, 0, 55, 0, GachaType.Career),       // 高品質
                new GachaItem("仕事人間", 35f, 0, 0, 30, 0, GachaType.Career),                // 普通
                new GachaItem("出世への執着", 30f, 0, 0, 10, 0, GachaType.Career)             // 低品質
            };

            // 円熟ガチャテーブル (成熟期限定) - バランス型（2ステータス上昇）
            gachaTables[GachaType.Mature] = new List<GachaItem>
            {
                new GachaItem("人生経験の深み", 15f, 0, 40, 45, 0, GachaType.Mature),          // 大当たり：運+集中
                new GachaItem("大人の余裕", 25f, 0, 30, 0, 35, GachaType.Mature),             // 高品質：運+優しさ
                new GachaItem("中年の焦り", 35f, 0, 0, 25, 20, GachaType.Mature),             // 普通：集中+優しさ
                new GachaItem("保守的になる", 25f, 0, 15, 10, 0, GachaType.Mature)            // 低品質：運+集中
            };

            // 長老ガチャテーブル (シニア期限定) - 優しさ特化（高レベル）
            gachaTables[GachaType.Senior] = new List<GachaItem>
            {
                new GachaItem("老獪な知恵", 10f, 0, 0, 0, 120, GachaType.Senior),             // 大当たり
                new GachaItem("円熟した人格", 25f, 0, 0, 0, 70, GachaType.Senior),            // 高品質
                new GachaItem("頑固さ", 35f, 0, 0, 0, 35, GachaType.Senior),                 // 普通
                new GachaItem("老いの受容", 30f, 0, 0, 0, 15, GachaType.Senior)              // 低品質
            };

            // 伝説ガチャテーブル (大成功で解放) - 運特化（超高レベル）
            gachaTables[GachaType.Legendary] = new List<GachaItem>
            {
                new GachaItem("運命の選択", 15f, 0, 150, 0, 0, GachaType.Legendary),            // 大当たり
                new GachaItem("奇跡の瞬間", 25f, 0, 90, 0, 0, GachaType.Legendary),             // 高品質
                new GachaItem("星に願いを", 35f, 0, 50, 0, 0, GachaType.Legendary),            // 普通
                new GachaItem("光と影", 25f, 0, 20, 0, 0, GachaType.Legendary)                 // 低品質
            };

            // 逆転ガチャテーブル (大失敗で解放) - 集中力特化（高レベル）
            gachaTables[GachaType.Redemption] = new List<GachaItem>
            {
                new GachaItem("どん底からの這い上がり", 15f, 0, 0, 130, 0, GachaType.Redemption), // 大当たり
                new GachaItem("失敗が教えた真実", 25f, 0, 0, 80, 0, GachaType.Redemption),       // 高品質
                new GachaItem("諦めない心", 35f, 0, 0, 45, 0, GachaType.Redemption),            // 普通
                new GachaItem("孤独な戦い", 25f, 0, 0, 18, 0, GachaType.Redemption)             // 低品質
            };

            // 恋愛ガチャテーブル (恋愛イベント成功で解放) - 2ステータス型（中レベル）
            gachaTables[GachaType.Love] = new List<GachaItem>
            {
                new GachaItem("真実の愛", 15f, 0, 50, 0, 60, GachaType.Love),                  // 大当たり：運+優しさ
                new GachaItem("恋する心", 25f, 0, 35, 40, 0, GachaType.Love),                  // 高品質：運+集中
                new GachaItem("愛の盲目", 35f, 0, 0, 20, 25, GachaType.Love),                  // 普通：集中+優しさ
                new GachaItem("恋の駆け引き", 25f, 0, 15, 12, 0, GachaType.Love)              // 低品質：運+集中
            };

            // 30歳逆転ガチャテーブル (30歳で自動解放) - 外見も含む特別ガチャ
            gachaTables[GachaType.Reversal] = new List<GachaItem>
            {
                new GachaItem("奇跡の大逆転", 30f, 120, 90, 85, 0, GachaType.Reversal),     // 成功: 外見+運+集中（2ステータス）
                new GachaItem("ささやかな希望", 40f, 0, 0, 0, 0, GachaType.Reversal),         // 失敗（何も得られない）
                new GachaItem("更なる絶望", 30f, 0, -50, -45, 0, GachaType.Reversal)       // 大失敗（2ステータス減少）
            };
            
            // 呪いガチャテーブル（2ステータス減少特化）- 常時利用可能だが誰も使わない
            gachaTables[GachaType.Cursed] = new List<GachaItem>
            {
                new GachaItem("絶望の淵", 40f, 0, -60, -55, 0, GachaType.Cursed),          // 運+集中大幅ダウン
                new GachaItem("不運の連鎖", 30f, 0, -45, 0, 0, GachaType.Cursed),            // 運だけダウン
                new GachaItem("心の闇", 20f, 0, 0, -40, -35, GachaType.Cursed),                // 集中+優しさダウン
                new GachaItem("無気力", 10f, 0, 0, -50, 0, GachaType.Cursed)                 // 集中力だけダウン
            };
            
            // 学術ガチャテーブル（集中力超特化・高コスト）
            gachaTables[GachaType.Academic] = new List<GachaItem>
            {
                new GachaItem("博士号取得", 10f, 0, 0, 200, 0, GachaType.Academic),          // 超大当たり
                new GachaItem("論文発表", 20f, 0, 0, 120, 0, GachaType.Academic),            // 大当たり
                new GachaItem("研究発表", 35f, 0, 0, 70, 0, GachaType.Academic),             // 普通
                new GachaItem("図書館通い", 35f, 0, 0, 30, 0, GachaType.Academic)            // 低品質
            };
            
            // 社交ガチャテーブル（優しさ超特化・高コスト）
            gachaTables[GachaType.Social] = new List<GachaItem>
            {
                new GachaItem("カリスマ的人望", 10f, 0, 0, 0, 250, GachaType.Social),        // 超大当たり
                new GachaItem("天然のまとめ役", 20f, 0, 0, 0, 150, GachaType.Social),        // 大当たり
                new GachaItem("パーティーの中心", 35f, 0, 0, 0, 80, GachaType.Social),       // 普通
                new GachaItem("愛想笑い", 35f, 0, 0, 0, 35, GachaType.Social)                // 低品質
            };
            
            // ギャンブラーガチャテーブル（運超特化・高コスト）
            gachaTables[GachaType.Gambler] = new List<GachaItem>
            {
                new GachaItem("黄金の右手", 10f, 0, 300, 0, 0, GachaType.Gambler),          // 超大当たり
                new GachaItem("勝負師の勘", 20f, 0, 180, 0, 0, GachaType.Gambler),          // 大当たり
                new GachaItem("強運の持ち主", 35f, 0, 90, 0, 0, GachaType.Gambler),         // 普通
                new GachaItem("ビギナーズラック", 35f, 0, 40, 0, 0, GachaType.Gambler)      // 低品質
            };
        }
        
        private void InitializeGachaCosts()
        {
            gachaCosts = new Dictionary<GachaType, int>
            {
                // 基本ガチャ（魂片コスト）
                [GachaType.Beauty] = 2,             // 2魂片
                [GachaType.FamilyWealth] = 2,       // 2魂片
                [GachaType.Personality] = 2,        // 2魂片
                [GachaType.Luck] = 2,               // 2魂片
                [GachaType.Concentration] = 2,      // 2魂片
                [GachaType.Kindness] = 2,           // 2魂片
                
                // ライフステージ限定ガチャ
                [GachaType.Childhood] = 3,          // 3魂片（少し高め）
                [GachaType.Youth] = 3,              // 3魂片
                [GachaType.Career] = 3,             // 3魂片
                [GachaType.Mature] = 3,             // 3魂片
                [GachaType.Senior] = 3,             // 3魂片
                
                // 特別解放ガチャ
                [GachaType.Legendary] = 5,          // 5魂片（高価）
                [GachaType.Redemption] = 4,         // 4魂片
                [GachaType.Love] = 4,               // 4魂片
                // 30歳逆転ガチャ
                [GachaType.Reversal] = 10,          // 10魂片（ハイリスク・ハイリターン）
                
                // 使えない（超損）ガチャ
                [GachaType.Cursed] = 1,             // 1魂片（安いが超危険）
                [GachaType.Academic] = 20,          // 20魂片（超高コスト・集中力特化）
                [GachaType.Social] = 25,            // 25魂片（超高コスト・優しさ特化）
                [GachaType.Gambler] = 30            // 30魂片（最高コスト・運特化）
            };
        }
        
        public GachaItem PullGacha(GachaType type)
        {
            var table = gachaTables[type];
            UnityEngine.Random.InitState((int)DateTime.Now.Ticks & 0x0000FFFF);

            // ライフステージに応じた期待値バイアス
            // 目的: 序盤≈80% / 中盤≈65% / 終盤≈45% でイベント達成しやすい方向へ
            // アプローチ: 各アイテムの価値スコアに応じ確率ウェイトを補正し、再正規化
            var biasedWeights = GetBiasedWeights(table);

            float roll = UnityEngine.Random.Range(0f, 100f);
            float cumulative = 0f;
            for (int i = 0; i < table.Count; i++)
            {
                cumulative += biasedWeights[i];
                if (roll <= cumulative)
                {
                    return table[i];
                }
            }

            return table[table.Count - 1]; // フォールバック
        }

        // 各アイテムの価値を基に、年齢帯ごとのバイアスを適用して確率を再分配
        private List<float> GetBiasedWeights(List<GachaItem> table)
        {
            // 価値スコア: 正の上昇量（points/luck/concentration/kindness）の合計
            float maxVal = 0f;
            var values = new List<float>(table.Count);
            for (int i = 0; i < table.Count; i++)
            {
                var it = table[i];
                float v = 0f;
                if (it.points > 0) v += it.points;
                if (it.luck > 0) v += it.luck;
                if (it.concentration > 0) v += it.concentration;
                if (it.kindness > 0) v += it.kindness;
                values.Add(v);
                if (v > maxVal) maxVal = v;
            }

            // 正規化（全て0の場合は等確率のまま）
            var normalized = new List<float>(table.Count);
            if (maxVal <= 0f)
            {
                // 負やゼロばかり（呪い系など）の場合はそのまま
                for (int i = 0; i < table.Count; i++) normalized.Add(0f);
            }
            else
            {
                for (int i = 0; i < table.Count; i++) normalized.Add(values[i] / maxVal);
            }

            // ライフステージ別バイアス係数（経験的に調整）
            // 0-25: +0.50（高価値が出やすい）
            // 30-55: +0.15（やや高価値寄り）
            // 60+: -0.20（高価値が出にくい）
            int age = PlayerData.Instance?.CurrentAge ?? 0;
            float bias = 0f;
            if (age <= 25) bias = 0.50f;         // 序盤 ≈80%
            else if (age <= 55) bias = 0.15f;    // 中盤 ≈65%
            else bias = -0.20f;                  // 終盤 ≈45%

            // ウェイト補正: w' = p * (1 + bias * n)
            // nは価値の0..1正規化。負・ゼロ値は価値0として扱う。
            var raw = new List<float>(table.Count);
            float sum = 0f;
            for (int i = 0; i < table.Count; i++)
            {
                float p = table[i].probability;
                float n = normalized[i];
                float w = p * (1f + bias * n);
                raw.Add(w);
                sum += w;
            }

            // 100%に再正規化
            var weights = new List<float>(table.Count);
            if (sum <= 0f)
            {
                // 念のためフォールバック（元の確率を使用）
                for (int i = 0; i < table.Count; i++) weights.Add(table[i].probability);
                return weights;
            }

            float scale = 100f / sum;
            for (int i = 0; i < table.Count; i++) weights.Add(raw[i] * scale);
            return weights;
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

using System;
using System.Collections.Generic;

namespace HikukaHikanaika.Models
{
    [System.Serializable]
    public class LifeStageEvent
    {
        public int lifeCycle; // 0-15のライフサイクル
        public string eventTitle; // イベント名
        public string eventDescription; // イベント説明
        public string challengeText; // 挑戦時のテキスト
        public BattleStatusType requiredStatus; // 必要なステータス
        public int difficultyThreshold; // 成功に必要なポイント
        public string successText; // 成功時テキスト
        public string failureText; // 失敗時テキスト
        public EventReward successReward; // 成功時報酬
        public EventPenalty failurePenalty; // 失敗時ペナルティ
    }

    [System.Serializable]
    public class EventReward
    {
        public int lifespanBonus; // 寿命ボーナス
        public string description; // 報酬説明
    }

    [System.Serializable]
    public class EventPenalty
    {
        public int lifespanLoss; // 寿命ペナルティ
        public string description; // ペナルティ説明
    }

    public enum BattleStatusType
    {
        Family,     // 家柄
        Appearance, // 容姿
        Personality // 性格
    }

    public class LifeStageEventData
    {
        public static List<LifeStageEvent> GetAllEvents()
        {
            return new List<LifeStageEvent>
            {
                // 0-5歳: 幼児期
                new LifeStageEvent
                {
                    lifeCycle = 0,
                    eventTitle = "💝 家族の愛情",
                    eventDescription = "生まれたばかりのあなた。家族からどれだけ愛されるかが、今後の人生に大きく影響します。",
                    challengeText = "現在の家柄ステータスで家族の愛情を獲得しますか？",
                    requiredStatus = BattleStatusType.Family,
                    difficultyThreshold = 50,
                    successText = "温かい家族の愛に包まれ、安定した幼児期を過ごしました。",
                    failureText = "家族の愛情不足により、不安定な幼児期を過ごすことになりました。",
                    successReward = new EventReward { lifespanBonus = 3, description = "愛情により寿命+3年" },
                    failurePenalty = new EventPenalty { lifespanLoss = 1, description = "ストレスで寿命-1年" }
                },

                // 5-10歳: 児童期
                new LifeStageEvent
                {
                    lifeCycle = 1,
                    eventTitle = "🎒 学校デビュー",
                    eventDescription = "小学校での初日。クラスメイトとうまくやっていけるかが重要です。",
                    challengeText = "現在の性格ステータスで友達作りに挑戦しますか？",
                    requiredStatus = BattleStatusType.Personality,
                    difficultyThreshold = 40,
                    successText = "すぐにクラスの人気者になり、楽しい学校生活を送りました。",
                    failureText = "なかなか友達ができず、孤独な学校生活を過ごしました。",
                    successReward = new EventReward { lifespanBonus = 2, description = "友情による充実感で寿命+2年" },
                    failurePenalty = new EventPenalty { lifespanLoss = 1, description = "孤独感で寿命-1年" }
                },

                // 10-15歳: 思春期
                new LifeStageEvent
                {
                    lifeCycle = 2,
                    eventTitle = "💕 初恋の相手",
                    eventDescription = "思春期の訪れと共に、初恋の相手が現れました。告白する勇気はありますか？",
                    challengeText = "現在の容姿ステータスで初恋に挑戦しますか？",
                    requiredStatus = BattleStatusType.Appearance,
                    difficultyThreshold = 60,
                    successText = "甘酸っぱい初恋を経験し、青春を満喫しました。",
                    failureText = "初恋は実らず、恋愛に対して消極的になってしまいました。",
                    successReward = new EventReward { lifespanBonus = 3, description = "恋愛の喜びで寿命+3年" },
                    failurePenalty = new EventPenalty { lifespanLoss = 2, description = "失恋のショックで寿命-2年" }
                },

                // 15-20歳: 青春期
                new LifeStageEvent
                {
                    lifeCycle = 3,
                    eventTitle = "🎓 進路選択",
                    eventDescription = "高校卒業を控え、進路を決める重要な時期。理想の進路に進めるでしょうか？",
                    challengeText = "現在の総合ステータスで理想の進路獲得に挑戦しますか？",
                    requiredStatus = BattleStatusType.Personality,
                    difficultyThreshold = 80,
                    successText = "希望通りの進路に進み、充実した青春時代を過ごしました。",
                    failureText = "妥協した進路選択となり、将来への不安を抱えることになりました。",
                    successReward = new EventReward { lifespanBonus = 4, description = "希望達成で寿命+4年" },
                    failurePenalty = new EventPenalty { lifespanLoss = 2, description = "ストレスで寿命-2年" }
                },

                // 20-25歳: 就職期
                new LifeStageEvent
                {
                    lifeCycle = 4,
                    eventTitle = "💼 就職活動",
                    eventDescription = "社会人デビューの時。理想の会社に就職できるかが人生を大きく左右します。",
                    challengeText = "現在のステータスで夢の企業への就職に挑戦しますか？",
                    requiredStatus = BattleStatusType.Personality,
                    difficultyThreshold = 100,
                    successText = "憧れの企業に就職し、やりがいのある仕事に就けました。",
                    failureText = "希望とは違う職に就き、日々に不満を感じるようになりました。",
                    successReward = new EventReward { lifespanBonus = 5, description = "やりがいで寿命+5年" },
                    failurePenalty = new EventPenalty { lifespanLoss = 3, description = "職場ストレスで寿命-3年" }
                },

                // 25-30歳: 成人期
                new LifeStageEvent
                {
                    lifeCycle = 5,
                    eventTitle = "💒 結婚相手",
                    eventDescription = "理想のパートナーとの出会い。一生を共にする相手を見つけられるでしょうか？",
                    challengeText = "現在の容姿ステータスで理想の結婚相手獲得に挑戦しますか？",
                    requiredStatus = BattleStatusType.Appearance,
                    difficultyThreshold = 120,
                    successText = "素晴らしいパートナーと結婚し、幸せな家庭を築きました。",
                    failureText = "理想の相手は見つからず、独身のまま過ごすことになりました。",
                    successReward = new EventReward { lifespanBonus = 6, description = "愛情と安定で寿命+6年" },
                    failurePenalty = new EventPenalty { lifespanLoss = 2, description = "孤独感で寿命-2年" }
                },

                // 30-35歳: 責任期
                new LifeStageEvent
                {
                    lifeCycle = 6,
                    eventTitle = "👨‍💼 昇進チャンス",
                    eventDescription = "重要なプロジェクトのリーダーに抜擢されました。成功すれば大きな昇進が待っています。",
                    challengeText = "現在の性格ステータスで昇進に挑戦しますか？",
                    requiredStatus = BattleStatusType.Personality,
                    difficultyThreshold = 140,
                    successText = "プロジェクトは大成功！重要なポストに昇進しました。",
                    failureText = "プロジェクトは失敗...昇進の機会を逃してしまいました。",
                    successReward = new EventReward { lifespanBonus = 4, description = "達成感で寿命+4年" },
                    failurePenalty = new EventPenalty { lifespanLoss = 3, description = "失敗の重圧で寿命-3年" }
                },

                // 35-40歳: 充実期
                new LifeStageEvent
                {
                    lifeCycle = 7,
                    eventTitle = "🏠 マイホーム購入",
                    eventDescription = "念願のマイホーム購入のチャンス。理想の住まいを手に入れられるでしょうか？",
                    challengeText = "現在の家柄ステータスで理想の家購入に挑戦しますか？",
                    requiredStatus = BattleStatusType.Family,
                    difficultyThreshold = 160,
                    successText = "理想の家を購入し、充実した生活を送っています。",
                    failureText = "予算の関係で妥協した家になり、不満を感じています。",
                    successReward = new EventReward { lifespanBonus = 5, description = "安住の地で寿命+5年" },
                    failurePenalty = new EventPenalty { lifespanLoss = 2, description = "住環境ストレスで寿命-2年" }
                },

                // 40-45歳: 中年期
                new LifeStageEvent
                {
                    lifeCycle = 8,
                    eventTitle = "🌟 人生の転機",
                    eventDescription = "中年期の危機。このまま現状維持か、新たな挑戦をするか重要な選択の時です。",
                    challengeText = "現在の総合ステータスで新しい挑戦をしますか？",
                    requiredStatus = BattleStatusType.Personality,
                    difficultyThreshold = 180,
                    successText = "新たな挑戦は成功し、人生に新しい意味を見出しました。",
                    failureText = "挑戦は失敗に終わり、現状維持のまま時が過ぎました。",
                    successReward = new EventReward { lifespanBonus = 7, description = "新たな目標で寿命+7年" },
                    failurePenalty = new EventPenalty { lifespanLoss = 3, description = "後悔と諦めで寿命-3年" }
                },

                // 45-50歳: 成熟期
                new LifeStageEvent
                {
                    lifeCycle = 9,
                    eventTitle = "👪 家族との絆",
                    eventDescription = "家族関係の重要性を感じる時期。良好な関係を築けているでしょうか？",
                    challengeText = "現在の性格ステータスで家族の絆を深めますか？",
                    requiredStatus = BattleStatusType.Personality,
                    difficultyThreshold = 160,
                    successText = "家族との絆は深まり、温かい関係を築けています。",
                    failureText = "家族との関係はぎくしゃくし、距離を感じています。",
                    successReward = new EventReward { lifespanBonus = 5, description = "家族の支えで寿命+5年" },
                    failurePenalty = new EventPenalty { lifespanLoss = 4, description = "家族問題のストレスで寿命-4年" }
                },

                // 50-55歳: 転換期
                new LifeStageEvent
                {
                    lifeCycle = 10,
                    eventTitle = "💡 新事業立ち上げ",
                    eventDescription = "長年の経験を活かし、独立起業のチャンスが訪れました。",
                    challengeText = "現在の総合ステータスで起業に挑戦しますか？",
                    requiredStatus = BattleStatusType.Family,
                    difficultyThreshold = 200,
                    successText = "事業は軌道に乗り、第二の人生が始まりました。",
                    failureText = "事業は失敗し、大きな損失を被りました。",
                    successReward = new EventReward { lifespanBonus = 8, description = "成功の喜びで寿命+8年" },
                    failurePenalty = new EventPenalty { lifespanLoss = 5, description = "失敗の重圧で寿命-5年" }
                },

                // 55-60歳: 安定期
                new LifeStageEvent
                {
                    lifeCycle = 11,
                    eventTitle = "🎨 趣味の世界",
                    eventDescription = "これまで時間がなくてできなかった趣味に本格的に取り組む時期です。",
                    challengeText = "現在のステータスで趣味の世界で成果を出しますか？",
                    requiredStatus = BattleStatusType.Personality,
                    difficultyThreshold = 140,
                    successText = "趣味の世界で認められ、生きがいを見つけました。",
                    failureText = "趣味も長続きせず、退屈な日々を過ごしています。",
                    successReward = new EventReward { lifespanBonus = 4, description = "生きがいで寿命+4年" },
                    failurePenalty = new EventPenalty { lifespanLoss = 2, description = "無気力で寿命-2年" }
                },

                // 60-65歳: 準備期
                new LifeStageEvent
                {
                    lifeCycle = 12,
                    eventTitle = "🎓 次世代への教育",
                    eventDescription = "これまでの経験を若い世代に伝える機会が訪れました。",
                    challengeText = "現在の知識で若者を指導しますか？",
                    requiredStatus = BattleStatusType.Personality,
                    difficultyThreshold = 160,
                    successText = "多くの若者を育て、社会に貢献することができました。",
                    failureText = "若い世代とのギャップを感じ、上手く指導できませんでした。",
                    successReward = new EventReward { lifespanBonus = 6, description = "社会貢献の満足感で寿命+6年" },
                    failurePenalty = new EventPenalty { lifespanLoss = 2, description = "世代ギャップのストレスで寿命-2年" }
                },

                // 65-70歳: シニア期
                new LifeStageEvent
                {
                    lifeCycle = 13,
                    eventTitle = "🌅 健康管理",
                    eventDescription = "健康が気になる年齢。今後の健康維持が重要になってきました。",
                    challengeText = "現在のライフスタイルで健康維持に取り組みますか？",
                    requiredStatus = BattleStatusType.Personality,
                    difficultyThreshold = 120,
                    successText = "健康的な生活を続け、元気なシニアライフを送っています。",
                    failureText = "健康管理が上手くいかず、体調を崩しがちです。",
                    successReward = new EventReward { lifespanBonus = 5, description = "健康維持で寿命+5年" },
                    failurePenalty = new EventPenalty { lifespanLoss = 4, description = "健康問題で寿命-4年" }
                },

                // 70-75歳: 長老期
                new LifeStageEvent
                {
                    lifeCycle = 14,
                    eventTitle = "📚 人生の記録",
                    eventDescription = "これまでの人生を振り返り、自叙伝を書く機会が訪れました。",
                    challengeText = "現在の経験値で人生の記録を残しますか？",
                    requiredStatus = BattleStatusType.Personality,
                    difficultyThreshold = 180,
                    successText = "素晴らしい自叙伝が完成し、多くの人に感動を与えました。",
                    failureText = "思うような記録は残せませんでしたが、それも人生です。",
                    successReward = new EventReward { lifespanBonus = 4, description = "達成感で寿命+4年" },
                    failurePenalty = new EventPenalty { lifespanLoss = 1, description = "軽い後悔で寿命-1年" }
                },

                // 75-80歳: 晩年期
                new LifeStageEvent
                {
                    lifeCycle = 15,
                    eventTitle = "🕊️ 人生の総括",
                    eventDescription = "長い人生の最終章。あなたはどのような人生を歩んだのでしょうか？",
                    challengeText = "これまでの人生の総括をしますか？",
                    requiredStatus = BattleStatusType.Personality,
                    difficultyThreshold = 200,
                    successText = "充実した人生だったと心から思えます。多くの人に愛され、安らかな気持ちです。",
                    failureText = "後悔も多い人生でしたが、それでも生きてこられて良かったと思います。",
                    successReward = new EventReward { lifespanBonus = 3, description = "人生の満足感で寿命+3年" },
                    failurePenalty = new EventPenalty { lifespanLoss = 0, description = "後悔はあるが人生を受け入れる" }
                }
            };
        }
    }
}
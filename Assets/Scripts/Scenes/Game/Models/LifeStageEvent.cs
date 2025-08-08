using System.Collections.Generic;

namespace HikukaHikanaika.Models
{
    [System.Serializable]
    public class LifeStageEvent
    {
        public int lifeCycle; // 0-15のライフサイクル
        public string eventTitle; // イベント名
        public string eventDescription; // イベント説明
        public BattleStatusType requiredStatus; // 必要なステータス
        public int difficultyThreshold; // 成功に必要なポイント

        // 結果を4段階に細分化
        public EventResult greatSuccess; // 大成功
        public EventResult success;      // 成功
        public EventResult failure;      // 失敗
        public EventResult greatFailure; // 大失敗
    }

    [System.Serializable]
    public class EventResult
    {
        public string text; // 結果表示テキスト
        public int tenmeiChange; // 天命の変動 (+で増加, -で減少)
        public string description; // 報酬/ペナルティの説明
    }

    public enum BattleStatusType
    {
        Family,     // 家柄
        Appearance, // 容姿
        Personality, // 性格
        Luck, // 運
        Concentration, // 集中力
        Kindness // 優しさ
    }

    public class LifeStageEventData
    {
        public static List<LifeStageEvent> GetAllEvents()
        {
            return new List<LifeStageEvent>
            {
                // 0-5歳: 幼児期 (優しさ)
                new LifeStageEvent
                {
                    lifeCycle = 0,
                    eventTitle = "💝 小さな親切",
                    eventDescription = "公園で転んだ友達。あなたは優しく手を差し伸べることができますか？",
                    requiredStatus = BattleStatusType.Kindness,
                    difficultyThreshold = 15,
                    greatSuccess = new EventResult { text = "あなたの純粋な優しさは、友達だけでなく、周りの大人たちの心も温めました。", tenmeiChange = 2, description = "人徳が上がり、天命+2" },
                    success = new EventResult { text = "あなたの優しさに友達は感謝し、二人は親友になりました。", tenmeiChange = 1, description = "心の成長で天命+1" },
                    failure = new EventResult { text = "見て見ぬふりをしたことで、少しだけ罪悪感を覚えました。", tenmeiChange = 0, description = "小さな後悔" },
                    greatFailure = new EventResult { text = "突き放したあなたの態度に、友達は深く傷ついてしまいました。", tenmeiChange = -1, description = "ちょっと気まずくなり天命-1" }
                },

                // 5-10歳: 児童期 (理不尽な高難易度イベント)
                new LifeStageEvent
                {
                    lifeCycle = 1,
                    eventTitle = "🎒 謎の転校生",
                    eventDescription = "全てが完璧な謎の転校生がやってきた。なぜかあなたに勝負を挑んでくる！",
                    requiredStatus = BattleStatusType.Appearance,
                    difficultyThreshold = 150,
                    greatSuccess = new EventResult { text = "誰もが驚くことに、あなたは謎の転校生に勝利した！あなたの名は伝説となった。", tenmeiChange = 10, description = "若くして伝説となり、天命+10" },
                    success = new EventResult { text = "奇跡的に、転校生と引き分けることができた。", tenmeiChange = 1, description = "自信がつき、天命+1" },
                    failure = new EventResult { text = "転校生に完膚なきまでに叩きのめされた...。", tenmeiChange = -3, description = "悔しくて三日三晩泣き続け、天命-3" },
                    greatFailure = new EventResult { text = "彼の圧倒的な力の前に、あなたの心は折れてしまった。", tenmeiChange = -5, description = "恥ずかしいあだ名をつけられ、天命-5" }
                },

                // 10-15歳: 思春期 (運)
                new LifeStageEvent
                {
                    lifeCycle = 2,
                    eventTitle = "テストのヤマ勘",
                    eventDescription = "明日は大事なテスト。一夜漬けのヤマ勘は当たるのか？",
                    requiredStatus = BattleStatusType.Luck,
                    difficultyThreshold = 40,
                    greatSuccess = new EventResult { text = "ヤマが完璧に的中！学年トップの成績を収め、ヒーローになりました。", tenmeiChange = 5, description = "強運の持ち主として天命+5" },
                    success = new EventResult { text = "見事にヤマが当たり、テストは高得点！", tenmeiChange = 2, description = "幸運に感謝して天命+2" },
                    failure = new EventResult { text = "ヤマは外れ、テスト結果は散々でした。", tenmeiChange = -1, description = "現実の厳しさを知り天命-1" },
                    greatFailure = new EventResult { text = "ヤマが全く当たらず、赤点を取ってしまいました。", tenmeiChange = -3, description = "補習地獄で夏休みがなくなり天命-3" }
                },

                // 15-20歳: 青春期 (集中力)
                new LifeStageEvent
                {
                    lifeCycle = 3,
                    eventTitle = "受験戦争",
                    eventDescription = "人生を左右する大学受験。あなたは集中して勉強に取り組めるか？",
                    requiredStatus = BattleStatusType.Concentration,
                    difficultyThreshold = 100,
                    greatSuccess = new EventResult { text = "驚異的な集中力で、見事第一志望の大学に合格した！", tenmeiChange = 7, description = "輝かしい未来が約束され、天命+7" },
                    success = new EventResult { text = "努力が実り、無事に大学に合格。キャンパスライフが始まる。", tenmeiChange = 3, description = "達成感で天命+3" },
                    failure = new EventResult { text = "受験に失敗。滑り止めの大学に行くことになった...。", tenmeiChange = -4, description = "不本意な進路にやる気をなくし天命-4" },
                    greatFailure = new EventResult { text = "全ての大学に落ちた...。しかし、進学した専門学校であなたの隠れた才能が爆発！その道のカリスマとなった。", tenmeiChange = 10, description = "我が道を見出し、天命+10" }
                },

                // 20-25歳: 就職期 (笑える失敗)
                new LifeStageEvent
                {
                    lifeCycle = 4,
                    eventTitle = "💼 黒歴史な自己PR",
                    eventDescription = "最終面接。ウケを狙って、渾身の自己PRを披露する！",
                    requiredStatus = BattleStatusType.Personality,
                    difficultyThreshold = 180,
                    greatSuccess = new EventResult { text = "あなたの自己PRが社長に大ウケ！『面白い人材だ』と即採用が決まった！", tenmeiChange = 15, description = "コミュ力で世界を掴み、天命+15" },
                    success = new EventResult { text = "自己PRはスベったが、真面目さが評価され、無事内定を得た。", tenmeiChange = 8, description = "安定した職を得て天命+8" },
                    failure = new EventResult { text = "面接官はドン引き。気まずい空気のまま面接は終わった...。", tenmeiChange = -10, description = "思い出したくない黒歴史が生まれ、天命-10" },
                    greatFailure = new EventResult { text = "あなたの自己PRはネットで晒され、伝説の『ヤバい奴』として有名になってしまった。", tenmeiChange = -20, description = "デジタルタトゥーの恐怖に震え、天命-20" }
                },

                // 25-30歳: 成人期 (家柄)
                new LifeStageEvent
                {
                    lifeCycle = 5,
                    eventTitle = "💒 運命の結婚",
                    eventDescription = "生涯のパートナーを決める時。あなたの家柄は、相手にどう映るでしょうか？",
                    requiredStatus = BattleStatusType.Family,
                    difficultyThreshold = 120,
                    greatSuccess = new EventResult { text = "あなたの素晴らしい家柄が決め手となり、玉の輿に乗りました！", tenmeiChange = 8, description = "盤石な生活基盤を得て天命+8" },
                    success = new EventResult { text = "良いご縁に恵まれ、幸せな結婚生活が始まりました。", tenmeiChange = 5, description = "心の安らぎを得て天命+5" },
                    failure = new EventResult { text = "家柄の違いから、結婚に反対されてしまいました。", tenmeiChange = -3, description = "破談のショックで天命-3" },
                    greatFailure = new EventResult { text = "あなたの家柄を知った相手は、静かに去っていきました。", tenmeiChange = -6, description = "埋められない格差に絶望し天命-6" }
                },

                // 30-35歳: 責任期 (優しさ)
                new LifeStageEvent
                {
                    lifeCycle = 6,
                    eventTitle = "🤝 人望",
                    eventDescription = "部下や後輩から、あなたはどれだけ慕われているでしょうか？",
                    requiredStatus = BattleStatusType.Kindness,
                    difficultyThreshold = 130,
                    greatSuccess = new EventResult { text = "あなたの優しさは伝説となり、多くの人があなたを心から尊敬しています。", tenmeiChange = 8, description = "揺るぎない人望を得て天命+8" },
                    success = new EventResult { text = "部下から慕われ、チームは最高の成果を上げました。", tenmeiChange = 4, description = "良好な人間関係が活力となり天命+4" },
                    failure = new EventResult { text = "部下との間に溝ができ、チームの雰囲気は最悪です。", tenmeiChange = -2, description = "人間関係のストレスで天命-2" },
                    greatFailure = new EventResult { text = "あなたの厳しい態度に、部下全員が辞めてしまいました。", tenmeiChange = -6, description = "孤独な管理職となり天命-6" }
                },

                // 35-40歳: 充実期 (性格)
                new LifeStageEvent
                {
                    lifeCycle = 7,
                    eventTitle = "🏠 マイホーム購入",
                    eventDescription = "念願のマイホーム。しかし、隣人との関係が重要になる。",
                    requiredStatus = BattleStatusType.Personality,
                    difficultyThreshold = 160,
                    greatSuccess = new EventResult { text = "隣人ともすぐに打ち解け、地域で最も愛される家族になりました。", tenmeiChange = 7, description = "最高のコミュニティに恵まれ天命+7" },
                    success = new EventResult { text = "ご近所付き合いもそつなくこなし、快適なマイホーム生活を送っています。", tenmeiChange = 3, description = "安住の地を得て天命+3" },
                    failure = new EventResult { text = "隣人トラブルに巻き込まれ、心休まる日がありません。", tenmeiChange = -4, description = "ご近所ストレスで天命-4" },
                    greatFailure = new EventResult { text = "あなたは地域から孤立し、マイホームは安らぎの場ではなくなりました。", tenmeiChange = -9, description = "安住の地を失い天命-9" }
                },

                // 40-45歳: 中年期 (笑える大失敗)
                new LifeStageEvent
                {
                    lifeCycle = 8,
                    eventTitle = "💍 世紀のプロポーズ",
                    eventDescription = "人生を賭けたプロポーズ！フラッシュモブを企画し、彼女を驚かせよう！",
                    requiredStatus = BattleStatusType.Luck,
                    difficultyThreshold = 250,
                    greatSuccess = new EventResult { text = "プロポーズは大成功！動画は世界中に拡散され、あなたは『愛の伝道師』と呼ばれた！", tenmeiChange = 30, description = "世界中から祝福され、天命+30" },
                    success = new EventResult { text = "プロポーズは成功したが、フラッシュモブは彼女に少し引かれた。", tenmeiChange = 2, description = "何はともあれ幸せになり、天命+2" },
                    failure = new EventResult { text = "盛大にスベった...。プロポーズは保留になった。", tenmeiChange = -10, description = "恥ずかしさで1年間寝込み、天命-10" },
                    greatFailure = new EventResult { text = "『あなたじゃない、そこのダンサーの人が好き』...あなたは盛大にフラれた。", tenmeiChange = -30, description = "世紀の勘違いとして歴史に名を刻み、天命-30" }
                },
                
                // 45-50歳: 成熟期 (集中力)
                new LifeStageEvent
                {
                    lifeCycle = 9,
                    eventTitle = "🎨 趣味の世界",
                    eventDescription = "長年続けた趣味。個展を開き、世に作品を問う時が来た。",
                    requiredStatus = BattleStatusType.Concentration,
                    difficultyThreshold = 200,
                    greatSuccess = new EventResult { text = "あなたの作品は世界的に評価され、歴史に名を残す芸術家となった。", tenmeiChange = 12, description = "生きがいがあなたを輝かせ、天命+12" },
                    success = new EventResult { text = "個展は成功し、あなたの作品は多くの人に感動を与えた。", tenmeiChange = 6, description = "趣味が認められ、天命+6" },
                    failure = new EventResult { text = "あなたの作品は、誰にも評価されなかった。", tenmeiChange = -3, description = "創作意欲を失い、天命-3" },
                    greatFailure = new EventResult { text = "「時間の無駄だった」と酷評され、あなたの心は深く傷ついた。", tenmeiChange = -7, description = "生きる意味を見失い、天命-7" }
                },

                // 50-55歳: 転換期 (家柄)
                new LifeStageEvent
                {
                    lifeCycle = 10,
                    eventTitle = "🏛️ 名誉職への推薦",
                    eventDescription = "あなたの家柄が評価され、名誉ある地位への推薦があった。",
                    requiredStatus = BattleStatusType.Family,
                    difficultyThreshold = 220,
                    greatSuccess = new EventResult { text = "あなたは一族の誇りとなり、歴史に名を刻むことになった。", tenmeiChange = 10, description = "最高の栄誉を得て、天命+10" },
                    success = new EventResult { text = "名誉職に就き、穏やかで尊敬される日々を送っている。", tenmeiChange = 5, description = "社会的な地位が安定し、天命+5" },
                    failure = new EventResult { text = "一族の不祥事が発覚し、推薦は取り消された。", tenmeiChange = -5, description = "一族の名に泥を塗り、天命-5" },
                    greatFailure = new EventResult { text = "あなたの家柄は、もはや何の価値も持たないと宣告された。", tenmeiChange = -10, description = "プライドが打ち砕かれ、天命-10" }
                },

                // 55-60歳: 安定期 (容姿)
                new LifeStageEvent
                {
                    lifeCycle = 11,
                    eventTitle = "✨ アンチエイジング",
                    eventDescription = "若々しさを保つための努力。その成果は現れるか。",
                    requiredStatus = BattleStatusType.Appearance,
                    difficultyThreshold = 180,
                    greatSuccess = new EventResult { text = "あなたの美貌は年齢を超越し、時の流れを止めたかのようです。", tenmeiChange = 8, description = "美の追求が実を結び天命+8" },
                    success = new EventResult { text = "若々しい見た目を保ち、充実した日々を送っています。", tenmeiChange = 4, description = "自信に満ちた生活で天命+4" },
                    failure = new EventResult { text = "年齢には勝てず、容姿の衰えを感じています。", tenmeiChange = -2, description = "老いへの不安で天命-2" },
                    greatFailure = new EventResult { text = "無理な若作りがたたり、かえって老け込んでしまいました。", tenmeiChange = -5, description = "心身の不調で天命-5" }
                },

                // 60-65歳: 準備期 (優しさ)
                new LifeStageEvent
                {
                    lifeCycle = 12,
                    eventTitle = "💖 地域への貢献",
                    eventDescription = "ボランティア活動への参加。あなたの優しさは地域を豊かにするか。",
                    requiredStatus = BattleStatusType.Kindness,
                    difficultyThreshold = 200,
                    greatSuccess = new EventResult { text = "あなたの活動が高く評価され、名誉市民の称号を授与されました。", tenmeiChange = 10, description = "社会への貢献が認められ天命+10" },
                    success = new EventResult { text = "地域の人々から感謝され、充実した日々を送っています。", tenmeiChange = 5, description = "人との繋がりに感謝し天mei+5" },
                    failure = new EventResult { text = "自己満足の活動となり、誰からも感謝されませんでした。", tenmeiChange = -2, description = "孤独感で天命-2" },
                    greatFailure = new EventResult { text = "良かれと思ってしたことが、大きなトラブルに発展してしまいました。", tenmeiChange = -8, description = "人間不信に陥り天命-8" }
                },

                // 65-70歳: シニア期 (笑える失敗)
                new LifeStageEvent
                {
                    lifeCycle = 13,
                    eventTitle = "📱 若者文化への挑戦",
                    eventDescription = "孫にウケたい一心で、流行りのSNSに挑戦！果たして『いいね』はもらえるか？",
                    requiredStatus = BattleStatusType.Luck,
                    difficultyThreshold = 280,
                    greatSuccess = new EventResult { text = "あなたの投稿が謎のアルゴリズムに乗り、世界的な大バズりを記録した！", tenmeiChange = 15, description = "インフルエンサーとして第二の人生が始まり、天命+15" },
                    success = new EventResult { text = "孫やその友達にウケて、少しだけ人気者になれた。", tenmeiChange = 7, description = "若者との交流が刺激になり、天命+7" },
                    failure = new EventResult { text = "あなたの投稿は、誰にも見向きもされなかった...。", tenmeiChange = -15, description = "時代の流れについていけず、天命-15" },
                    greatFailure = new EventResult { text = "あなたの投稿が『痛い』とネットニュースになり、炎上してしまった。", tenmeiChange = -40, description = "ネットリンチの恐怖に震え、天命-40" }
                },

                // 70-75歳: 長老期 (家柄)
                new LifeStageEvent
                {
                    lifeCycle = 14,
                    eventTitle = "📚 自叙伝の出版",
                    eventDescription = "あなたの人生を綴った本。一族の名誉を高めることができるか。",
                    requiredStatus = BattleStatusType.Family,
                    difficultyThreshold = 250,
                    greatSuccess = new EventResult { text = "自叙伝は世界的なベストセラーとなり、一族は永遠の栄光を手に入れた。", tenmeiChange = 12, description = "歴史に名を刻み、天命+12" },
                    success = new EventResult { text = "あなたの人生は多くの人に感銘を与え、一族の誇りとなった。", tenmeiChange = 6, description = "人生の集大成が認められ、天命+6" },
                    failure = new EventResult { text = "自叙伝は誰にも読まれず、静かに忘れ去られた。", tenmeiChange = -4, description = "過去の栄光が色褪せ、天命-4" },
                    greatFailure = new EventResult { text = "一族の恥を晒したと、激しい非難を浴びた。", tenmeiChange = -9, description = "晩節を汚し、天命-9" }
                },

                // 75-80歳: 晩年期 (性格)
                new LifeStageEvent
                {
                    lifeCycle = 15,
                    eventTitle = "🕊️ 人生の総括",
                    eventDescription = "長い人生の最終章。あなたはどのような人間として記憶されるのでしょうか？",
                    requiredStatus = BattleStatusType.Personality,
                    difficultyThreshold = 300,
                    greatSuccess = new EventResult { text = "あなたの人生は伝説となり、後世まで語り継がれるでしょう。", tenmeiChange = 20, description = "完璧な人生を全うし天命+20" },
                    success = new EventResult { text = "多くの人に愛され、穏やかで幸せな最期を迎えることができました。", tenmeiChange = 10, description = "満足感に包まれ天命+10" },
                    failure = new EventResult { text = "後悔の念に苛まれながら、静かに人生の終わりを待ちます。", tenmeiChange = -5, description = "満たされない思いが天命を削る" },
                    greatFailure = new EventResult { text = "誰からも看取られることなく、孤独な最期を迎えました。", tenmeiChange = -20, description = "虚無感の中で天命が尽きる" }
                }
            };
        }
    }
}
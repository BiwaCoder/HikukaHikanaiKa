using UnityEngine;
using HikukaHikanaika.Models;
using System;

namespace HikukaHikanaika.Logic
{
    public class GachaLogic
    {
        private static GachaLogic instance;
        public static GachaLogic Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GachaLogic();
                }
                return instance;
            }
        }
        
        private GachaModel gachaModel;
        
        private GachaLogic()
        {
            gachaModel = new GachaModel();
        }
        
        public static void Initialize()
        {
            instance = new GachaLogic();
        }
        
        public static void Reset()
        {
            instance = null;
        }
        
        public PlayerData PlayerData => PlayerData.Instance;
        
        public bool CanPerformGacha(GachaType gachaType)
        {
            // ガチャタイプ別コスト取得
            int cost = gachaModel.GetGachaCost(gachaType);
            
            // 基本的な魂片チェック
            if (!PlayerData.CanAffordTenmei(cost))
                return false;
                
            // ガチャ解放状態チェック
            if (!PlayerData.IsGachaUnlocked(gachaType))
                return false;
                
            // ライフステージ限定ガチャの年齢制限チェック
            if (!PlayerData.IsLifestageGachaAvailable(gachaType))
                return false;
                
            // 一回限りガチャの使用済みチェック
            if (PlayerData.IsOnceOnlyGacha(gachaType) && PlayerData.HasUsedOnceOnlyGacha(gachaType))
                return false;
                
            return true;
        }
        
        public string PerformGacha(GachaType gachaType)
        {
            // ガチャタイプ別のコスト取得
            int cost = gachaModel.GetGachaCost(gachaType);
            
            if (PlayerData.IsGameOver())
            {
                return "💀 \"あら♡ 魂片がゼロになっちゃったのね。約束通り、あなたの全てはわたしのもの♪\" - 小悪魔天使より";
            }
            
            if (!CanPerformGacha(gachaType))
            {
                if (!PlayerData.IsGachaUnlocked(gachaType))
                {
                    return "🔒 このガチャはまだ解放されていません";
                }
                if (!PlayerData.IsLifestageGachaAvailable(gachaType))
                {
                    return "⏰ この年齢ではもうこのガチャは引けません";
                }
                if (PlayerData.IsOnceOnlyGacha(gachaType) && PlayerData.HasUsedOnceOnlyGacha(gachaType))
                {
                    return "✋ この伝説ガチャはもう使用済みです";
                }
                return $"⏳ \"あら、魂片が足りないのね♡ もっと魂を削らなきゃダメよ？\" 残り{PlayerData.RemainingTenmei}片（必要：{cost}片） - 小悪魔天使";
            }
            
            PlayerData.SpendTenmei(cost);
            GachaItem pulledItem = gachaModel.PullGacha(gachaType);
            
            // 一回限りガチャの場合は使用済みマーク
            PlayerData.MarkOnceOnlyGachaAsUsed(gachaType);
            
            string result = ProcessGachaResult(pulledItem, gachaType);
            
            // ガチャ後に魂片チェック
            if (PlayerData.IsGameOver())
            {
                result += "\n\n💀 \"あら♡ ついに魂片がゼロに...あなたの人生、とっても美味しそうだったわ♪\"";
            }
            
            return result;
        }
        
        private string ProcessGachaResult(GachaItem item, GachaType gachaType)
        {
            // ステータス直接上昇ガチャの処理
            if (IsStatusDirectGacha(gachaType))
            {
                return UpdateStatus(item, gachaType);
            }

            // 複合ステータスガチャの処理（新しいライフステージ・特別ガチャ）
            if (IsMultiStatusGacha(gachaType))
            {
                return UpdateMultipleStatus(item, gachaType);
            }

            // 従来の装備ガチャの処理
            if (PlayerData.HasItem(item.name, gachaType))
            {
                return $"🎴 結果: {item.name}\n🔄 既に所持しています。";
            }
            
            PlayerData.AddItem(item.name, gachaType);
            return UpdateCurrentEquipment(item, gachaType);
        }
        
        private bool IsStatusDirectGacha(GachaType gachaType)
        {
            return gachaType == GachaType.Luck || 
                   gachaType == GachaType.Concentration || 
                   gachaType == GachaType.Kindness;
        }
        
        private bool IsMultiStatusGacha(GachaType gachaType)
        {
            return gachaType == GachaType.Childhood || 
                   gachaType == GachaType.Youth ||
                   gachaType == GachaType.Career ||
                   gachaType == GachaType.Mature ||
                   gachaType == GachaType.Senior ||
                   gachaType == GachaType.Legendary ||
                   gachaType == GachaType.Redemption ||
                   gachaType == GachaType.Love ||
                   gachaType == GachaType.Reversal ||
                   gachaType == GachaType.Cursed ||
                   gachaType == GachaType.Academic ||
                   gachaType == GachaType.Social ||
                   gachaType == GachaType.Gambler;
        }
        
        private string UpdateMultipleStatus(GachaItem item, GachaType gachaType)
        {
            // 逆転ガチャの特別処理
            if (gachaType == GachaType.Reversal)
            {
                return ProcessReversalGacha(item);
            }
            
            // 複数ステータスを同時に上昇
            if (item.points > 0) {
                // 外見系は装備として扱う
                if (PlayerData.CurrentOutfit == null || item.points > PlayerData.CurrentOutfit.points) {
                    PlayerData.CurrentOutfit = item;
                }
            }
            
            PlayerData.Luck += item.luck;
            PlayerData.Concentration += item.concentration;
            PlayerData.Kindness += item.kindness;
            
            string gachaName = GetGachaDisplayName(gachaType);
            string statusChanges = "";
            
            if (item.points != 0) statusChanges += $"\n👗 外見 {(item.points > 0 ? "+" : "")}{item.points}";
            if (item.luck != 0) statusChanges += $"\n🍀 運 {(item.luck > 0 ? "+" : "")}{item.luck}";
            if (item.concentration != 0) statusChanges += $"\n🎯 集中力 {(item.concentration > 0 ? "+" : "")}{item.concentration}";
            if (item.kindness != 0) statusChanges += $"\n💖 優しさ {(item.kindness > 0 ? "+" : "")}{item.kindness}";
            
            return $"【{gachaName}結果】\n『{item.name}』を引いた！\n\n{GetGachaFlavorText(gachaType)}{statusChanges}\n\n人生にさらなる変化が起きました！";
        }
        
        private string ProcessReversalGacha(GachaItem item)
        {
            string resultMessage = "【🎯 逆転ガチャ結果】\n";
            
            if (item.name == "奇跡の大逆転")
            {
                // 成功: 全ステータス大幅アップ
                if (item.points > 0 && (PlayerData.CurrentOutfit == null || item.points > PlayerData.CurrentOutfit.points))
                {
                    PlayerData.CurrentOutfit = item;
                }
                PlayerData.Luck += item.luck;
                PlayerData.Concentration += item.concentration;
                PlayerData.Kindness += item.kindness;
                
                resultMessage += $"『{item.name}』\n\n";
                resultMessage += "🌟 これまでの挫折が全て報われた！\n人生の歯車が一気に回り出す！\n\n";
                resultMessage += $"👗 外見 +{item.points}\n🍀 運 +{item.luck}\n🎯 集中力 +{item.concentration}\n💖 優しさ +{item.kindness}\n\n";
                resultMessage += "✨ 今こそが真の出発点です！";
            }
            else if (item.name == "ささやかな希望") 
            {
                // 失敗: 何も得られず魂片だけ減る
                PlayerData.SpendTenmei(15); // 魂片15追加消費
                resultMessage += $"『{item.name}』\n\n";
                resultMessage += "💔 思うようにはいきませんでした...\n";
                resultMessage += "しかし、この経験も無駄ではないはず。\n\n";
                resultMessage += "⚠️ 追加で魂片15片を失いました\n";
                resultMessage += "現在の魂片: " + PlayerData.RemainingTenmei + "片";
            }
            else // "更なる絶望"
            {
                // 大失敗: ステータス減少 + 魂片追加消費
                PlayerData.Luck = Math.Max(0, PlayerData.Luck + item.luck); // 負の値なのでマイナスになる
                PlayerData.Concentration = Math.Max(0, PlayerData.Concentration + item.concentration);
                PlayerData.Kindness = Math.Max(0, PlayerData.Kindness + item.kindness);
                PlayerData.SpendTenmei(15); // 魂片15追加消費
                
                resultMessage += $"『{item.name}』\n\n";
                resultMessage += "💀 最悪の結果です...\n";
                resultMessage += "逆転を狙ったつもりが、更に深い底へ。\n\n";
                resultMessage += $"🍀 運 {item.luck}\n🎯 集中力 {item.concentration}\n💖 優しさ {item.kindness}\n\n";
                resultMessage += "⚠️ 追加で魂片15片を失いました\n";
                resultMessage += "現在の魂片: " + PlayerData.RemainingTenmei + "片\n\n";
                resultMessage += "😰 でも...まだ諦めるには早すぎる。";
            }
            
            return resultMessage;
        }
        
        private string GetGachaDisplayName(GachaType gachaType)
        {
            switch (gachaType)
            {
                case GachaType.Childhood: return "童心ガチャ";
                case GachaType.Youth: return "青春ガチャ";
                case GachaType.Career: return "キャリアガチャ";
                case GachaType.Mature: return "円熟ガチャ";
                case GachaType.Senior: return "長老ガチャ";
                case GachaType.Legendary: return "伝説ガチャ";
                case GachaType.Redemption: return "逆転ガチャ";
                case GachaType.Love: return "恋愛ガチャ";
                case GachaType.Reversal: return "人生逆転ガチャ";
                case GachaType.Cursed: return "呪いガチャ";
                case GachaType.Academic: return "学術ガチャ";
                case GachaType.Social: return "社交ガチャ";
                case GachaType.Gambler: return "ギャンブラーガチャ";
                default: return gachaType.ToString();
            }
        }
        
        private string GetGachaFlavorText(GachaType gachaType)
        {
            switch (gachaType)
            {
                case GachaType.Childhood: return "「子供の頃の純粋な心が、あなたの人生を輝かせます」";
                case GachaType.Youth: return "「青春の情熱が、新たな可能性を切り開きます」";
                case GachaType.Career: return "「プロとしての矜持が、あなたを成長させます」";
                case GachaType.Mature: return "「人生経験という財産が、深みを与えてくれます」";
                case GachaType.Senior: return "「長年の知恵が、最後の輝きを放ちます」";
                case GachaType.Legendary: return "「運命の女神があなたに微笑みかけています」";
                case GachaType.Redemption: return "「失敗こそが最大の教師。立ち上がる力を授けます」";
                case GachaType.Love: return "「愛の力が、あなたを内側から変えていきます」";
                case GachaType.Reversal: return "「これまでの人生がうまくいかなくても、ここから全てを変えられます」";
                case GachaType.Cursed: return "「闇の力があなたを蝕んでいきます...本当に引く気ですか？」";
                case GachaType.Academic: return "「知識は力なり。ただし代償は大きいです」";
                case GachaType.Social: return "「人とのつながりが一番大切...しかし値段も一番高いです」";
                case GachaType.Gambler: return "「運こそすべて！最高額を賭ける価値はあるか？」";
                default: return "「特別な力があなたを包み込みます」";
            }
        }

        private string UpdateStatus(GachaItem item, GachaType gachaType)
        {
            switch (gachaType)
            {
                case GachaType.Luck:
                    PlayerData.Luck += item.luck;
                    PlayerData.LastLuckItemName = item.name; // 最後のアイテム名を記録
                    return $"【運ガチャ結果】\n『{item.name}』を引いた！\n\n「見えざる力、信じる者は救われるのです...たぶん」\n\n🍀 運が {item.luck} 上昇！ (現在値: {PlayerData.Luck})";
                case GachaType.Concentration:
                    PlayerData.Concentration += item.concentration;
                    PlayerData.LastConcentrationItemName = item.name; // 最後のアイテム名を記録
                    return $"【集中力ガチャ結果】\n『{item.name}』を引いた！\n\n「これであなたも意識高い系。スタバでMacを開きましょう」\n\n🎯 集中力が {item.concentration} 上昇！ (現在値: {PlayerData.Concentration})";
                case GachaType.Kindness:
                    PlayerData.Kindness += item.kindness;
                    PlayerData.LastKindnessItemName = item.name; // 最後のアイテム名を記録
                    return $"【優しさガチャ結果】\n『{item.name}』を引いた！\n\n「優しさは、時に利用されるだけ...なんて言わないであげてください」\n\n💖 優しさが {item.kindness} 上昇！ (現在値: {PlayerData.Kindness})";
                default:
                    return ""; // ここには来ないはず
            }
        }
        
        private string UpdateCurrentEquipment(GachaItem item, GachaType gachaType)
        {
            string previousItemName;
            switch (gachaType)
            {
                case GachaType.Beauty:
                    previousItemName = PlayerData.CurrentOutfit?.name ?? "布";
                    if (PlayerData.CurrentOutfit == null || item.points > PlayerData.CurrentOutfit.points)
                    {
                        PlayerData.CurrentOutfit = item;
                        return $"【美貌ガチャ結果】\nSSR【{item.name}】キター！\n\n「{previousItemName}」を脱ぎ捨て、新たな自分へ。\n街を歩けば、誰もが振り返る...はず！";
                    }
                    else
                    {
                        return $"【美貌ガチャ結果】\n『{item.name}』を引いた！\n\n...まあ、今の『{previousItemName}』の方がマシかな。\nタンスの肥やしがまた一つ増えました。";
                    }
                
                case GachaType.FamilyWealth:
                    previousItemName = PlayerData.CurrentFamilyWealth?.name ?? "庶民";
                    if (PlayerData.CurrentFamilyWealth == null || item.points > PlayerData.CurrentFamilyWealth.points)
                    {
                        PlayerData.CurrentFamilyWealth = item;
                        return $"【家柄ガチャ結果】\nUR【{item.name}】降臨！\n\nもはやこれまでとは別人です。\n明日からあなたを見る目が変わります。たぶん。";
                    }
                    else
                    {
                        return $"【家柄ガチャ結果】\n『{item.name}』を引いた！\n\n今の『{previousItemName}』のほうが強そうなので、\nプロフィール帳の片隅にでも書いておきましょう。";
                    }
                
                case GachaType.Personality:
                    previousItemName = PlayerData.CurrentPersonality?.name ?? "無個性";
                    if (PlayerData.CurrentPersonality == null || item.points > PlayerData.CurrentPersonality.points)
                    {
                        PlayerData.CurrentPersonality = item;
                        return $"【性格ガチャ結果】\n★5【{item.name}】爆誕！\n\n「{previousItemName}」だった頃の記憶はもうありません。\nこれであなたもクラスの人気者...だといいね！";
                    }
                    else
                    {
                        return $"【性格ガチャ結果】\n『{item.name}』を引いた！\n\n今の『{previousItemName}』のままでいいや...\nこれ以上ややこしくなるのは勘弁。";
                    }
                
                default:
                    return $"🎴 結果: {item.name}";
            }
        }
    }
}
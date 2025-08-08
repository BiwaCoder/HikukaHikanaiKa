using UnityEngine;
using HikukaHikanaika.Models;

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
            const int LIFESPAN_COST = 2; // ガチャ1回につき2年消費
            return PlayerData.CanAffordLifespan(LIFESPAN_COST);
        }
        
        public string PerformGacha(GachaType gachaType)
        {
            const int LIFESPAN_COST = 2; // ガチャ1回につき2年消費
            
            if (PlayerData.IsGameOver())
            {
                return "💀 人生が終了しています...";
            }
            
            if (!PlayerData.CanAffordLifespan(LIFESPAN_COST))
            {
                return "⏰ 寿命が足りません！残り" + PlayerData.RemainingLifespan + "年";
            }
            
            PlayerData.SpendLifespan(LIFESPAN_COST);
            GachaItem pulledItem = gachaModel.PullGacha(gachaType);
            
            string result = ProcessGachaResult(pulledItem, gachaType);
            
            // ガチャ後に寿命チェック
            if (PlayerData.IsGameOver())
            {
                result += "\n\n💀 あなたの人生は終了しました...";
            }
            
            return result;
        }
        
        private string ProcessGachaResult(GachaItem item, GachaType gachaType)
        {
            // 新しいステータスガチャの処理
            if (gachaType == GachaType.Luck || gachaType == GachaType.Concentration || gachaType == GachaType.Kindness)
            {
                return UpdateStatus(item, gachaType);
            }

            // 既存の装備ガチャの処理
            if (PlayerData.HasItem(item.name, gachaType))
            {
                return $"🎴 結果: {item.name}\n🔄 既に所持しています。";
            }
            
            PlayerData.AddItem(item.name, gachaType);
            return UpdateCurrentEquipment(item, gachaType);
        }

        private string UpdateStatus(GachaItem item, GachaType gachaType)
        {
            switch (gachaType)
            {
                case GachaType.Luck:
                    PlayerData.Luck += item.luck;
                    return $"【運ガチャ結果】\n『{item.name}』を引いた！\n\n「見えざる力、信じる者は救われるのです...たぶん」\n\n🍀 運が {item.luck} 上昇！ (現在値: {PlayerData.Luck})";
                case GachaType.Concentration:
                    PlayerData.Concentration += item.concentration;
                    return $"【集中力ガチャ結果】\n『{item.name}』を引いた！\n\n「これであなたも意識高い系。スタバでMacを開きましょう」\n\n🎯 集中力が {item.concentration} 上昇！ (現在値: {PlayerData.Concentration})";
                case GachaType.Kindness:
                    PlayerData.Kindness += item.kindness;
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
using System.Collections.Generic;
using UnityEngine;

namespace HikukaHikanaika.Models
{
    // イベント結果を記録するためのデータ構造
    public class GameEventRecord
    {
        public string LifeStage { get; }
        public string EventTitle { get; }
        public string ResultText { get; }

        public GameEventRecord(string lifeStage, string eventTitle, string resultText)
        {
            LifeStage = lifeStage;
            EventTitle = eventTitle;
            ResultText = resultText;
        }
    }

    public class PlayerData
    {
        private static PlayerData instance;
        public static PlayerData Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PlayerData();
                }
                return instance;
            }
        }
        
        public string PlayerName { get; set; } = "あなた";
        public int CurrentAge { get; set; } = 0;
        public int RemainingTenmei { get; set; } = 80; // 残り魂片（片）
        public int LifeCycle { get; set; } = 0;
        
        public int Luck { get; set; } = 0;
        public int Concentration { get; set; } = 0;
        public int Kindness { get; set; } = 0;

        // ゲームイベントの履歴
        public List<GameEventRecord> EventHistory { get; private set; }

        // 所持アイテム
        private HashSet<string> ownedBeautyItems = new HashSet<string>();
        private HashSet<string> ownedFamilyWealthItems = new HashSet<string>();
        private HashSet<string> ownedPersonalityItems = new HashSet<string>();
        
        // 現在の装備
        public GachaItem CurrentOutfit { get; set; }
        public GachaItem CurrentFamilyWealth { get; set; }
        public GachaItem CurrentPersonality { get; set; }

        // 新しいステータスを上げた最後のアイテム名
        public string LastLuckItemName { get; set; } = "なし";
        public string LastConcentrationItemName { get; set; } = "なし";
        public string LastKindnessItemName { get; set; } = "なし";
        
        // ガチャ解放状態管理
        private HashSet<GachaType> unlockedGachaTypes = new HashSet<GachaType>();
        
        // 最新の解放されたガチャ（通知用）
        public List<GachaType> NewlyUnlockedGachaTypes { get; private set; } = new List<GachaType>();
        
        // 一回限りガチャの使用状況
        private HashSet<GachaType> usedOnceOnlyGachas = new HashSet<GachaType>();
        
        private PlayerData()
        {
            CurrentAge = 0;
            RemainingTenmei = 80;
            LifeCycle = 0;
            Luck = 10;
            Concentration = 10;
            Kindness = 10;
            EventHistory = new List<GameEventRecord>(); // 履歴リストを初期化
            
            // 基本ガチャは最初から解放
            unlockedGachaTypes.Add(GachaType.Beauty);
            unlockedGachaTypes.Add(GachaType.FamilyWealth);
            unlockedGachaTypes.Add(GachaType.Personality);
            unlockedGachaTypes.Add(GachaType.Luck);
            unlockedGachaTypes.Add(GachaType.Concentration);
            unlockedGachaTypes.Add(GachaType.Kindness);
            
            // 使えない（超損）ガチャも最初から利用可能（誰も使わないけど）
            unlockedGachaTypes.Add(GachaType.Cursed);
            unlockedGachaTypes.Add(GachaType.Academic);
            unlockedGachaTypes.Add(GachaType.Social);
            unlockedGachaTypes.Add(GachaType.Gambler);
        }
        
        public static void Initialize()
        {
            instance = new PlayerData();
        }
        
        public static void Reset()
        {
            instance = null;
        }

        // イベント履歴を追加するメソッド
        public void AddEventRecord(string lifeStage, string eventTitle, string resultText)
        {
            EventHistory.Add(new GameEventRecord(lifeStage, eventTitle, resultText));
        }
        
        public bool HasItem(string itemName, GachaType type)
        {
            return GetOwnedItemsSet(type).Contains(itemName);
        }
        
        public void AddItem(string itemName, GachaType type)
        {
            GetOwnedItemsSet(type).Add(itemName);
        }
        
        public HashSet<string> GetOwnedItems(GachaType type)
        {
            return GetOwnedItemsSet(type);
        }
        
        public int GetAppearancePoints()
        {
            int total = 0;
            total += CurrentOutfit?.points ?? 0;
            total += CurrentFamilyWealth?.points ?? 0;
            total += CurrentPersonality?.points ?? 0;
            return total;
        }
        
        public Dictionary<string, int> GetAllStatus()
        {
            var status = new Dictionary<string, int>();
            status.Add("外見", GetAppearancePoints());
            status.Add("運", Luck);
            status.Add("集中力", Concentration);
            status.Add("優しさ", Kindness);
            return status;
        }

        public void SpendTenmei(int points)
        {
            RemainingTenmei -= points;
            if (RemainingTenmei < 0) RemainingTenmei = 0;
        }
        
        public bool CanAffordTenmei(int points)
        {
            return RemainingTenmei >= points;
        }
        
        public void AdvanceAge()
        {
            LifeCycle++;
            CurrentAge = LifeCycle * 5;
        }
        
        public bool IsAlive()
        {
            return RemainingTenmei > 0 && CurrentAge < 80;
        }
        
        public bool IsGameOver()
        {
            return RemainingTenmei <= 0 || CurrentAge >= 80;
        }
        
        public string GetLifeStage()
        {
            switch (LifeCycle)
            {
                case 0: return "幼児期 (0-5歳)";
                case 1: return "児童期 (5-10歳)";
                case 2: return "思春期 (10-15歳)";
                case 3: return "青春期 (15-20歳)";
                case 4: return "就職期 (20-25歳)";
                case 5: return "成人期 (25-30歳)";
                case 6: return "責任期 (30-35歳)";
                case 7: return "充実期 (35-40歳)";
                case 8: return "中年期 (40-45歳)";
                case 9: return "成熟期 (45-50歳)";
                case 10: return "転換期 (50-55歳)";
                case 11: return "安定期 (55-60歳)";
                case 12: return "準備期 (60-65歳)";
                case 13: return "シニア期 (65-70歳)";
                case 14: return "長老期 (70-75歳)";
                case 15: return "晩年期 (75-80歳)";
                default: return "人生終了";
            }
        }
        
        private HashSet<string> GetOwnedItemsSet(GachaType type)
        {
            switch (type)
            {
                case GachaType.Beauty: return ownedBeautyItems;
                case GachaType.FamilyWealth: return ownedFamilyWealthItems;
                case GachaType.Personality: return ownedPersonalityItems;
                default: return ownedBeautyItems;
            }
        }
        
        // ガチャ解放・制御メソッド
        public bool IsGachaUnlocked(GachaType gachaType)
        {
            return unlockedGachaTypes.Contains(gachaType);
        }
        
        public void UnlockGacha(GachaType gachaType)
        {
            if (!unlockedGachaTypes.Contains(gachaType))
            {
                unlockedGachaTypes.Add(gachaType);
                NewlyUnlockedGachaTypes.Add(gachaType);
            }
        }
        
        public void ClearNewUnlockNotifications()
        {
            NewlyUnlockedGachaTypes.Clear();
        }
        
        // 一回限りガチャのチェック
        public bool IsOnceOnlyGacha(GachaType gachaType)
        {
            return gachaType == GachaType.Legendary;
        }
        
        public bool HasUsedOnceOnlyGacha(GachaType gachaType)
        {
            return usedOnceOnlyGachas.Contains(gachaType);
        }
        
        public void MarkOnceOnlyGachaAsUsed(GachaType gachaType)
        {
            if (IsOnceOnlyGacha(gachaType))
            {
                usedOnceOnlyGachas.Add(gachaType);
            }
        }
        
        // 上位5%判定（全ステータス合計基準）
        public bool IsTopTierPlayer()
        {
            int totalStats = Luck + Concentration + Kindness + 
                           (CurrentOutfit?.points ?? 0) + 
                           (CurrentFamilyWealth?.points ?? 0) + 
                           (CurrentPersonality?.points ?? 0);
            
            // 上位5%の基準：全ステータス合計1200以上
            // この数値は通常のプレイでは到達困難で、やり込み要素
            return totalStats >= 1200;
        }
        
        // 神の道開放条件チェック
        public bool CanAccessDivinePath()
        {
            return LifeCycle >= 16 && IsTopTierPlayer();
        }
        
        // ライフステージ限定ガチャの利用可能性チェック
        public bool IsLifestageGachaAvailable(GachaType gachaType)
        {
            if (!IsGachaUnlocked(gachaType)) return false;
            
            switch (gachaType)
            {
                case GachaType.Childhood: return CurrentAge <= 10;
                case GachaType.Youth: return CurrentAge >= 15 && CurrentAge <= 25;
                case GachaType.Career: return CurrentAge >= 20 && CurrentAge <= 40;
                case GachaType.Mature: return CurrentAge >= 40 && CurrentAge <= 60;
                case GachaType.Senior: return CurrentAge >= 60;
                case GachaType.Reversal: return CurrentAge >= 25 && CurrentAge <= 35; // 30歳前後でのみ利用可能
                default: return true; // 通常ガチャは年齢制限なし
            }
        }
        
        // イベント結果によるガチャ解放
        public void ProcessEventResult(GachaUnlockResult result, string eventType = "")
        {
            switch (result)
            {
                case GachaUnlockResult.GreatSuccess:
                    UnlockGacha(GachaType.Legendary);
                    break;
                case GachaUnlockResult.GreatFailure:
                    UnlockGacha(GachaType.Redemption);
                    break;
            }
            
            // 恋愛イベント特別処理
            if (eventType == "恋愛" && result == GachaUnlockResult.Success)
            {
                UnlockGacha(GachaType.Love);
            }
            
            // ライフステージ限定ガチャの自動解放
            CheckAndUnlockLifestageGachas();
        }
        
        private void CheckAndUnlockLifestageGachas()
        {
            // 年齢に応じてライフステージガチャを解放
            if (CurrentAge <= 10 && !IsGachaUnlocked(GachaType.Childhood))
            {
                UnlockGacha(GachaType.Childhood);
            }
            if (CurrentAge >= 15 && CurrentAge <= 25 && !IsGachaUnlocked(GachaType.Youth))
            {
                UnlockGacha(GachaType.Youth);
            }
            if (CurrentAge >= 20 && CurrentAge <= 40 && !IsGachaUnlocked(GachaType.Career))
            {
                UnlockGacha(GachaType.Career);
            }
            if (CurrentAge == 30 && !IsGachaUnlocked(GachaType.Reversal))
            {
                // 30歳で逆転ガチャを特別解放
                UnlockGacha(GachaType.Reversal);
            }
            if (CurrentAge >= 40 && CurrentAge <= 60 && !IsGachaUnlocked(GachaType.Mature))
            {
                UnlockGacha(GachaType.Mature);
            }
            if (CurrentAge >= 60 && !IsGachaUnlocked(GachaType.Senior))
            {
                UnlockGacha(GachaType.Senior);
            }
        }
    }
}
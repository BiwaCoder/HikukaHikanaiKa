# MVCコンポーネント接続図

## 🎯 コンポーネント関係図

```
┌─────────────────────────────────────┐
│            Unity Scene              │
│  ┌─────────────────────────────────┐│
│  │         Canvas                  ││
│  │  ┌─────────────────────────────┐││
│  │  │      GachaSystem            │││
│  │  │  ┌─────────────────────────┐│││
│  │  │  │   GachaController       ││││ ←── メインコントローラー
│  │  │  │   - GachaController.cs  ││││
│  │  │  │   - GachaView.cs        ││││
│  │  │  └─────────────────────────┘│││
│  │  │                             │││
│  │  │  ┌─────────────────────────┐│││
│  │  │  │   GachaButtons          ││││
│  │  │  │   - BeautyGachaButton   ││││ ──┐
│  │  │  │   - FamilyWealthButton  ││││   │
│  │  │  │   - PersonalityButton   ││││   │
│  │  │  └─────────────────────────┘│││   │
│  │  │                             │││   │
│  │  │  ┌─────────────────────────┐│││   │
│  │  │  │   GachaUI               ││││   │
│  │  │  │   - MoneyText           ││││ ◄─┤ 参照接続
│  │  │  │   - ResultText          ││││   │
│  │  │  │   - StatusText          ││││   │
│  │  │  │   - OwnedItemsText      ││││   │
│  │  │  └─────────────────────────┘│││   │
│  │  └─────────────────────────────┘││   │
│  │                                 ││   │
│  │  ┌─────────────────────────────┐││   │
│  │  │   RealityShowSystem         │││   │
│  │  │  ┌─────────────────────────┐│││   │
│  │  │  │  RealityShowController  ││││ ◄─┘
│  │  │  │  - RealityShowCont...cs ││││
│  │  │  │  - RealityShowView.cs   ││││
│  │  │  └─────────────────────────┘│││
│  │  │                             │││
│  │  │  ┌─────────────────────────┐│││
│  │  │  │  RealityShowUI          ││││
│  │  │  │  - JudgeButton          ││││ ──┐
│  │  │  │  - JudgeResultText      ││││   │
│  │  │  │  - RivalStatusText      ││││ ◄─┘
│  │  │  └─────────────────────────┘│││
│  │  └─────────────────────────────┘││
│  └─────────────────────────────────┘│
└─────────────────────────────────────┘
```

## 🔗 詳細な接続マップ

### GachaController → GachaView
```
GachaController (Script Component)
├── GachaView参照 ──→ GachaView (同じGameObject)
│
GachaView (Script Component)  
├── beautyGachaButton ──→ BeautyGachaButton (Button)
├── familyWealthGachaButton ──→ FamilyWealthGachaButton (Button) 
├── personalityGachaButton ──→ PersonalityGachaButton (Button)
├── resultText ──→ ResultText (Text)
├── moneyText ──→ MoneyText (Text)
├── statusText ──→ StatusText (Text)
└── ownedItemsText ──→ OwnedItemsText (Text)
```

### RealityShowController → RealityShowView
```
RealityShowController (Script Component)
├── realityShowView ──→ RealityShowView (同じGameObject)
├── gachaController ──→ GachaController GameObject
│
RealityShowView (Script Component)
├── judgeButton ──→ JudgeButton (Button)
├── judgeResultText ──→ JudgeResultText (Text)
└── rivalStatusText ──→ RivalStatusText (Text)
```

## 📊 Inspector設定チェックリスト

### ✅ GachaController GameObject
- [ ] **Gacha Controller (Script)** コンポーネント追加済み
- [ ] **Gacha View (Script)** コンポーネント追加済み
- [ ] GachaController.gachaView → 同GameObject の GachaView

### ✅ GachaView設定 (GachaController GameObject内)
- [ ] beautyGachaButton → BeautyGachaButton
- [ ] familyWealthGachaButton → FamilyWealthGachaButton  
- [ ] personalityGachaButton → PersonalityGachaButton
- [ ] resultText → ResultText
- [ ] moneyText → MoneyText
- [ ] statusText → StatusText
- [ ] ownedItemsText → OwnedItemsText

### ✅ RealityShowController GameObject  
- [ ] **Reality Show Controller (Script)** コンポーネント追加済み
- [ ] **Reality Show View (Script)** コンポーネント追加済み
- [ ] RealityShowController.realityShowView → 同GameObject の RealityShowView
- [ ] RealityShowController.gachaController → GachaController GameObject

### ✅ RealityShowView設定 (RealityShowController GameObject内)
- [ ] judgeButton → JudgeButton
- [ ] judgeResultText → JudgeResultText  
- [ ] rivalStatusText → RivalStatusText

## 🎮 実際の配置例

### Canvas階層の実際の構造
```
Canvas
├── GachaSystem                    ←── Empty GameObject
│   ├── GachaController           ←── Empty GameObject + Scripts
│   │   ├── GachaController.cs    ←── Script Component
│   │   └── GachaView.cs          ←── Script Component  
│   ├── GachaButtons              ←── Empty GameObject (UI整理用)
│   │   ├── BeautyGachaButton     ←── Button Component
│   │   ├── FamilyWealthButton    ←── Button Component
│   │   └── PersonalityButton     ←── Button Component
│   └── GachaUI                   ←── Empty GameObject (UI整理用)
│       ├── MoneyText             ←── Text Component
│       ├── ResultText            ←── Text Component  
│       ├── StatusText            ←── Text Component
│       └── OwnedItemsText        ←── Text Component
│
└── RealityShowSystem             ←── Empty GameObject
    ├── RealityShowController     ←── Empty GameObject + Scripts
    │   ├── RealityShowController.cs  ←── Script Component
    │   └── RealityShowView.cs    ←── Script Component
    └── RealityShowUI             ←── Empty GameObject (UI整理用)
        ├── JudgeButton           ←── Button Component
        ├── JudgeResultText       ←── Text Component
        └── RivalStatusText       ←── Text Component
```

## 🚨 よくある設定ミス

### ❌ 間違い例
1. **同じGameObjectに複数のControllerを配置**
   - GachaControllerとRealityShowControllerは別々のGameObjectに配置

2. **ViewをControllerと別のGameObjectに配置**  
   - ViewはControllerと同じGameObjectに配置

3. **UI要素の参照ミス**
   - InspectorでNoneになっている項目がないか確認

### ✅ 正しい例
1. **各システムごとに専用GameObject**
2. **ControllerとViewは同じGameObject内**
3. **UI要素は適切にグループ化**

この配置により、MVCアーキテクチャの利点を最大限活用できる、保守性の高いUnityプロジェクトが完成します！
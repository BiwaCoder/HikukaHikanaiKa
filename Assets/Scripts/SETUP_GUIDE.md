# Unity Editor MVC配置ガイド

## 📋 手順概要
1. [シーン階層の作成](#1-シーン階層の作成)
2. [MVCコンポーネントの配置](#2-mvcコンポーネントの配置)
3. [UI要素の接続](#3-ui要素の接続)
4. [Inspector設定](#4-inspector設定)
5. [動作確認](#5-動作確認)

---

## 1. シーン階層の作成

### 推奨シーン構造
```
SampleScene
├── Main Camera
├── Directional Light
├── Canvas
│   ├── GachaSystem (Empty GameObject)
│   │   ├── GachaButtons (Empty GameObject)
│   │   │   ├── BeautyGachaButton (Button)
│   │   │   ├── FamilyWealthGachaButton (Button)
│   │   │   └── PersonalityGachaButton (Button)
│   │   ├── GachaUI (Empty GameObject)
│   │   │   ├── MoneyText (Text)
│   │   │   ├── ResultText (Text)
│   │   │   ├── StatusText (Text)
│   │   │   └── OwnedItemsText (Text)
│   │   └── GachaController (Empty GameObject)
│   └── RealityShowSystem (Empty GameObject)
│       ├── RealityShowUI (Empty GameObject)
│       │   ├── JudgeButton (Button)
│       │   ├── JudgeResultText (Text)
│       │   └── RivalStatusText (Text)
│       └── RealityShowController (Empty GameObject)
└── EventSystem
```

### 作成手順

#### Step 1-1: 基本システム用Empty GameObjectを作成
1. Canvasを右クリック → Create Empty
2. 名前を「GachaSystem」に変更
3. 同様に「RealityShowSystem」を作成

#### Step 1-2: UI要素グループ用Empty GameObjectを作成
1. GachaSystemを右クリック → Create Empty
2. 名前を「GachaButtons」に変更
3. 同様に「GachaUI」を作成
4. RealityShowSystemに「RealityShowUI」を作成

#### Step 1-3: コントローラー用Empty GameObjectを作成
1. GachaSystemを右クリック → Create Empty
2. 名前を「GachaController」に変更
3. RealityShowSystemに「RealityShowController」を作成

---

## 2. MVCコンポーネントの配置

### Step 2-1: GachaControllerの設定
1. **GachaController** GameObjectを選択
2. Inspector → Add Component
3. 「Gacha Controller (Script)」を追加

### Step 2-2: GachaViewの設定
1. **GachaController** GameObjectを選択（同じオブジェクト）
2. Inspector → Add Component
3. 「Gacha View (Script)」を追加

### Step 2-3: RealityShowControllerの設定
1. **RealityShowController** GameObjectを選択
2. Inspector → Add Component
3. 「Reality Show Controller (Script)」を追加

### Step 2-4: RealityShowViewの設定
1. **RealityShowController** GameObjectを選択（同じオブジェクト）
2. Inspector → Add Component  
3. 「Reality Show View (Script)」を追加

---

## 3. UI要素の接続

### Step 3-1: ガチャボタンの作成と配置
1. **GachaButtons** を右クリック → UI → Button
2. 3つのボタンを作成して以下の名前に変更：
   - `BeautyGachaButton`
   - `FamilyWealthGachaButton` 
   - `PersonalityGachaButton`

3. 各ボタンの子Text要素のテキストを変更：
   - 「服」
   - 「家柄」
   - 「性格」

### Step 3-2: ガチャUI要素の作成
1. **GachaUI** を右クリック → UI → Text で以下を作成：
   - `MoneyText` - 「💰 所持金：300,000,000 円」
   - `ResultText` - 「ガチャ結果がここに表示されます」  
   - `StatusText` - 「プレイヤーステータス」
   - `OwnedItemsText` - 「所持アイテム」

### Step 3-3: リアリティーショーUI要素の作成
1. **RealityShowUI** を右クリック → UI → Button
2. 名前を `JudgeButton` に変更、テキストを「判定開始」に変更

3. **RealityShowUI** を右クリック → UI → Text で以下を作成：
   - `JudgeResultText` - 「判定結果がここに表示されます」
   - `RivalStatusText` - 「ライバル情報」

---

## 4. Inspector設定

### Step 4-1: GachaController Inspector設定
1. **GachaController** GameObjectを選択
2. **Gacha Controller** コンポーネントで：
   - `Gacha View` に同じGameObjectの **Gacha View** をドラッグ

3. **Gacha View** コンポーネントで：
   - `Beauty Gacha Button` に **BeautyGachaButton** をドラッグ
   - `Family Wealth Gacha Button` に **FamilyWealthGachaButton** をドラッグ  
   - `Personality Gacha Button` に **PersonalityGachaButton** をドラッグ
   - `Result Text` に **ResultText** をドラッグ
   - `Money Text` に **MoneyText** をドラッグ
   - `Status Text` に **StatusText** をドラッグ
   - `Owned Items Text` に **OwnedItemsText** をドラッグ

### Step 4-2: RealityShowController Inspector設定
1. **RealityShowController** GameObjectを選択
2. **Reality Show Controller** コンポーネントで：
   - `Reality Show View` に同じGameObjectの **Reality Show View** をドラッグ
   - `Gacha Controller` に **GachaController** GameObjectをドラッグ

3. **Reality Show View** コンポーネントで：
   - `Judge Button` に **JudgeButton** をドラッグ
   - `Judge Result Text` に **JudgeResultText** をドラッグ
   - `Rival Status Text` に **RivalStatusText** をドラッグ

---

## 5. 動作確認

### Step 5-1: Play Modeでテスト
1. Playボタンを押してPlay Modeに入る
2. 各ガチャボタンをクリックして動作確認
3. リアリティーショーの判定ボタンをクリックして動作確認

### Step 5-2: トラブルシューティング
**ボタンが反応しない場合：**
- EventSystemが存在するか確認
- Canvas配下にUI要素があるか確認
- Button ComponentのInteractableがtrueか確認

**参照エラーが出る場合：**
- Inspector設定で参照が正しくドラッグされているか確認
- スクリプトファイルが正しい場所にあるか確認

---

## 6. レイアウト調整のヒント

### UI配置の推奨値
```
ガチャボタン:
- Position: 横並び、適度な間隔
- Size: (160, 50)

テキスト要素:
- MoneyText: 画面上部
- ResultText: 画面中央
- StatusText: 画面左側
- OwnedItemsText: 画面右側
```

### Canvas設定
- Canvas Scaler: Scale With Screen Size
- Reference Resolution: (1920, 1080)
- Screen Match Mode: Match Width Or Height

これで完璧なMVCアーキテクチャのUnityシーンが完成します！
# HikukaHikanaika Unity UI セットアップマニュアル

## 概要
『運命の館 〜HikukaHikanaika〜』ゲームのUIセットアップ手順書です。
夕暮れ時の古いヨーロピアンスタイルの館を舞台に、美しき女性たちが織りなす運命の物語を描く
ドラマチックなリアリティーショー機能とAI連携のためのUI要素を正しく配置する方法を説明します。

## 前提条件
- Unity 2021.3.28f1以上
- Newtonsoft.Json パッケージがインストール済み

## 必要なパッケージ

### 1. Newtonsoft.Json のインストール
```
Window > Package Manager > Add package by name...
com.unity.nuget.newtonsoft-json
```

## UIヒエラルキー構造

### 基本シーン構成
```
Canvas
├── GachaPanel
│   ├── GachaButton
│   ├── MoneyText
│   └── PlayerStatusPanel
│       ├── OutfitText
│       ├── FamilyWealthText
│       └── PersonalityText
└── RealityShowPanel
    ├── RivalStatusText
    ├── JudgeButtonGroup
    │   ├── LocalJudgeButton
    │   └── AIJudgeButton
    ├── LoadingIndicator
    │   ├── LoadingText
    │   └── LoadingSpinner (オプション)
    ├── ErrorText
    └── ResultScrollView
        └── ResultText
```

## コンポーネントセットアップ詳細

### 1. Canvas セットアップ
1. **Hierarchy** で右クリック → **UI** → **Canvas**
2. **Canvas Scaler** コンポーネントを追加
   - **UI Scale Mode**: Scale With Screen Size
   - **Reference Resolution**: 1920x1080
   - **Screen Match Mode**: Match Width Or Height
   - **Match**: 0.5

### 2. GachaPanel セットアップ
1. **Canvas** の子として **Empty GameObject** を作成、名前を **GachaPanel** に変更
2. **RectTransform** を追加し、画面左半分に配置
   - **Anchors**: Min(0,0), Max(0.5,1)
   - **Offset**: Left=10, Right=-10, Top=-10, Bottom=10

#### GachaController 接続
1. **GachaPanel** に **GachaController** スクリプトをアタッチ
2. **GachaView** スクリプトもアタッチ
3. 必要なUI要素を **Inspector** で接続

### 3. RealityShowPanel セットアップ
1. **Canvas** の子として **Empty GameObject** を作成、名前を **RealityShowPanel** に変更
2. **RectTransform** を追加し、画面右半分に配置
   - **Anchors**: Min(0.5,0), Max(1,1)
   - **Offset**: Left=10, Right=-10, Top=-10, Bottom=10

#### 主要UI要素の作成

##### RivalStatusText
1. **RealityShowPanel** の子として **UI** → **Text** を作成
2. 名前を **RivalStatusText** に変更
3. **RectTransform** 設定:
   - **Anchors**: Top-Left to Top-Right
   - **Height**: 150
4. **Text** コンポーネント設定:
   - **Font Size**: 18
   - **Alignment**: Top-Left
   - **Text**: "ライバル情報"

##### JudgeButtonGroup
1. **RealityShowPanel** の子として **Empty GameObject** を作成
2. 名前を **JudgeButtonGroup** に変更
3. **Horizontal Layout Group** コンポーネントを追加
   - **Spacing**: 20
   - **Child Controls Size**: Width=true, Height=true

###### LocalJudgeButton
1. **JudgeButtonGroup** の子として **UI** → **Button** を作成
2. 名前を **LocalJudgeButton** に変更
3. 子の **Text** を "ローカル判定" に設定

###### AIJudgeButton
1. **JudgeButtonGroup** の子として **UI** → **Button** を作成
2. 名前を **AIJudgeButton** に変更
3. 子の **Text** を "AI判定" に設定

##### LoadingIndicator
1. **RealityShowPanel** の子として **Empty GameObject** を作成
2. 名前を **LoadingIndicator** に変更
3. **Image** コンポーネントを追加（背景用）
4. デフォルトでは非アクティブに設定

###### LoadingText
1. **LoadingIndicator** の子として **UI** → **Text** を作成
2. 名前を **LoadingText** に変更
3. **Text**: "AI解説生成中..."
4. **Font Size**: 24
5. **Alignment**: Center

##### ErrorText
1. **RealityShowPanel** の子として **UI** → **Text** を作成
2. 名前を **ErrorText** に変更
3. **Color**: 赤色 (RGB: 255, 100, 100)
4. デフォルトでは非アクティブに設定

##### ResultScrollView
1. **RealityShowPanel** の子として **UI** → **Scroll View** を作成
2. 名前を **ResultScrollView** に変更
3. **RectTransform** 設定:
   - **Anchors**: Stretch-Stretch
   - **Top**: -300（他のUI要素分のマージン）

###### ResultText
1. **ResultScrollView** → **Viewport** → **Content** の子として **UI** → **Text** を作成
2. 名前を **ResultText** に変更
3. **Content Size Fitter** コンポーネントを追加
   - **Vertical Fit**: Preferred Size
4. **Text** コンポーネント設定:
   - **Font Size**: 16
   - **Alignment**: Top-Left
   - **Rich Text**: true（色付きテキスト対応）

## スクリプト接続手順

### リアリティーショー機能のセットアップ

#### 推奨方法: ImprovedRealityShowController を使用
**統合コントローラー（ローカル判定 + AI判定の両方対応）**

1. **RealityShowPanel** に **ImprovedRealityShowController** スクリプトをアタッチ
2. **RealityShowPanel** に **RealityShowView** スクリプトもアタッチ
3. **Inspector** で以下を接続:
   - **Reality Show View**: 同オブジェクトの **RealityShowView**
   - **Gacha Controller**: **GachaPanel** の **GachaController**
   - **API URL**: "http://localhost:5001"

#### 代替方法: 個別コントローラーを使用
**個別に機能を分離したい場合**

##### A. RealityShowController (ローカル判定のみ)
1. **RealityShowPanel** に **RealityShowController** スクリプトをアタッチ
2. **Inspector** で以下を接続:
   - **Reality Show View**: 同オブジェクトの **RealityShowView**
   - **Gacha Controller**: **GachaPanel** の **GachaController**

##### B. RealityShowAPIController (AI判定のみ)
1. **RealityShowPanel** に **RealityShowAPIController** スクリプトをアタッチ
2. **Inspector** で以下を接続:
   - **API URL**: "http://localhost:5001"
   - **Reality Show View**: 同オブジェクトの **RealityShowView**
   - **Gacha Controller**: **GachaPanel** の **GachaController**

### RealityShowView セットアップ
**（どの方法でも共通）**

1. **RealityShowPanel** に **RealityShowView** スクリプトをアタッチ
2. **Inspector** で以下のUI要素を接続:
   - **Judge Button**: **LocalJudgeButton**
   - **Ai Judge Button**: **AIJudgeButton**
   - **Judge Result Text**: **ResultText**
   - **Rival Status Text**: **RivalStatusText**
   - **Loading Text**: **LoadingText**
   - **Error Text**: **ErrorText**
   - **Result Scroll Rect**: **ResultScrollView**
   - **Loading Indicator**: **LoadingIndicator**

### キャラクター判定機能のセットアップ（オプション）
**CharacterJudgmentController は別機能です**

1. 別の **Panel** に **CharacterJudgmentController** スクリプトをアタッチ
2. **CharacterJudgmentView** スクリプトもアタッチ  
3. **Inspector** で対応するUI要素を接続

## ボタンイベント接続

### 推奨方法: ImprovedRealityShowController を使用している場合

#### ローカル判定ボタン
1. **LocalJudgeButton** の **Button** コンポーネントで **On Click()** イベントを設定
2. **Target**: **ImprovedRealityShowController**
3. **Function**: **ImprovedRealityShowController.OnLocalJudgeButtonClicked()**

#### AI判定ボタン
1. **AIJudgeButton** の **Button** コンポーネントで **On Click()** イベントを設定
2. **Target**: **ImprovedRealityShowController**
3. **Function**: **ImprovedRealityShowController.OnAIJudgeButtonClicked()**

### 代替方法: 個別コントローラーを使用している場合

#### ローカル判定ボタン（RealityShowController使用時）
1. **LocalJudgeButton** の **Button** コンポーネントで **On Click()** イベントを設定
2. **Target**: **RealityShowController**
3. **Function**: **RealityShowController.OnJudgeButtonClicked()**

#### AI判定ボタン（RealityShowAPIController使用時）
1. **AIJudgeButton** の **Button** コンポーネントで **On Click()** イベントを設定
2. **Target**: **RealityShowAPIController**
3. **Function**: **RealityShowAPIController.OnJudgeWithAIButtonClicked()**

## デザイン調整のコツ

### テキストの見やすさ向上
1. **Text** コンポーネントに **Shadow** エフェクトを追加
2. **Effect Color**: 黒、**Effect Distance**: (1, -1)

### ボタンのビジュアル向上
1. **Button** の **Image** に **Sprite** を設定
2. **Button** コンポーネントの **Transition**: Color Tint
3. **Highlighted Color**: 明るい色
4. **Pressed Color**: 暗い色

### スクロールビューの改善
1. **Scrollbar Vertical** を表示
2. **Content** に **Vertical Layout Group** を追加
3. **Child Controls Size**: Height=true
4. **Child Force Expand**: Height=false

## トラブルシューティング

### よくある問題と解決方法

#### 1. "RealityShowView error" が表示される
- **原因**: UI要素の参照が正しく設定されていない
- **解決**: Inspector でNull参照がないかチェック

#### 2. ボタンが反応しない
- **原因**: EventSystem がシーンに存在しない
- **解決**: Hierarchy → Create → UI → Event System

#### 3. テキストが表示されない
- **原因**: Canvas の Render Mode が間違っている
- **解決**: Canvas の Render Mode を Screen Space - Overlay に設定

#### 4. ScrollView でスクロールできない
- **原因**: Content の Size が正しく設定されていない
- **解決**: Content Size Fitter の Vertical Fit を Preferred Size に設定

#### 5. API通信エラー
- **原因**: Pythonサーバーが起動していない、またはURL が間違っている
- **解決**: 
  - Pythonサーバーを起動: `python hikuka_hikanaika_server.py`
  - API URLを確認: "http://localhost:5001"
  - 環境変数 OPENAI_API_KEY が設定されているか確認

#### 6. 物語テキストが長すぎて表示されない
- **原因**: 新しいドラマチックな文体で生成されるテキストが長い
- **解決**: 
  - ScrollView の Content に Content Size Fitter を追加
  - Vertical Fit を Preferred Size に設定
  - Text コンポーネントの Rich Text を有効にする
  - ScrollView の高さを十分に確保する

#### 7. キャラクター名が表示されない（新キャラクター）
- **原因**: character_data.json の新しいキャラクター（詩織、るな、愛美）に対応していない
- **解決**: 
  - 最新の character_data.json を使用
  - APIリクエストで正しいキャラクター名を指定
  - サーバーログでキャラクター情報を確認

## 高度なカスタマイズ

### アニメーション追加
1. **Window** → **Animation** → **Animation**
2. **LoadingIndicator** に回転アニメーションを追加
3. **ResultText** にフェードインアニメーションを追加

### ローカライゼーション対応
1. **Localization Package** をインストール
2. **Text** コンポーネントを **Localize String Event** に置き換え

### パフォーマンス最適化
1. 大量のテキストには **Object Pooling** を適用
2. **Canvas** を適切に分割
3. **Text** の **Rich Text** オプションを適切に使用

## まとめ
このマニュアルに従ってUIを設定することで、『運命の館 〜HikukaHikanaika〜』のドラマチックなリアリティーショー機能とAI連携が正常に動作します。

新しい機能:
- **詩的で文学的な物語表現**: 夕暮れの館を舞台にした美しい描写
- **詳細なキャラクター設定**: 詩織、るな、愛美の3人の魅力的なキャラクター
- **ドラマチックなUI表示**: 物語調にフォーマットされた結果表示
- **心理描写重視**: 内面の美しさや秘めた想いを表現

問題が発生した場合は、トラブルシューティングセクションを参照してください。
美しき物語の世界をお楽しみください。
# シーン遷移セットアップガイド

## 📋 実装内容

PrologueSerif.cs に GameProtoScene への遷移機能を追加しました。

## 🔧 追加された機能

### 1. シーン遷移の自動実行
- プロローグの全セリフ表示後、自動的に次のシーンに遷移
- 「ガチャの世界へ...」メッセージを表示してからスムーズに遷移

### 2. 設定可能なパラメータ
```csharp
[Header("Settings")]
public string nextSceneName = "GameProtoScene";  // 遷移先シーン名

[Header("Scene Transition")]
public float transitionDelay = 1.0f;  // 遷移までの待機時間
```

### 3. エラーハンドリング
- シーン名による遷移が失敗した場合、インデックスでの遷移を試行
- 完全に失敗した場合はエラーメッセージを表示

## ⚙️ Unity Editor設定手順

### Step 1: Build Settings に GameProtoScene を追加

1. **File → Build Settings** を開く
2. **Add Open Scenes** をクリック（現在開いているシーンを追加）
3. または **Scenes In Build** エリアにシーンファイルをドラッグ&ドロップ

#### 推奨シーン順序
```
Scenes In Build:
[0] PrologueScene      ←── プロローグシーン
[1] GameProtoScene     ←── ゲーム本編シーン（ガチャシステム）
```

### Step 2: GameProtoScene を作成（存在しない場合）

1. **Assets → Create → Scene** で新しいシーンを作成
2. シーン名を「GameProtoScene」に変更
3. 既存の SampleScene をコピーして GameProtoScene にリネームでも可

### Step 3: Inspector での設定確認

PrologueSerif コンポーネントの Inspector で以下を確認：

```
┌─ Prologue Serif (Script) ─┐
│                           │
│ UI References             │
│ ├─ Dialogue Text  [設定済] │
│ └─ Clickable Area [設定済] │
│                           │
│ Settings                  │
│ ├─ Text Speed: 0.05       │
│ └─ Next Scene Name:       │
│    "GameProtoScene"       │  ←── 遷移先シーン名
│                           │
│ Scene Transition          │
│ └─ Transition Delay: 1.0  │  ←── 遷移待機時間（秒）
└───────────────────────────┘
```

## 🎮 動作の流れ

### 実行時の動作
1. プロローグのセリフを順次表示
2. 最後のセリフ「さあ、引くのです。」をクリック
3. 「ガチャの世界へ...」メッセージを表示
4. 1秒待機（transitionDelay）
5. GameProtoScene に自動遷移

### デバッグログ
```
描画開始
クリックされました
プロローグ終了。シーン遷移開始...
```

## 🔧 カスタマイズオプション

### 遷移先シーンの変更
```csharp
// Inspector で設定するか、コード内で変更
nextSceneName = "YourCustomScene";
```

### 遷移タイミングの調整
```csharp
// より長い待機時間にする場合
transitionDelay = 2.0f;

// 即座に遷移する場合
transitionDelay = 0f;
```

### カスタム遷移効果の追加
```csharp
IEnumerator TransitionToNextScene()
{
    // フェードアウト効果を追加したい場合
    // fadePanel.DOFade(1f, 0.5f);
    // yield return new WaitForSeconds(0.5f);
    
    SceneManager.LoadScene(nextSceneName);
}
```

## ❗ トラブルシューティング

### シーンが見つからないエラー
```
Scene 'GameProtoScene' の読み込みに失敗しました
```
**解決方法:**
1. File → Build Settings を開く
2. GameProtoScene が Scenes In Build に含まれているか確認
3. シーン名のスペルが正確か確認

### Inspector設定が反映されない
**解決方法:**
1. PrologueSerif コンポーネントの Inspector を確認
2. Next Scene Name フィールドが正しく設定されているか確認
3. Play Mode を終了してから再度実行

### 遷移後にエラーが発生
**解決方法:**
1. GameProtoScene に必要なUI要素が配置されているか確認
2. GameProtoScene に EventSystem が存在するか確認
3. 必要なスクリプトコンポーネントが適切に設定されているか確認

これで PrologueSerif から GameProtoScene への自動遷移が完璧に動作します！
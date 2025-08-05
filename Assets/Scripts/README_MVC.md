# MVCアーキテクチャリファクタリング

このプロジェクトは、より保守しやすく拡張しやすいコードベースを実現するために、MVCアーキテクチャパターンに従ってリファクタリングされました。

## アーキテクチャ概要

### Model層 (`Models/`)
データ構造とビジネスロジックを担当

- **GachaItem.cs**: ガチャアイテムのデータ構造
- **PlayerData.cs**: プレイヤーの状態管理（所持金、装備、所持アイテム）
- **GachaModel.cs**: ガチャシステムのビジネスロジック（確率計算、アイテム抽選）
- **RealityShowModel.cs**: リアリティーショーの判定ロジックとライバルデータ

### View層 (`Views/`)
UI表示とユーザーインターフェースを担当

- **GachaView.cs**: ガチャ関連のUI表示（結果、ステータス、所持アイテム）
- **RealityShowView.cs**: リアリティーショー関連のUI表示（ライバル情報、判定結果）

### Controller層 (`Controllers/`)
ユーザー入力の処理とModelとViewの連携を担当

- **GachaController.cs**: ガチャシステムの制御（ボタンクリック処理、状態更新）
- **RealityShowController.cs**: リアリティーショーの制御（判定処理、結果生成）

## 旧システムとの互換性

- **BeautyGachaManagerRefactored.cs**: レガシーサポート用ラッパークラス
- 既存のUnityシーンとの互換性を保つために提供

## 利点

### 1. 責任の分離
- **Model**: データ管理とビジネスロジック
- **View**: UI表示のみ
- **Controller**: ユーザー入力とロジック制御

### 2. 保守性の向上
- 各クラスの責任が明確
- 変更時の影響範囲が限定される
- テストしやすい構造

### 3. 拡張性
- 新機能追加時に既存コードへの影響が最小限
- UI変更時にロジックに影響しない
- ロジック変更時にUIに影響しない

### 4. 再利用性
- ModelとControllerは他のViewでも利用可能
- 異なるプラットフォーム（モバイル、VR等）への対応が容易

## 使用方法

### Unity Editor設定
1. GameObjectにGachaControllerとGachaViewを追加
2. GameObjectにRealityShowControllerとRealityShowViewを追加
3. InspectorでView参照を適切に設定
4. UI要素（Button、Text等）をView経由で接続

### 新機能追加時の手順
1. Modelに新しいデータ構造やロジックを追加
2. Viewに新しいUI表示メソッドを追加
3. Controllerで新しい制御ロジックを実装
4. Unity Editorで参照を設定

## 移行ガイド

既存のBeautyGachaManagerを使用している場合：
1. 新しいMVCコンポーネントをシーンに追加
2. BeautyGachaManagerRefactored経由でアクセス
3. 段階的に新しいアーキテクチャに移行
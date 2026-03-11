# KaomojiClash スクリプトアーキテクチャ

このドキュメントは Assets/Scripts 配下の責務分割を、運用しやすい単位で整理したものです。

## 1. 全体構成

主要コンポーネントは次のレイヤーで構成されています。

- Flow: 戦闘進行・シーン遷移・コンテキスト制御
- Domain: プレイヤー、敵、戦闘、マップ、インベントリ
- Data: ScriptableObject による静的データ定義
- Presentation: UI、カメラ、演出、オーディオ

## 2. 主要スクリプト

### Flow / Orchestration

- BattleFlowManager.cs: 戦闘全体の進行制御
- SceneChangeManager.cs: シーン遷移管理
- Context.cs: 実行コンテキスト保持
- Area/AreaManager.cs: エリアの状態と遷移管理

### Player / Enemy

- Player/PlayerController.cs: プレイヤー制御の入口
- Player/PlayerMovement.cs: 発射・移動挙動
- Player/PlayerMental.cs: 体力や状態管理
- Player/PlayerReflect.cs: 反射処理
- Enemy/EnemyController.cs: 敵制御の入口
- Enemy/EnemyMovement.cs: 敵追従・移動
- Enemy/EnemyMental.cs: 敵の生存状態管理

### Combat Controllers

- Controller/WaveController.cs: Wave 進行制御
- Controller/EnemySpawnController.cs: 敵スポーン制御
- Controller/DropController.cs: ドロップ制御
- Controller/ResultController.cs: リザルト集計
- Controller/WallController.cs: 壁関連制御

### Build / Inventory

- Inventory/Inventory.cs: 所持データ
- Inventory/InventoryManager.cs: 所持品管理
- KaomojiParts/KaomojiPartsManager.cs: パーツ統合管理
- Player/PlayerUpgradeService.cs: 成長計算と反映

### Arena Item

- ArenaItem/AreaItemBase.cs: アリーナアイテム基底
- ArenaItem/ReboundPad.cs: 反射支援
- ArenaItem/HealingPad.cs: 回復床
- ArenaItem/SpeedUpArea.cs: 加速床
- ArenaItem/Medikit.cs: 接触回復アイテム

### Presentation

- AudioManager.cs: BGM/SE の一元制御
- Camera/CameraShake.cs: カメラ振動
- Camera/CameraZoom.cs: ズーム演出
- UI/CharacterDamageText.cs: ダメージ表示
- UI/MainUIs/*: 画面別 UI モジュール

## 3. ScriptableObject データ設計

データは ScriptableObject で管理し、ロジックと分離しています。

- Character: PlayerData, EnemyData, EnemyDatabase
- KaomojiData: KaomojiPartData, KaomojiPartsDatabase
- KaomojiSkill: UpgradeDefinition, 各種 Skill 定義
- ArenaItemData: アイテム設定とデータベース
- AreaData: エリアと難易度の静的設定
- AudioDatas: BGM/SE リソース定義

## 4. UI モジュール分割

Main UI は用途ごとにモジュール分割されています。

- Home: エリア情報、プレビュー、導線
- KaomojiBuild: パーツ選択、顔文字構成、ステータス表示
- ArenaBuild: アリーナ設置物の選択と構成
- Battle: ステータス、進行、獲得報酬
- TotalResult: まとめ表示、報酬集計

## 5. データフロー

1. ScriptableObject から基礎パラメータを読み込む
2. ビルド構成と成長値を合成して実ステータスを算出
3. Wave 進行中に敵スポーン、衝突、ドロップを処理
4. 戦闘終了後に報酬を Inventory へ反映
5. UI が結果と次導線を表示して次ループへ遷移

## 6. 保守運用ルール

- 新規ゲームルールはまず ScriptableObject に定義
- ランタイム制御は Controller 層に集約
- 一時演出は UI/OnlyForThis に隔離して副作用を限定
- 依存方向は Data -> Domain -> Presentation を基本に維持

## 7. 今後の拡張ポイント

- スキル条件の共通評価基盤を追加
- 難易度スケーリング式の外部設定化
- 戦闘ログ収集によるバランス検証の自動化

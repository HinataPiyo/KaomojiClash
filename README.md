# KaomojiClash

顔文字を組み立て、弾き、衝突させて戦う
ビルド主導型トップダウンアクションゲーム

⸻

## 1. Overview

KaomojiClash は、プレイヤーが独自の顔文字キャラクターを組み立て、アリーナで敵と激突させるトップダウン型アクションゲームです。

強さはキャラクターレベルではなく、**記号パーツの構成（ビルド）** によって決まります。プレイヤーは顔文字を引っ張って発射し、敵に衝突させることでダメージを与えます。収集した記号パーツを組み替えることで戦略的な深みが生まれ、「記号収集 → ビルド再構成 → 再挑戦」のループがゲームの核心を形成します。

⸻

## 2. Core Concept
	•	見た目ではなく「構成」が性能を決定
	•	衝突アクション × ビルド戦略
	•	記号収集 → 再構成 → 再挑戦 のループ

⸻

## 3. Core Gameplay Loop
	1.	エリア選択
	2.	戦闘開始（Wave進行）
	3.	敵撃破
	4.	記号ドロップ
	5.	ビルド再構成
	6.	より高難易度へ挑戦

⸻

## 4. Combat System

Player Action

	•	Drag & Release による発射
	•	壁反射
	•	条件付きスキル（例：Rebound Boost）

Enemy

	•	難易度別スポーン
	•	文化圏レベルによる強化

### Arena Battle System

アリーナ戦闘はWave制で進行し、プレイヤーと敵のキャラクターステータスが戦況に直接影響します。

**プレイヤーステータス（`CharacterStatus`）**

| ステータス | 説明 |
|---|---|
| Speed | 発射速度・移動速度に影響 |
| Power | 衝突時のダメージ量に影響 |
| Guard | 受けるダメージを軽減 |
| Stamina | キャラクターの最大体力に影響 |

各ステータスは装備しているKaomojiパーツの合計値によって決定されます（`Status.UpdateTotalPartsParameter`）。

**メディキット回復メカニズム**

アリーナ内に配置される `Medikit` アイテムは、プレイヤーが触れると最大体力の一定割合（グレードに応じた割合）を即時回復します。使用後はクールタイムが発生し、時間経過で再使用可能になります。グレードは `None / MK-1 / MK-2` の3段階があり、上位グレードほど回復量・クールタイムが優れています。

⸻

## 5. Build System

### Character Customization

顔文字キャラクターはモジュール設計されており、以下の**5つのパーツスロット**で構成されます。各スロットに割り当てる記号パーツを変更することで、キャラクターの外見と性能が変化します。

| パーツ | スロット | 説明 |
|---|---|---|
| Mouth（口） | 中央 | 最も影響度が高い主力パーツ（倍率 ×1.0） |
| Eyes（目） | 左右 | サブ性能を担う（倍率 ×0.5） |
| Hands（手） | 装飾的 | 補助ステータスに貢献（倍率 ×0.3） |
| Decoration First（装飾1） | 追加 | 微細なステータス補正（倍率 ×0.1） |
| Decoration Second（装飾2） | 追加 | 微細なステータス補正（倍率 ×0.1） |

パーツは戦闘でのドロップや進行報酬として段階的にアンロックされ、プレイヤーは強力なパーツを収集するにつれてビルドの幅が広がります。パーツは重複装備・レベルアップが可能で、経験値を獲得するごとに性能が成長します。

### Growth System

ステータス上昇は以下で決定：

`Final Growth = Base Growth × Growth Rate`

成長率タイプ（`GrowthRateType`）は5段階あり、パーツごとに設定されています：

| タイプ | 倍率 |
|---|---|
| VeryLow | 0.85× |
| Low | 0.92× |
| Normal | 1.00× |
| High | 1.15× |
| VeryHigh | 1.20× |

⸻

## 6. Difficulty Structure

### Stage System and Difficulty Levels

ゲームは複数の難易度をサポートし、ステージが進むにつれて敵が強化されます。

| 難易度 | ステータス倍率 | 獲得経験値倍率 | 表示色 |
|---|---|---|---|
| Easy | ×0.95 | ×1.00 | 緑 |
| Normal | ×1.00 | ×1.25 | 黄 |
| Hard | ×1.15 | ×1.50 | オレンジ |
| Extreme | ×1.25 | ×1.75 | 赤 |

難易度が上がるほど敵のステータス倍率が増加し、同時に獲得できる報酬（経験値・マネー）も増加します。低難易度のステージはより高い難易度への準備フェーズとして設計されており、段階的なアンロック構造によってプレイヤーはビルドを強化しながら挑戦の難易度を上げていきます。

⸻

## 7. Arena System

戦闘時に生成されるアリーナ内に
設置型オブジェクトを配置可能。

例：

	•	Rebound Pad
	•	Heal Pad
	•	Acceleration Area
	•	Recovery Kit（Medikit）

⸻

## 8. Skills

タグベースのスキルシステム。
スキルは重複可能で同じスキルがあればレベルが上昇し、スキルの効果も上昇する

例：

	•	Shunsoku
	•	Muscle Boost
	•	Iron Wall
	•	Rebound Boost（条件型）

⸻

## 9. Audio Management

`AudioManager` はシングルトンとして動作し、BGM・SEの再生を一元管理します。

### Dynamic BGM Switching

シーン遷移や戦闘フェーズの変化に応じて、再生するBGMを動的に切り替えます。

| フェーズ | BGM |
|---|---|
| ホーム画面 | `Home` |
| エンカウント | BGMを停止 → SE `Encount` 再生 |
| 戦闘開始 | `StartBattle` |
| 戦闘終了 | `EndBattle` |
| ステージクリア | `TotalResult_StageClear` |
| ステージ失敗 | `TotalResult_StageFailed` |

### Fade Transitions

BGMの切り替えはコルーチン（`BGMFadeTransition`）によって制御されます。

- **フェードアウト**: 現在再生中のBGMを `bgmFadeDuration` 秒かけて音量ゼロまで減衰させてから停止
- **フェードイン**: 新しいBGMを音量ゼロから開始し、`bgmFadeDuration` 秒かけてフルボリュームまで上昇

これにより、シーン・フェーズ間のBGM切り替えが滑らかに行われます。複数の切り替えが連続して発生した場合も、前回のコルーチンを中断してから新たなフェード処理を開始します。

⸻

## 10. UI and Effects

### Damage Feedback

戦闘中にキャラクターが受けたダメージは `CharacterDamageText`（TextMeshProUGUI）によってワールド空間に表示されます。ダメージ値は数値に丸めて表示され、ダメージの種類に応じてテキストカラーを変更することで視覚的なフィードバックを提供します。

### Camera Shake

`CameraShake`（Cinemachine `BasicMultiChannelPerlin` を使用）により、衝突や強攻撃などの激しいゲームイベント時にカメラ振動エフェクトを発生させます。

```csharp
CameraShake.I.ApplyShake(amplitudeGain, frequencyGain, duration);
```

- **amplitudeGain**: 振動の振幅（大きいほど激しい揺れ）
- **frequencyGain**: 振動の周波数（大きいほど細かい揺れ）
- **duration**: 振動の持続時間（秒）

指定した時間が経過するとパラメータはリセットされ、カメラが通常状態に戻ります。

⸻

## 11. Technical Stack
	•	Unity
	•	C#
	•	TextMeshPro
	•	ScriptableObject設計
	•	Top-Down Physics Based Combat
	•	Cinemachine（Camera Shake / Zoom）
	•	Unity Input System

⸻

## 12. Current Status

Implemented

	•	Core combat loop
	•	Wave progression
	•	Build system
	•	Arena objects
	•	Skill system (basic)
	•	Hit effects / Hit stop / Hit zoom
	•	Afterimage system (Rebound Boost)
	•	Dynamic BGM switching with fade transitions
	•	Camera shake via Cinemachine
	•	Damage text feedback UI

### In Progress
	•	Balance tuning
	•	Additional skill conditions
	•	UI refinement

### Planned
	•	More build variations
	•	Additional arena mechanics
	•	Extended difficulty scaling

⸻

## 13. Design Philosophy
	•	プレイヤー成長より「ビルド成長」を重視
	•	数値ではなく構成で強さが変わる設計
	•	シンプル操作 × 奥行きビルド

⸻

## 14. Future Direction
	•	Online expansion (Trophy-based matchmaking)
	•	More combinational bonuses
	•	Expanded ASCII environment system


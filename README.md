# KaomojiClash

顔文字を組み立て、弾き、衝突させて戦う
ビルド主導型トップダウンアクションゲーム

⸻

## 1. Overview

KaomojiClash は
顔文字（記号構成）によって強さが決定する
トップダウン型アクションゲームです。

プレイヤーは顔文字を引っ張って発射し、
敵に衝突させることでダメージを与えます。

強さはレベルではなく、
記号構成（ビルド）によって決定されます。

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

⸻

## 5. Build System

顔文字は以下のパーツで構成されます：
	•	Outline
	•	Eyes
	•	Mouth
	•	Decoration

各パーツは以下のステータスを持ちます：
	•	Speed
	•	Power
	•	Defence
	•	Stamina

### Growth System

ステータス上昇は以下で決定：
'Final Growth = Base Growth × Growth Rate'

## 6. Difficulty Structure
	•	複数難易度
	•	段階的アンロック
	•	文化圏レベルによる敵強化

低難易度は高難易度への準備フェーズとして設計。

⸻

## 7. Arena System

戦闘時に生成されるアリーナ内に
設置型オブジェクトを配置可能。

例：
	•	Rebound Pad
	•	Heal Pad
	•	Acceleration Area
	•	Recovery Kit

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

## 9. Technical Stack
	•	Unity
	•	C#
	•	TextMeshPro
	•	ScriptableObject設計
	•	Top-Down Physics Based Combat

⸻

## 10. Current Status

Implemented
	•	Core combat loop
	•	Wave progression
	•	Build system
	•	Arena objects
	•	Skill system (basic)
	•	Hit effects / Hit stop / Hit zoom
	•	Afterimage system (Rebound Boost)

### In Progress
	•	Balance tuning
	•	Additional skill conditions
	•	UI refinement

### Planned
	•	More build variations
	•	Additional arena mechanics
	•	Extended difficulty scaling

⸻

## 11. Design Philosophy
	•	プレイヤー成長より「ビルド成長」を重視
	•	数値ではなく構成で強さが変わる設計
	•	シンプル操作 × 奥行きビルド

⸻

## 12. Future Direction
	•	Online expansion (Trophy-based matchmaking)
	•	More combinational bonuses
	•	Expanded ASCII environment system


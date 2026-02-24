# KaomojiClash

> 顔文字を組み立て、弾き、衝突させて戦う  
> ビルド主導型トップダウンアクションゲーム

---

## Status

**Prototype / Active Development**

---

## Overview

**KaomojiClash** は、  
顔文字（記号構成）によって強さが決定するトップダウンアクションゲームです。

プレイヤーは顔文字を引っ張って発射し、  
敵に衝突させてダメージを与えます。

強さはレベルではなく、  
**記号構成（ビルド）によって決定されます。**

---

## Core Concept

- 見た目ではなく「構成」が性能を決定
- 衝突アクション × ビルド戦略
- 記号収集 → 再構成 → 再挑戦 のループ構造

---

## Core Gameplay Loop

1. エリア選択
2. 戦闘開始（Wave進行）
3. 敵撃破
4. 記号ドロップ
5. ビルド再構成
6. より高難易度へ挑戦

---

## Combat System

### Player Action
- Drag & Release による発射
- 壁反射
- 条件付きスキル発動（例：Rebound Boost）

### Enemy System
- 難易度別スポーン
- 文化圏レベルによる強化
- 平均レベル表示による強さ可視化

---

## Build System

顔文字は以下のパーツで構成されます：

- Outline
- Eyes
- Mouth
- Decoration

各パーツは以下のステータスを持ちます：

- Speed
- Power
- Defence
- Stamina

---

## Growth System

ステータス上昇は以下の式で決定されます：Final Growth = Base Growth × Growth Rate

- Base Growth：基礎成長量
- Growth Rate：成長率（段階式）

---

## Difficulty Structure

- 複数難易度制
- 段階的アンロック
- 文化圏レベルによる敵強化

低難易度は高難易度へ挑戦するための準備段階として設計。

---

## Arena System

戦闘時に生成されるアリーナ内に設置型オブジェクトを配置可能。

例：

- Rebound Pad
- Heal Pad
- Acceleration Area
- Recovery Kit

---

## Skill System

タグベースのスキル構造。

例：

- Shunsoku
- Muscle Boost
- Iron Wall
- Rebound Boost（条件型スキル）

---

## Technical Stack

- Unity
- C#
- TextMeshPro
- ScriptableObject設計
- Physicsベース衝突処理

---

## Implemented Features

- コア戦闘ループ
- Wave進行
- 記号ビルドシステム
- アリーナ設置システム
- 基礎スキル実装
- Hit Effect / Hit Stop / Hit Zoom
- Afterimage（条件付きスキル連動）

---

## In Progress

- バランス調整
- スキル拡張
- UI最適化
- 背景アスキーアート強化

---

## Planned

- 記号バリエーション追加
- 組み合わせボーナス拡張
- オンライン対応（将来的構想）
- 難易度拡張

---

## Design Philosophy

- プレイヤー成長より「ビルド成長」を重視
- シンプル操作 × 戦略構成
- 数値より構成の意味を重視

---

## Future Direction

- ビルド特化型拡張
- 高難易度チャレンジ設計
- トロフィー制度（将来的オンライン構想）
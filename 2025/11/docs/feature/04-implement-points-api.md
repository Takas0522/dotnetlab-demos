---
title: 実装: ポイント付与/消費/照会 API 実装
assignee: coding-agent
effort: large
dependencies: ["実装: PointsService プロジェクト骨格作成", "設計: Points API と DB モデル確定"]
---

## 概要
`/api/points/grant`, `/api/points/consume`, `/api/points/{userId}/balance`, `/api/points/{userId}/transactions`, `/api/points/me/*` などを実装します。DB トランザクションを用いた FIFO 消費と監査ログ記録を行います。

## 受け入れ基準
- すべての主要 API が実装され、単体テストで主要なロジック（付与、消費、残高計算）が検証できること。

## 実装ノート
- 消費は古い allocation から順に減算する（FIFO）。
- 部分消費が発生する場合には `PointsAllocations.RemainingAmount` を更新する。
- 監査のため `PointsTransactions` にすべてのイベントを記録する。

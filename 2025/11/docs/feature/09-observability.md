---
title: observability: OpenTelemetry 計装
assignee: dev
effort: small
dependencies: ["実装: PointsService プロジェクト骨格作成"]
---

## 概要
PointsService に OpenTelemetry トレースとメトリクスを追加します。主要エンドポイント（grant/consume/expire）のカウント、エラーレート、レイテンシを収集します。

## 受け入れ基準
- トレースとメトリクスが既存の観測基盤に送信されること。

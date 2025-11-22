---
title: 実装: BFF 連携ルート追加
assignee: dev
effort: small
dependencies: ["実装: ポイント付与/消費/照会 API 実装"]
---

## 概要
BFF にフロント用の集約エンドポイントを追加します。例: `/bff/users/me/points`, `/bff/users/{id}/points`, `/bff/users/{id}/points/transactions`。BFF はクライアントの JWT（Cookie など）を利用して `PointsService` へ Authorization ヘッダを付与して転送します。

## 受け入れ基準
- フロントから BFF を経由して Points の残高・履歴が取得できること。

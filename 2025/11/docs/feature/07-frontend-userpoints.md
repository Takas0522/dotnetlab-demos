---
title: 実装: フロント - ユーザー管理ページにポイント表示追加
assignee: dev
effort: small
dependencies: ["実装: BFF 連携ルート追加"]
---

## 概要
Angular のユーザー管理ページにポイント残高と履歴表示を追加します。新しい `PointsService` を実装し、BFF の `/bff/users/me/points` と `/bff/users/me/points/transactions` を呼び出します。

## 受け入れ基準
- ユーザー詳細画面から自身のポイント残高と最近の履歴が確認できること。

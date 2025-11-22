---
title: 実装: ExpirePointsJob（期限切れ処理）
assignee: dev
effort: medium
dependencies: ["実装: ポイント付与/消費/照会 API 実装"]
---

## 概要
日次バッチで `PointsAllocations` の `ExpiresAt <= UTC Now` を検出して `Status` を `expired` に更新し、`PointsTransactions` に expire レコードを追加します。`PointsBalances` を更新して整合性を保ちます。

## 受け入れ基準
- 日次バッチが実行され、失効処理が DB に反映されること（テスト環境で確認）。

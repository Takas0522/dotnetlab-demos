---
title: テスト: 単体・統合・E2E テスト作成
assignee: dev
effort: medium
dependencies: ["実装: ポイント付与/消費/照会 API 実装", "実装: ExpirePointsJob（期限切れ処理）"]
---

## 概要
競合消費、失効、部分消費、BFF 経由での UI 表示などをカバーする単体・統合・E2E テストを用意します。CI で自動実行されること。

## 受け入れ基準
- 主要シナリオのテストが CI でパスすること。

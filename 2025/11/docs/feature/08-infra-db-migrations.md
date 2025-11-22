---
title: infra: DB マイグレーションとデプロイ構成
assignee: dev
effort: medium
dependencies: ["実装: ポイント付与/消費/照会 API 実装"]
---

## 概要
PointsService 用の DB マイグレーション（EF Core または Flyway）を用意し、Docker イメージ、Kubernetes マニフェスト、CI でのマイグレーション適用フローを追加します。

## 受け入れ基準
- マイグレーションが CI 上で実行され、コンテナ起動時にスキーマが適用されること。

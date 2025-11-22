# ファイル共有サービス (File Sharing Service)

ファイル共有サービスはファイルのアップロード、メタデータ管理、アクセス制御を担当します。実際のファイルは Azure Blob Storage 等の外部ストレージに保存し、サービスはメタデータと参照 URL を管理します。

## 主な責務
- ファイルアップロードの受け入れ（直接アップロードまたは署名付き URL を返す）
- メタデータ管理（所有者、ファイル名、サイズ、contentType、タグ、共有設定）
- アクセス制御（公開/非公開、共有リンク、アクセスログ）

## API 契約（代表的）
- POST /api/files/upload
  - 入力: { fileName, contentType, ownerId, metadata }
  - 出力: { uploadUrl, fileId } （署名付き URL を返すパターン）
- POST /api/files/complete
  - 入力: { fileId, blobPath, size, checksum }
  - 出力: 200
- GET /api/files/{id}
  - 出力: { fileId, ownerId, fileName, url (SAS), metadata }
- DELETE /api/files/{id}
  - 出力: 204

## DB スキーマ（概略）
- Files
  - id (PK), ownerId, fileName, blobPath, size, contentType, checksum, isPublic, createdAt, updatedAt

## ストレージ連携
- 大きなファイルはクライアントが直接 Blob Storage に PUT するための署名付き URL を BFF・File Service が発行する。
- アップロード完了後、クライアントは /api/files/complete を呼び出し、サービスがメタデータを確定する。

## 同期と参照整合性
- File Service は ownerId の変更やユーザー削除イベントを User Management の Change Feed で受け、メタデータを更新またはアクセス権を調整する。

## セキュリティ
- 署名付き URL の有効期限を短くし、SAS ポリシーを最小権限に設定する。
- アクセス制御はサービスレベルで検証し、Blob Storage のパブリック設定は極力避ける。

## 運用
- ストレージコストの可視化とライフサイクルルール（アーカイブ、削除）を設定する。
- 大量アップロード時のスロットリングと再試行設計を実装する。

---

作成者: GitHub Copilot デモ用アウトライン

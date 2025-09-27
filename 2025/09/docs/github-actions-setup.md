# GitHub Actions設定ガイド

このドキュメントでは、リポジトリのGitHub Actionsワークフローの設定方法について説明します。

## 必要なSecrets設定

GitHub Actionsが正常に動作するには、以下のSecretsをリポジトリに設定する必要があります。

### Azure OpenAI関連

1. **AZURE_OPENAI_ENDPOINT**
   - Azure OpenAIリソースのエンドポイントURL
   - 例: `https://your-resource-name.openai.azure.com`

2. **AZURE_OPENAI_API_KEY**
   - Azure OpenAIのAPIキー
   - Azure Portalのリソース > キーとエンドポイントから取得

3. **AZURE_OPENAI_DEPLOYMENT**
   - デプロイしたモデルの名前
   - 例: `gpt-4o-mini`、`gpt-4`など

### Secrets設定手順

1. GitHubリポジトリのページを開く
2. **Settings** タブをクリック
3. 左サイドバーの **Secrets and variables** > **Actions** をクリック
4. **New repository secret** ボタンをクリック
5. Name欄にSecret名を入力
6. Value欄に対応する値を入力
7. **Add secret** ボタンをクリック

## ワークフロー詳細

### 1. 自動Issue分析とラベル付与 (`auto-label-issues.yml`)

**トリガー**: Issueが開かれた時（`issues: opened`）

**処理フロー**:
1. **コードコンテキスト収集**: プロジェクトの主要ファイル（.cs, .ts, .js等）を収集・分析
2. **ラベル取得**: GitHub APIでリポジトリの全ラベルと説明を取得
3. **AI分析**: Azure OpenAI APIでIssueの内容とコードコンテキストを分析
4. **ラベル選択**: 最大3つの適切なラベルを選択
5. **ラベル適用**: 選択されたラベルをIssueに自動適用
6. **コメント追加**: 適用結果をIssueにコメント

**AIプロンプト戦略**:
- プロジェクトの主要コードファイルを自動収集・分析
- 利用可能なラベル一覧を提供し、一覧からのみ選択するよう厳格に指示
- Issueタイトル、本文、コードコンテキストを総合的に分析
- 技術領域（frontend, backend, database等）の判定精度向上
- 適切なラベルが見つからない場合は`needs-triage`を適用
- レスポンス形式：コンマ区切りのラベル名リスト
- 存在しないラベルの使用を防ぐエラーハンドリング（HTTP 422対応）

### 2. Issue回答支援 (`ai-answer.yml`)

**トリガー**: Issueが開かれた時（`issues: opened`）

**処理フロー**:
1. **コードコンテキスト収集**: プロジェクトの主要ファイルを収集
2. **AI分析**: コードコンテキストを含めてIssueを分析
3. **回答生成**: 技術的な解決策や説明を生成
4. **コメント投稿**: 生成された回答をIssueにコメント

### 3. Discussion回答支援 (`discussion-answer.yml`)

**トリガー**: Discussionが作成された時（`discussion: created`）

**処理**: Issue回答支援と同様のフローでDiscussionに回答

## Azure OpenAI設定

### 1. Azure OpenAIリソースの作成

1. Azure Portalにアクセス
2. **リソースを作成** > **AI + Machine Learning** > **Azure OpenAI**
3. 必要な情報を入力してリソースを作成

### 2. モデルのデプロイ

1. Azure OpenAI Studioにアクセス
2. **Deployments** > **Create new deployment**
3. モデル（GPT-4o-mini推奨）を選択
4. デプロイメント名を設定（例: `gpt-4o-mini`）

### 3. APIキーとエンドポイントの取得

1. Azure Portalでリソースを開く
2. **Keys and Endpoint** から以下を取得:
   - Key 1または Key 2（API_KEY）
   - Endpoint（ENDPOINT）

## トラブルシューティング

### よくある問題と解決方法

#### 1. ワークフローが動作しない
- **確認事項**: 
  - 必要なSecretsが全て設定されているか
  - Azure OpenAIリソースが正常にデプロイされているか
  - GitHub Actionsが有効になっているか

#### 2. Azure OpenAI APIエラー
- **403 Forbidden**: API Keyが正しくないか、リソースへのアクセス権限がない
- **404 Not Found**: エンドポイントURLまたはデプロイメント名が間違っている
- **429 Too Many Requests**: レート制限に達している

#### 3. ラベルが適用されない
- **確認事項**:
  - GitHub Actionsにissues: write権限があるか
  - 指定されたラベルがリポジトリに存在するか（HTTP 422エラー）
  - AI応答が正しい形式で返されているか
  - AIが既存のラベル一覧から選択しているか

#### 4. AIが存在しないラベルを提案する
- **対処法**:
  - プロンプトでより厳格にラベル一覧の制約を強調
  - 温度パラメータを下げて、より確実性の高い応答を得る
  - ラベル一覧の形式を改善（説明付きなど）

### ログの確認方法

1. GitHubリポジトリの **Actions** タブを開く
2. 該当するワークフロー実行をクリック
3. 各ステップのログを確認
4. エラーメッセージを確認して問題を特定

## カスタマイズ

### コードコンテキスト収集の調整

より精度の高いラベル選択のために、以下をカスタマイズできます：

1. **対象ファイルタイプの調整**：
```bash
# 例: SQLファイルも含める
find src -type f \( -name "*.cs" -o -name "*.ts" -o -name "*.js" -o -name "*.sql" -o -name "*.json" \) | head -20
```

2. **収集するファイル数の調整**：
```bash
# 例: より多くのファイルを収集
head -20  # を head -30 に変更
```

3. **コンテキストサイズの調整**：
```bash
# 例: より大きなコンテキスト
head -c 4000  # を head -c 6000 に変更
```

### AIプロンプトの調整

Issue分析の精度を向上させるために、以下を調整できます：

1. **システムメッセージの修正** (`auto-label-issues.yml`の`SYSTEM_MSG`)
2. **温度パラメータの調整** (`temperature`: 0.1 - 1.0)
3. **最大トークン数の調整** (`max_tokens`)

### 対象ラベルの制限

特定のラベルのみを対象にしたい場合は、ラベル取得処理でフィルタリングを追加できます。

```bash
# 例: "bug", "enhancement", "documentation"のみ対象
jq -r '.[] | select(.name | test("^(bug|enhancement|documentation)$")) | "\(.name): \(.description // "説明なし")"' labels.json > labels_list.txt
```

## ベストプラクティス

1. **定期的な監視**: ワークフローの実行結果を定期的に確認
2. **コスト管理**: Azure OpenAI APIの使用量を監視
3. **精度向上**: AI応答の品質を定期的に評価・改善
4. **セキュリティ**: APIキーなどの機密情報を適切に管理

## さらなる改善案

- [ ] ラベル付与の精度測定と改善
- [ ] 複数言語対応（英語Issue対応）
- [ ] カスタムラベルの自動提案
- [ ] Issue優先度の自動判定
- [ ] プロジェクト固有の分析ルール追加
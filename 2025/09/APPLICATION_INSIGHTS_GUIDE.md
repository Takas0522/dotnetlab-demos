# Application Insights 統合ガイド

このプロジェクトでは、WebAPIとAngularフロントエンドの両方でApplication Insightsを使用してエラーキャッチとパフォーマンス測定を行います。

## 🔧 設定手順

### 1. Azure Application Insightsリソースの作成

1. Azure Portalにログイン
2. 「Application Insights」を検索して選択
3. 「作成」をクリック
4. 必要な情報を入力：
   - リソースグループ: 新規作成または既存を選択
   - 名前: `todo-app-insights`（例）
   - リージョン: お好みのリージョン
   - リソースモード: クラシック または ワークスペースベース
5. 作成完了後、「概要」ページから**接続文字列**をコピー

### 2. WebAPI側の設定

#### appsettings.jsonの更新
```json
{
  "ApplicationInsights": {
    "ConnectionString": "ここにApplication Insightsの接続文字列を貼り付け"
  }
}
```

#### 実装済み機能
- ✅ カスタムミドルウェアによるリクエスト/レスポンスの自動追跡
- ✅ 例外の自動キャッチと詳細情報の送信
- ✅ パフォーマンスメトリクスの測定
- ✅ ユーザー情報の追跡
- ✅ 構造化ログ

### 3. Angular側の設定

#### environment.tsの更新
```typescript
export const environment = {
  // ...他の設定...
  applicationInsights: {
    connectionString: 'ここにApplication Insightsの接続文字列を貼り付け'
  }
};
```

#### 実装済み機能
- ✅ HTTPリクエストの自動追跡
- ✅ グローバルエラーハンドリング
- ✅ ページビューの追跡
- ✅ ユーザーアクションの追跡
- ✅ パフォーマンス測定

## 📊 追跡される情報

### WebAPI側
- **リクエスト情報**: エンドポイント、レスポンス時間、ステータスコード
- **例外情報**: スタックトレース、エラーメッセージ、コンテキスト情報
- **パフォーマンス**: API レスポンス時間、エラー率
- **ユーザー情報**: 認証済みユーザーのID

### Angular側
- **ページビュー**: ページ遷移、滞在時間
- **HTTPリクエスト**: API呼び出しの成功/失敗、レスポンス時間
- **ユーザーアクション**: ToDo作成、編集、削除、フィルタリング
- **エラー情報**: JavaScriptエラー、HTTPエラー
- **パフォーマンス**: 操作の実行時間

## 🔍 Application Insightsで確認できるメトリクス

### カスタムイベント
- `TodoList.PageLoad` - ToDoリストページの読み込み
- `TodoList.TodoCreated` - ToDo作成
- `TodoList.TodoToggled` - ToDo完了状態の切り替え
- `TodoList.TodoDeleted` - ToDo削除
- `TodoList.FilterChanged` - フィルター変更
- `TodoList.SearchPerformed` - 検索実行

### カスタムメトリクス
- `API.ResponseTime` - APIレスポンス時間
- `API.Errors` - APIエラー数
- `HTTP.RequestDuration` - HTTPリクエスト時間
- `Frontend.Errors` - フロントエンドエラー数
- `TodoList.LoadedCount` - 読み込まれたToDo数

### 依存関係の追跡
- HTTP APIコール
- データベースクエリ（WebAPI側）
- 外部サービス呼び出し

## 🚀 使用方法

### 開発環境での確認

1. Application Insightsの接続文字列を設定
2. アプリケーションを起動
3. 通常通りアプリを使用
4. Azure PortalのApplication Insightsで以下を確認：
   - **ライブメトリクス**: リアルタイムの使用状況
   - **アプリケーションマップ**: 依存関係の可視化
   - **パフォーマンス**: レスポンス時間の分析
   - **エラー**: 例外とエラーの詳細
   - **ログ**: カスタムイベントとトレース

### よく使用するKQLクエリ

#### エラー率の確認
```kql
requests
| where timestamp > ago(1h)
| summarize 
    Total = count(),
    Errors = countif(success == false),
    ErrorRate = round(100.0 * countif(success == false) / count(), 2)
by bin(timestamp, 5m)
| order by timestamp desc
```

#### 最も時間のかかるAPI呼び出し
```kql
requests
| where timestamp > ago(1h)
| top 10 by duration desc
| project timestamp, name, duration, resultCode, success
```

#### ユーザーアクションの頻度
```kql
customEvents
| where timestamp > ago(1h)
| where name startswith "TodoList."
| summarize count() by name
| order by count_ desc
```

## 🔧 トラブルシューティング

### データが表示されない場合
1. 接続文字列が正しく設定されているか確認
2. Azure Portalでデータの取り込みが有効になっているか確認
3. ファイアウォールの設定を確認
4. 数分待ってから再確認（データの反映には時間がかかる場合があります）

### エラーが多発する場合
1. **ライブメトリクス**でリアルタイムのエラーを確認
2. **エラー**セクションで詳細なスタックトレースを確認
3. **ログ**でカスタムトレースを確認

## 📈 ダッシュボード作成

Application Insightsのデータを使用してカスタムダッシュボードを作成できます：

1. Azure Portal > Application Insights > ダッシュボード
2. よく使用するクエリをピン留め
3. アラートの設定（エラー率が高い場合など）

## 🏃‍♂️ パフォーマンス最適化

Application Insightsのデータを使用して以下を最適化できます：

- 遅いAPIエンドポイントの特定
- よく使用される機能の分析
- エラーが多発する箇所の改善
- ユーザーエクスペリエンスの向上

---

より詳細な情報については、[Microsoft Application Insights ドキュメント](https://docs.microsoft.com/azure/azure-monitor/app/app-insights-overview)を参照してください。

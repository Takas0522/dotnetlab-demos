# MCPサーバーに認証機能とAzureサーチを実行する機能を追加してみた話

# 注意

- 2025年5月時点の情報です。
- 当時ベストプラクティスが提供されていないのでテストコードや実装を見ながら「こうしたらいけるんやろな」という程度の実験モノです。
- 参照次点でベストプラクティス/ライブラリのAPIが提供されている可能性があるのでまずは[SDKの公式リポジトリ](https://github.com/modelcontextprotocol/csharp-sdk)を参考にしてください。

## MCPサーバー側の実装として

### Azureサーチを実施する機能

[Program.cs](./McpServerSample/Program.cs)

`AddAzureClients`でAzureサーチを利用できる準備をする(今回はちょっと雑だがAPIキーを試用している。MIの方が絶対良い)

### 認証を実施する機能

ミドルウェアで認証を実施する。

[こちらの記事](https://philippbauknecht.medium.com/authentication-authorization-in-azure-functions-with-azure-active-directory-using-c-net-aad52c8de925)をめっちゃ参考にした。

多分もっといい方法がありそうな気がしている。

## Client側の実装

Sampleでは特に言及ないけど、[内部的にはHttpClientを利用した単純なHTTP通信っぽい](https://github.com/modelcontextprotocol/csharp-sdk/blob/c750f09e9770ccc8c98b61d1b0f21ae159793042/src/ModelContextProtocol/Protocol/Transport/SseClientTransport.cs)

テストコード見る感じ、`ClientTransport`を作成する時にHttpClientを食わせる事ができるらしい。

なので、HttpClientの通信実行時にmsal介してアクセストークンを取得してヘッダに設定するデリゲートを作成し、HttpClientのインスタンス作成時にそのデリゲートを利用するように構成を行った。

## 結果

良い感じに動くようになる
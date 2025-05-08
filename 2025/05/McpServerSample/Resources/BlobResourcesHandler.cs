using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using ModelContextProtocol.Protocol.Types;

namespace McpServerSample.Resources
{
    public class BlobResourcesHandler
    {
        private readonly BlobServiceClient _client;
        private readonly BlobContainerClient _containerClient;
        private readonly HttpClient _httpClient;

        private readonly string accountName = "";
        private readonly string containerName = "";
        private readonly string accountKey = "";
        private readonly string baseUri = "";

        public BlobResourcesHandler(
            BlobServiceClient client,
            IConfiguration config
        ) {
            _client = client;

            accountName = config["Blob:accountName"];
            containerName = config["Blob:containerName"];
            accountKey = config["Blob:accountKey"];
            baseUri = config["Blob:baseUri"];

            _containerClient = _client.GetBlobContainerClient(containerName);

        }

        public async ValueTask<ListResourceTemplatesResult> GetListResourceTemplatesAsync()
        {
            var url = baseUri + containerName + "/{fileName}";

            return new ListResourceTemplatesResult
            {
                ResourceTemplates =
                    [
                        new ResourceTemplate {
                            Name = "Wikiリソーステンプレートリスト",
                            Description = "fileNameをIDにしたテンプレートリストです",
                            UriTemplate = url
                        }
                    ]
            };
        }

        public async ValueTask<ListResourcesResult> GetListResourceAsync()
        {
            // BLOBの一覧を作成。URLはSASURLで構成し外部（あらゆるClient）からアクセスできるように。mimeTypeはtext/markdownにしてみる
            var blobs = _containerClient.GetBlobsAsync().AsPages();
            var resources = new List<Resource>();

            await foreach (var blobPage in blobs)
            {
                foreach (var item in blobPage.Values)
                {
                    if (item.Name.Contains(".md"))
                    {
                        var uri = item.Name;
                        var addItem = new Resource
                        {
                            Name = item.Name,
                            MimeType = "text/markdown",
                            Uri = uri
                        };
                        resources.Add(addItem);
                    }
                }
            }

            return new ListResourcesResult
            {
                Resources = resources
            };
        }

        public async ValueTask<ReadResourceResult> GetReadResourceAsync(string uri)
        {
            var blobs = _containerClient.GetBlobClient(uri);
            var content = await blobs.DownloadContentAsync();
            var stringContet = content.Value.Content.ToString();
            var res = new List<ResourceContents>();
            var item = new TextResourceContents {
                Text = stringContet,
                MimeType = "text/markdown",
                Uri = uri
            };
            res.Add(item);

            return new ReadResourceResult
            {
                Contents = res
            };

        }
    }
}

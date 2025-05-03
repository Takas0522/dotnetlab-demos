namespace McpServerSample.Models
{
    public class ToolSearchResult
    {
        public double Score { get; set; } = 0;
        public Document Document { get; set; }
    }

    public class Document
    {
        public string Content { get; set; } = string.Empty;
    }
}

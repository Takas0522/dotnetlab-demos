using System.Text;

namespace AsciiToUtfEight;

public class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("使用方法: AsciiToUtfEight <ディレクトリパス>");
            Console.WriteLine("指定されたディレクトリ内のすべてのファイルをUTF-8で再保存します。");
            return 1;
        }

        string directoryPath = args[0];

        if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine($"エラー: ディレクトリが見つかりません: {directoryPath}");
            return 1;
        }

        try
        {
            var result = ConvertFilesToUtf8(directoryPath);
            return result.ErrorCount > 0 ? 1 : 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"予期しないエラーが発生しました: {ex.Message}");
            return 1;
        }
    }

    public static ConversionResult ConvertFilesToUtf8(string directoryPath)
    {
        string[] files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);
        int processedCount = 0;
        int errorCount = 0;

        // Outputsディレクトリを作成
        string outputBaseDir = Path.Combine(directoryPath, "Outputs");
        Directory.CreateDirectory(outputBaseDir);

        Console.WriteLine($"処理を開始します。対象ファイル数: {files.Length}");
        Console.WriteLine($"出力先: {outputBaseDir}");
        Console.WriteLine();

        foreach (string filePath in files)
        {
            try
            {
                // Outputsディレクトリ内のファイルはスキップ
                if (filePath.StartsWith(outputBaseDir))
                {
                    continue;
                }

                // ファイルの内容を読み込む（エンコーディングを自動検出）
                string content;
                Encoding detectedEncoding;

                using (var reader = new StreamReader(filePath, true))
                {
                    content = reader.ReadToEnd();
                    detectedEncoding = reader.CurrentEncoding;
                }

                // 出力パスを構築（ディレクトリ構造を維持）
                string relativePath = Path.GetRelativePath(directoryPath, filePath);
                string outputPath = Path.Combine(outputBaseDir, relativePath);
                string? outputDir = Path.GetDirectoryName(outputPath);

                if (outputDir != null)
                {
                    Directory.CreateDirectory(outputDir);
                }

                // UTF-8で書き込む（BOMなし）
                File.WriteAllText(outputPath, content, new UTF8Encoding(false));

                processedCount++;
                Console.WriteLine($"✓ 変換完了: {relativePath}");
                Console.WriteLine($"  元のエンコーディング: {detectedEncoding.EncodingName}");
                Console.WriteLine($"  出力先: {outputPath}");
            }
            catch (Exception ex)
            {
                errorCount++;
                Console.WriteLine($"✗ エラー: {filePath}");
                Console.WriteLine($"  {ex.Message}");
            }

            Console.WriteLine();
        }

        Console.WriteLine("==========================================");
        Console.WriteLine($"処理完了: {processedCount}件成功, {errorCount}件失敗");
        Console.WriteLine("==========================================");

        return new ConversionResult(processedCount, errorCount);
    }
}

public record ConversionResult(int ProcessedCount, int ErrorCount);

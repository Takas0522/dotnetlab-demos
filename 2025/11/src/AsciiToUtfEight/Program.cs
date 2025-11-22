using System.Text;

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
    string[] files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);
    int processedCount = 0;
    int copiedCount = 0;
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

            // 出力パスを構築（ディレクトリ構造を維持）
            string relativePath = Path.GetRelativePath(directoryPath, filePath);
            string outputPath = Path.Combine(outputBaseDir, relativePath);
            string? outputDir = Path.GetDirectoryName(outputPath);
            
            if (outputDir != null)
            {
                Directory.CreateDirectory(outputDir);
            }

            // ファイルのエンコーディングを判定
            byte[] fileBytes = File.ReadAllBytes(filePath);
            bool isUtf8 = IsUtf8Encoded(fileBytes);

            if (isUtf8)
            {
                // すでにUTF-8の場合はコピーのみ
                File.Copy(filePath, outputPath, true);
                copiedCount++;
                Console.WriteLine($"✓ コピー完了: {relativePath}");
                Console.WriteLine($"  エンコーディング: UTF-8（変換不要）");
                Console.WriteLine($"  出力先: {outputPath}");
            }
            else
            {
                // ファイルの内容を読み込む（エンコーディングを自動検出）
                string content;
                Encoding detectedEncoding;

                using (var reader = new StreamReader(filePath, true))
                {
                    content = reader.ReadToEnd();
                    detectedEncoding = reader.CurrentEncoding;
                }

                // UTF-8で書き込む（BOMなし）
                var utf8NoBom = new UTF8Encoding(false);
                File.WriteAllText(outputPath, content, utf8NoBom);
                processedCount++;
                Console.WriteLine($"✓ 変換完了: {relativePath}");
                Console.WriteLine($"  元のエンコーディング: {detectedEncoding.EncodingName}");
                Console.WriteLine($"  出力先: {outputPath}");
            }
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
    Console.WriteLine($"処理完了: {processedCount}件変換, {copiedCount}件コピー, {errorCount}件失敗");
    Console.WriteLine("==========================================");

    return errorCount > 0 ? 1 : 0;
}
catch (Exception ex)
{
    Console.WriteLine($"予期しないエラーが発生しました: {ex.Message}");
    return 1;
}

static bool IsUtf8Encoded(byte[] data)
{
    // 空のファイルやバイナリファイルの場合
    if (data.Length == 0)
    {
        return true; // 空ファイルはUTF-8として扱う
    }

    // BOMのチェック
    if (data.Length >= 3 && data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF)
    {
        return true; // UTF-8 BOM付き
    }

    // UTF-8の妥当性をチェック
    int i = 0;
    while (i < data.Length)
    {
        byte b = data[i];

        // ASCII範囲（0x00-0x7F）
        if (b <= 0x7F)
        {
            i++;
            continue;
        }

        // 2バイト文字（0xC2-0xDF）
        if (b >= 0xC2 && b <= 0xDF)
        {
            if (i + 1 >= data.Length || !IsUtf8ContinuationByte(data[i + 1]))
                return false;
            i += 2;
            continue;
        }

        // 3バイト文字（0xE0-0xEF）
        if (b >= 0xE0 && b <= 0xEF)
        {
            if (i + 2 >= data.Length || 
                !IsUtf8ContinuationByte(data[i + 1]) || 
                !IsUtf8ContinuationByte(data[i + 2]))
                return false;
            i += 3;
            continue;
        }

        // 4バイト文字（0xF0-0xF4）
        if (b >= 0xF0 && b <= 0xF4)
        {
            if (i + 3 >= data.Length || 
                !IsUtf8ContinuationByte(data[i + 1]) || 
                !IsUtf8ContinuationByte(data[i + 2]) || 
                !IsUtf8ContinuationByte(data[i + 3]))
                return false;
            i += 4;
            continue;
        }

        // 無効なバイト
        return false;
    }

    return true;
}

static bool IsUtf8ContinuationByte(byte b)
{
    return (b & 0xC0) == 0x80;
}

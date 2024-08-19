namespace FindXml;

public class ParserEIRRMULog
{
    private string _fileTXT;
    private const string LEVEL_TYPE_0 = "Уровень вида учета"; // нет пробелов в начале строки
    private const string LEVEL_TRANSFER_STATUS_2 = "Уровень статуса передачи файлов"; // два пробела
    private const string LEVEL_VENDOR_4 = "Уровень поставщика";
    private const string LEVEL_FILE_6_8 = "Уровень файла";
    private const string LEVEL_OTHER = "Другое";

    public ParserEIRRMULog(string fileTXT)
    {
        _fileTXT = fileTXT;
    }

    public HashSet<string> GetFileNames()
    {
        var lines = File.ReadAllLines(_fileTXT);
        var fileNames = new HashSet<string>();
        bool needBlock = false;
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (IsItNeedStatusLine(line))
            {
                needBlock = true;
                continue;
            }

            if (needBlock && IsItStatusLevelLine(line) && !IsItNeedStatusLine(line))
            {
                needBlock = false;
                continue;
            }

            if (needBlock && IsItFileLevelLine(line))
            {
                var arr = line.Trim().Split(':');
                if (arr.Length > 0)
                {
                    var fileName = arr[0].Replace("Файл ", "");
                    if (!fileNames.Contains(fileName))
                    {
                        fileNames.Add(fileName);
                    }
                }
            }
        }
        return fileNames;
    }

    private string GetLineLevel(string line)
    {
        var whiteSpaceCount = line.TakeWhile(Char.IsWhiteSpace).Count();
        switch (whiteSpaceCount)
        {
            case 0:
                return LEVEL_TYPE_0;
            case 2:
                return LEVEL_TRANSFER_STATUS_2;
            case 4:
                return LEVEL_VENDOR_4;
            case 6:
                return LEVEL_FILE_6_8;
            case 8:
                return LEVEL_FILE_6_8;
            default: return LEVEL_OTHER;
        }

    }

    private bool IsItNeedStatusLine(string line)
    {
        var result = false;
        foreach (var section in Consts.REQUIRED_LOG_SECTIONS)
        {
            result = line.Contains(section);
            if(result)
                break;
        }
        return result;
    }

    private bool IsItStatusLevelLine(string line)
    {
        return GetLineLevel(line) == LEVEL_TRANSFER_STATUS_2;
    }

    private bool IsItFileLevelLine(string line)
    {
        return GetLineLevel(line) == LEVEL_FILE_6_8;
    }

    private bool IsItVendorLevelLine(string line)
    {
        return GetLineLevel(line) == LEVEL_VENDOR_4;
    }
}

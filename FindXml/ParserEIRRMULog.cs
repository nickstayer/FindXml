using System.Text;
using System.Text.RegularExpressions;

namespace FindXml;

public class ParserEIRRMULog(string fileTXT)
{
    private readonly string _fileTXT = fileTXT;
    private const string LEVEL_ACCOUNTING_TYPE_0 = "Уровень вида учета"; // нет пробелов в начале строки
    private const string LEVEL_TRANSFER_STATUS_2 = "Уровень статуса передачи файлов"; // два пробела
    private const string LEVEL_VENDOR_4 = "Уровень поставщика";
    private const string LEVEL_FILE_6_8 = "Уровень файла";
    private const string LEVEL_OTHER = "Другое";

    public List<Transfer> Parse()
    {
        var lines = File.ReadAllLines(_fileTXT);
        var transfers = new List<Transfer>();
        string currentAccountingType = string.Empty;
        string currentTransferStatus = string.Empty;
        string currentVendor = string.Empty;
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            if(IsItAccountingTypeLevelLine(line))
                currentAccountingType = line;
            
            if(IsItTransferStatusLevelLine(line))
                currentTransferStatus = GetTransferStatus(line);
            
            if(IsItVendorLevelLine(line))
                currentVendor = GetVendor(line);

            if (IsItFileLevelLine(line))
            {
                var arr = line.Trim().Split(": ");
                if (arr.Length > 0)
                {
                    var fileName = arr[0].Replace("Файл ", "");
                    var errorDescription = GetErrorDescription(line);
                    var accountingType = currentAccountingType;
                    var transferStatus = currentTransferStatus;
                    var vendor = currentVendor;
                    var transfer = new Transfer(accountingType, transferStatus, vendor, fileName, errorDescription);
                    transfers.Add(transfer);
                }
            }
        }
        return transfers;
    }

    private static string GetLineLevel(string line)
    {
        var whiteSpaceCount = line.TakeWhile(char.IsWhiteSpace).Count();
        return whiteSpaceCount switch
        {
            0 => LEVEL_ACCOUNTING_TYPE_0,
            2 => LEVEL_TRANSFER_STATUS_2,
            4 => LEVEL_VENDOR_4,
            6 => LEVEL_FILE_6_8,
            8 => LEVEL_FILE_6_8,
            _ => LEVEL_OTHER,
        };
    }

    private static bool IsItTransferStatusLevelLine(string line)
    {
        return GetLineLevel(line) == LEVEL_TRANSFER_STATUS_2;
    }

    private static bool IsItFileLevelLine(string line)
    {
        return GetLineLevel(line) == LEVEL_FILE_6_8;
    }

    private static bool IsItVendorLevelLine(string line)
    {
        return GetLineLevel(line) == LEVEL_VENDOR_4;
    }

    private static bool IsItAccountingTypeLevelLine(string line)
    {
        return GetLineLevel(line) == LEVEL_ACCOUNTING_TYPE_0;
    }

    public static string GetVendor(string vendorLine)
    {
        var pattern = @"\d{16}";
        var match = Regex.Match(vendorLine, pattern);
        if(match.Success)
        {
            return match.Value;
        }
        return string.Empty;
    }

    public static string GetTransferStatus(string transferStatusLine)
    {
        return transferStatusLine.Replace(":", "").Trim();
    }

    public static string GetErrorDescription(string line)
    {
        var arr = line.Split(": ");
        var errorDescription = new StringBuilder();
        for(int i = 0; i < arr.Length; i++)
        {
            if(i > 0)
            {
                errorDescription.Append(arr[i]);
                if(i != arr.Length - 1)
                    errorDescription.Append(": ");
            }

        }
        return errorDescription.ToString();
    }
}

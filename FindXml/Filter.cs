
namespace FindXml;

public class Filter()
{
    public const string STATUS_ERRORS = "Обработанные с ошибкой";
    public const string STATUS_REPEAT = "Отправленные на повторную обработку";
    public const string STATUS_CONFLICTS = "Конфликты";
    public const string STATUS_SUCCESS = "Успешно обработанные";
    public const string TYPE_NOT_VALID = "Файлы не прошедшие валидацию";
    public const string EXCLUDE_KEYWORD_STATEMENT_HAS_BEEN_UPLOADED = "уже было загружено в систему";

    public static string[] INCLUDE_TRANSFER_STATUS = [
        STATUS_ERRORS,
        STATUS_REPEAT,
        STATUS_CONFLICTS,
    ];

    public static string[] INCLUDE_ACCOUNTING_TYPE = [
        TYPE_NOT_VALID,
    ];

    public static string[] EXCLUDE_ERROR_DESCRIPTION_KEYWORDS = [
        EXCLUDE_KEYWORD_STATEMENT_HAS_BEEN_UPLOADED
    ];
    public static List<Transfer> Do(List<Transfer> transfers)
    {
        var result = transfers
            .Where(transfer =>
                (IncludeTransferStatusLine(transfer)
                || IncludeAccountingType(transfer))
                && !ContainsExcludeDescriptionErrorKeyword(transfer)
            )
            .ToList();
        return result;
    }

    public static bool IncludeAccountingType(Transfer transfer)
    {
        var result = false;
        foreach (var accountingType in INCLUDE_ACCOUNTING_TYPE)
        {
            result = transfer.AccountingType.Contains(accountingType);
            if (result)
                break;
        }
        return result;
    }

    public static bool IncludeTransferStatusLine(Transfer transfer)
    {
        var result = false;
        foreach (var transferStatus in INCLUDE_TRANSFER_STATUS)
        {
            result = transfer.TransferStatus.Contains(transferStatus);
            if (result)
                break;
        }
        return result;
    }

    public static bool ContainsExcludeDescriptionErrorKeyword(Transfer transfer)
    {
        var result = false;
        foreach (var keyword in EXCLUDE_ERROR_DESCRIPTION_KEYWORDS)
        {
            result = transfer.ErrorDescription.Contains(keyword);
            if (result)
                break;
        }
        return result;
    }

    public static string GetTargetFile(Transfer transfer, string sourceFile, string resultFolder)
    {
        var fileName = Path.GetFileName(sourceFile);
        var newFile = Path.Combine(resultFolder, transfer.AccountingType, transfer.TransferStatus, fileName);
        return newFile;
    }
}

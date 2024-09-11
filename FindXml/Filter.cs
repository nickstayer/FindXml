
namespace FindXml;

public class Filter()
{
    public const string STATUS_ERRORS = "Обработанные с ошибкой";
    public const string STATUS_REPEAT = "Отправленные на повторную обработку";
    public const string STATUS_CONFLICTS = "Конфликты";
    public const string STATUS_SUCCESS = "Успешно обработанные";
    public const string TYPE_NOT_VALID = "Файлы не прошедшие валидацию";
    public const string EXCLUDE_KEYWORD_STATEMENT_HAS_BEEN_UPLOADED = "уже было загружено в систему";

    public static (string, string) ERROR_KEYWORD_NO_GUID = ("В заявлении не был передан GUID адреса", "Нет GUID адреса");
    public static (string, string) ERROR_INCORRECT_PERIOD = ("Срок пребывания должен быть в диапазоне", "Некорректный срок пребывания");
    public static (string, string) ERROR_CANT_CREATE_CASE = ("Невозможно создать дело", "Невозможно создать дело");//Отсутствует адрес принимающей стороны

    public static (string, string) ERROR_NO_ADDRESS = ("ошибка ФЛК: отсутствует поле \"Адрес принимающей стороны\"", "Отсутствует адрес принимающей стороны");
    public static (string, string) NO_EXPECTED_SYMBOLS = ("Поле не содержит цифр и символов", "Поле не содержит цифр и символов");

    public static (string, string)[] ERROR_KEYWORDS = [
        ERROR_KEYWORD_NO_GUID,
        ERROR_INCORRECT_PERIOD,
        ERROR_CANT_CREATE_CASE,
        ERROR_NO_ADDRESS,
        NO_EXPECTED_SYMBOLS,
    ];


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
        var errorFolderName = GetErrorFolderName(transfer);
        var fileName = Path.GetFileName(sourceFile);
        var newFile = Path.Combine(resultFolder, transfer.AccountingType, transfer.TransferStatus, errorFolderName, fileName);
        return newFile;
    }

    private static string GetErrorFolderName(Transfer transfer)
    {
        foreach (var keyword in ERROR_KEYWORDS)
        {
            if (transfer.ErrorDescription.Contains(keyword.Item1))
                return keyword.Item2;
        }
        return string.Empty;
    }
}

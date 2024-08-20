namespace FindXml;

public class Consts
{
    public const string LOGS_DIR_NAME = "Logs";
    public const string FILE_LOG_NAME = "log.log";
    public const string LOG_DATE_FORMAT_FOR_FILE_NAME = "ddMMyyyy";

    public static string[] INCLUDE_LOG_SECTIONS = [
        "Обработанные с ошибкой:",
        "Отправленные на повторную обработку:",
        "Не прошедшие проверку сертификата пользователя:",
        "Не прошедшие проверку подписи:",
        "Не прошедшие проверку XSD схемы:",
        "Конфликты:"
    ];

    public static string[] EXCLUDE_KEYWORDS = [
        "не найдено в",
        "уже было загружено в систему"
    ];
}
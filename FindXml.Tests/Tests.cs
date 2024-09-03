namespace FindXml.Tests;

public class Tests
{
    [Test]
    [TestCase(@"..\..\..\testdata\report_979355.txt", (65) + (577 - 5) + 136 + 2 + 2 + 4 + 1)]
    [TestCase(@"..\..\..\testdata\report_984518.txt", (123) + (279 - 14) + 87 + 8)]
    [TestCase(@"..\..\..\testdata\report_986256.txt", (59) + (72) + 36 + 0)]
    public void FilterTest(string logFile, int expected)
    {
        var parser = new ParserEIRRMULog(logFile);
        var transfers = parser.Parse();
        var filtredTransfers = Filter.Do(transfers);
        Assert.That(filtredTransfers, Has.Count.EqualTo(expected));
    }

    [Test]
    [TestCase(
        "Файлы переданные в Регистрационный учет",
        "Конфликты",
        "Данные физ. лица для ГАЛАНОВ ДАНИЛА ОЛЕГОВИЧ введены некорректно.",
        @"C:\test\result\Файлы переданные в Регистрационный учет\Конфликты\Form5_7400000008002041_77566_ee640a9b-8fd4-4d21-b13f-209bf6f837c7.xml"
    )]
    public void GetTargetFileTest(
        string accountingType, 
        string transferStatus, 
        string errorDescription, 
        string expected
    )
    {
        var resultFolder = @"C:\test\result\";
        var sourceFile = @"C:\test\data\Form5_7400000008002041_77566_ee640a9b-8fd4-4d21-b13f-209bf6f837c7.xml";
        var transfer = new Transfer(
            accountingType: accountingType, 
            transferStatus: transferStatus, 
            vendor: "7400000008002041", 
            fileName: Path.GetFileName(sourceFile), 
            errorDescription: errorDescription
        );
        var actual = Filter.GetTargetFile(transfer, sourceFile, resultFolder);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(@"..\..\..\testdata\report_979355.txt", Filter.STATUS_ERRORS, 577 - 5)]
    [TestCase(@"..\..\..\testdata\report_979355.txt", Filter.STATUS_CONFLICTS, 65)]
    public void AppartFilterTest(string logFile, string transferStatus, int expected)
    {
        var parser = new ParserEIRRMULog(logFile);
        var transfers = parser.Parse();
        var filtredTransfers = Filter.Do(transfers);
        var actual = StatusFilter(filtredTransfers, transferStatus);
        Assert.That(actual, Has.Count.EqualTo(expected));
    }

    [TestCase(@"..\..\..\testdata\report_986256.txt", Filter.STATUS_ERRORS, 72)]
    [TestCase(@"..\..\..\testdata\report_986256.txt", Filter.STATUS_CONFLICTS, 59)]
    [TestCase(@"..\..\..\testdata\report_986256.txt", Filter.STATUS_SUCCESS, 749)]
    [TestCase(@"..\..\..\testdata\report_979355.txt", Filter.STATUS_ERRORS, 577)]
    [TestCase(@"..\..\..\testdata\report_979355.txt", Filter.STATUS_CONFLICTS, 65)]
    [TestCase(@"..\..\..\testdata\report_979355.txt", Filter.STATUS_SUCCESS, 976)]
    [TestCase(@"..\..\..\testdata\report_984518.txt", Filter.STATUS_ERRORS, 279)]
    [TestCase(@"..\..\..\testdata\report_984518.txt", Filter.STATUS_CONFLICTS, 123)]
    [TestCase(@"..\..\..\testdata\report_984518.txt", Filter.STATUS_SUCCESS, 1452)]
    public void ParseLogTest(string logFile, string transferStatus, int expected)
    {
        var parser = new ParserEIRRMULog(logFile);
        var transfers = parser.Parse();
        var actual = StatusFilter(transfers, transferStatus);
        Assert.That(actual, Has.Count.EqualTo(expected));
    }

    [TestCase(
        @"Файл Form5_7400000008002041_77478_6b6cf4a6-9a1c-4236-b50b-4b4c21d27a2e.xml: Номер заявления 77478; Загрузка завершилась ошибкой: Заявление о постановке на РУ по МП в гостинице №[77478] уже было загружено в систему. Идентификатор созданного дела 3428823691142631428. Повторной загрузки не требуется", 
        @"Номер заявления 77478; Загрузка завершилась ошибкой: Заявление о постановке на РУ по МП в гостинице №[77478] уже было загружено в систему. Идентификатор созданного дела 3428823691142631428. Повторной загрузки не требуется"
    )]
    [TestCase(
        @"Файл MigCs_7400000008006162_1372_a6f58c40-3559-4b3e-b6ed-b25ddcabc91a.xml: Номер заявления 02/740249/24/001372; Отказ подсистемы: [500 Internal Server Error] during [GET] to [http://esfl.gismu.it.mvd.ru:8080/gw/esfl/v1/core/v3/persons/related/synonyms/5419578307014635520] [CommonCoreEsflFeignClient#getPersonRelatedSynonyms(Long)]: [{""timestamp"":""2024-08-31T14:06:24.019+00:00"",""path"":""/esfl/v1/core/v3/persons/related/synonyms/5419578307014635520"",""status"":500,""error"":""Internal Server Error"",""message"":"",""requestId"":""f5bac83d-261489077""}]", 
        @"Номер заявления 02/740249/24/001372; Отказ подсистемы: [500 Internal Server Error] during [GET] to [http://esfl.gismu.it.mvd.ru:8080/gw/esfl/v1/core/v3/persons/related/synonyms/5419578307014635520] [CommonCoreEsflFeignClient#getPersonRelatedSynonyms(Long)]: [{""timestamp"":""2024-08-31T14:06:24.019+00:00"",""path"":""/esfl/v1/core/v3/persons/related/synonyms/5419578307014635520"",""status"":500,""error"":""Internal Server Error"",""message"":"",""requestId"":""f5bac83d-261489077""}]"
    )]
    public void GetErrorDescriptionTest(string fileLevelLine, string expected)
    {
        var actual = ParserEIRRMULog.GetErrorDescription(fileLevelLine);
        Assert.That(actual, Is.EqualTo(expected));
    }

    private static List<Transfer> StatusFilter(List<Transfer> transfers, string transferStatus)
    {
        return transfers
            .Where(transfer => 
                transfer.TransferStatus == transferStatus
            )
            .ToList();
    }
}
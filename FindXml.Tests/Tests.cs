namespace FindXml.Tests;

public class Tests
{
    [Test]
    [TestCase("..\\..\\..\\testdata\\report_979355.txt", (65) + (577 - 5) + 136 + 2 + 2 + 4 + 1)]
    [TestCase("..\\..\\..\\testdata\\report_984518.txt", (123) + (279 - 14) + 87 + 8)]
    [TestCase("..\\..\\..\\testdata\\report_986256.txt", (59) + (72) + 36 + 0)]
    public void FilterTest(string logFile, int expected)
    {
        var parser = new ParserEIRRMULog(logFile);
        var transfers = parser.Parse();
        var filtredTransfers = Filter.Do(transfers);
        Assert.That(filtredTransfers, Has.Count.EqualTo(expected));
    }

    [Test]
    [TestCase("..\\..\\..\\testdata\\report_979355.txt", Filter.STATUS_ERRORS, 577 - 5)]
    [TestCase("..\\..\\..\\testdata\\report_979355.txt", Filter.STATUS_CONFLICTS, 65)]
    public void AppartFilterTest(string logFile, string transferStatus, int expected)
    {
        var parser = new ParserEIRRMULog(logFile);
        var transfers = parser.Parse();
        var filtredTransfers = Filter.Do(transfers);
        var actual = StatusFilter(filtredTransfers, transferStatus);
        Assert.That(actual, Has.Count.EqualTo(expected));
    }

    [TestCase("..\\..\\..\\testdata\\report_986256.txt", Filter.STATUS_ERRORS, 72)]
    [TestCase("..\\..\\..\\testdata\\report_986256.txt", Filter.STATUS_CONFLICTS, 59)]
    [TestCase("..\\..\\..\\testdata\\report_986256.txt", Filter.STATUS_SUCCESS, 749)]
    [TestCase("..\\..\\..\\testdata\\report_979355.txt", Filter.STATUS_ERRORS, 577)]
    [TestCase("..\\..\\..\\testdata\\report_979355.txt", Filter.STATUS_CONFLICTS, 65)]
    [TestCase("..\\..\\..\\testdata\\report_979355.txt", Filter.STATUS_SUCCESS, 976)]
    [TestCase("..\\..\\..\\testdata\\report_984518.txt", Filter.STATUS_ERRORS, 279)]
    [TestCase("..\\..\\..\\testdata\\report_984518.txt", Filter.STATUS_CONFLICTS, 123)]
    [TestCase("..\\..\\..\\testdata\\report_984518.txt", Filter.STATUS_SUCCESS, 1452)]
    public void ParseLogTest(string logFile, string transferStatus, int expected)
    {
        var parser = new ParserEIRRMULog(logFile);
        var transfers = parser.Parse();
        var actual = StatusFilter(transfers, transferStatus);
        Assert.That(actual, Has.Count.EqualTo(expected));
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
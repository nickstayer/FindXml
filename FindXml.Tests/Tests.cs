using FindXml;

namespace FindXml.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }


    [Test]
    [TestCase("..\\..\\..\\testdata\\report_979355.txt", (577 - 5) + 2 + 2 + 4 + 1 + 136 + (65 - 10))]
    [TestCase("..\\..\\..\\testdata\\report_984518.txt", (123 - 36) + (279 - 14) + 87 + 8)]
    [TestCase("..\\..\\..\\testdata\\report_986256.txt", (59 - 6) + (72 - 0) + 36 + 0)]
    public void ParseLogTest(string logFile, int expected)
    {
        var parser = new ParserEIRRMULog(logFile);
        var fileNames = parser.Parse();
        Assert.That(fileNames.Count, Is.EqualTo(expected));
    }
}
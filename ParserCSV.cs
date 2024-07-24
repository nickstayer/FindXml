using System.Text;

namespace FindXml;

public class ParserCSV
{
    private string _fileCSV;
    public ParserCSV(string fileCSV) 
    {
        _fileCSV = fileCSV; 
    }

    public List<Record> Parse()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var encoding = Encoding.GetEncoding("windows-1251");
        var lines = File.ReadAllLines(_fileCSV, encoding);
        var records = new List<Record>();
        for(int i=0;i<lines.Length;i++)
        {
            if (i == 0) continue;
            var line = lines[i];
            var arr = line.Split(';');
            var date = ParseDate(arr[5]);
            var rec = new Record 
            { 
                FullName = arr[2], 
                Bdate = arr[3],
                PriemDate = date
            };
            records.Add(rec);
        }
        return records;
    }


    public DateTime ParseDate(string date)
    {
        var arr = date.Split(".");
        if (arr.Length == 3)
        {
            return new DateTime(int.Parse(arr[2]), int.Parse(arr[1]), int.Parse(arr[0]));
        }
        else return default;
    }
}

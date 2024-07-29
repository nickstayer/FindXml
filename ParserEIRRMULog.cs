namespace FindXml;

public class ParserEIRRMULog
{
    private string _fileTXT;
    public ParserEIRRMULog(string fileTXT)
    {
        _fileTXT = fileTXT;
    }

    public HashSet<string> GetFileNames()
    {
        var lines = File.ReadAllLines(_fileTXT);
        var fileNames = new HashSet<string>();
        bool inSideBlock = false;
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (line.StartsWith("Обработанные с ошибкой:") 
                || line.StartsWith("Отправленные на повторную обработку:"))
            {
                inSideBlock = true;
                continue;
            }
                

            if (inSideBlock && line.StartsWith("Поставщик"))
                continue;

            if(inSideBlock 
                && !line.StartsWith("Файл") 
                && !line.StartsWith("Отправленные на повторную обработку:")
                && !line.StartsWith("Обработанные с ошибкой:"))
            {
                inSideBlock = false;
                continue;
            }
                

            if (inSideBlock && line.StartsWith("Файл"))
            {
                var arr = line.Split(' ');
                var fileName = arr[1].Replace(":", "");
                if (arr.Length > 0 && !fileNames.Contains(fileName))
                {
                    fileNames.Add(fileName);
                }
            }
        }
        return fileNames;
    }
}

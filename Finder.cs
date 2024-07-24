namespace FindXml;

public class Finder
{
    public static string GetFileByRecord(Record record, string xmlFolder)
    {
        string result = string.Empty;

        foreach (var file in Directory.GetFiles(xmlFolder))
        {
            try
            {
                if (Path.GetExtension(file) != ".xml")
                    continue;
                
                var fileCreationTime = File.GetCreationTime(file);
                var fileChangeTime = File.GetLastWriteTime(file);
                if (fileChangeTime < record.PriemDate)
                    continue;
                var fileRecord = ParserXML.GetRecord(file);
                if (fileRecord.FullName.Equals(record.FullName, StringComparison.OrdinalIgnoreCase)
                    && fileRecord.Bdate.Equals(record.Bdate))
                {
                    return file;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка: {ex}. Файл: {file}");
            }
        }

        foreach (var dir in Directory.GetDirectories(xmlFolder))
        {
            result = GetFileByRecord(record, dir);
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }
        }
        return result;
    }

    public static string GetFileByName(string fileName, string xmlFolder)
    {
        string result = string.Empty;

        foreach (var file in Directory.GetFiles(xmlFolder))
        {
            try
            {
                if (Path.GetExtension(file) != ".xml")
                    continue;

                if (fileName.ToLower() == Path.GetFileName(file).ToLower())
                {
                    return file;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка: {ex}. Файл: {file}");
            }
        }

        foreach (var dir in Directory.GetDirectories(xmlFolder))
        {
            result = GetFileByName(fileName, dir);
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }
        }
        return result;
    }
}

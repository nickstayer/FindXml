namespace FindXml;

public class Utils
{
    public static string GetPath(string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            path = Path.Combine(path.Trim().Replace("\"", ""));
            if (Directory.Exists(path))
            {
                return path;
            }
        }
        return string.Empty;
    }

    public static string GetFile(string file)
    {
        if (!string.IsNullOrEmpty(file))
        {
            file = file.Trim().Replace("\"", "");
            if (File.Exists(file))
            {
                return file;
            }
        }
        return string.Empty;
    }
}

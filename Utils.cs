namespace FindXml;

public class Utils
{
    public static string NormalizePath(string path)
    {
        return path.Trim().Replace("\"", "");
    }
}

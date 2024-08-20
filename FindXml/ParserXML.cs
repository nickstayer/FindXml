using System.Text;
using System.Xml;

namespace FindXml;

public class ParserXML
{
    public static Record GetRecord(string xmlFile)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(xmlFile);
        var lastName = xmlDoc.GetElementsByTagName("lastName");
        var firstName = xmlDoc.GetElementsByTagName("firstName");
        var middleName = xmlDoc.GetElementsByTagName("middleName");
        var birthDate = xmlDoc.GetElementsByTagName("birthDate");
        
        var fullNameBuilder = new StringBuilder();
        var bDate = string.Empty;

        if (lastName?.Count > 0)
        {
            fullNameBuilder.Append(lastName[0]?.InnerText).Append(" ");
        }

        if (firstName?.Count > 0)
        {
            fullNameBuilder.Append(firstName[0]?.InnerText).Append(" ");
        }

        if (middleName?.Count > 0)
        {
            fullNameBuilder.Append(middleName[0]?.InnerText);
        }

        var fullName = fullNameBuilder.ToString().Trim().Replace("  "," ");

        if (birthDate?.Count > 0)
        {
            bDate = birthDate[0]?.InnerText.Trim();
        }
        var record = new Record(FullName: fullName, Bdate: bDate!);
        return record;
    }
}

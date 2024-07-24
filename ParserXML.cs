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
        var record = new Record();

        var fullName = new StringBuilder();

        if (lastName?.Count > 0)
        {
            fullName.Append(lastName[0].InnerText).Append(" ");
        }

        if (firstName?.Count > 0)
        {
            fullName.Append(firstName[0].InnerText).Append(" ");
        }

        if (middleName?.Count > 0)
        {
            fullName.Append(middleName[0].InnerText);
        }

        record.FullName = fullName.ToString().Trim().Replace("  "," ");

        if (birthDate?.Count > 0)
        {
            record.Bdate = birthDate[0].InnerText.Trim();
        }

        return record;
    }
}

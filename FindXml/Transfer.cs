namespace FindXml;

public class Transfer(string accountingType, string transferStatus, string vendor, string fileName, string errorDescription)
{
    public string AccountingType = accountingType;
    public string TransferStatus = transferStatus;
    public string Vendor = vendor;
    public string FileName = fileName;
    public string ErrorDescription = errorDescription;
}
namespace FindXml
{
    public class Record(string FullName, string Bdate)
    {
        public string FullName { get; set; } = FullName;
        public string Bdate { get; set; } = Bdate;
        public DateTime PriemDate { get; set; }// 5
    }
}

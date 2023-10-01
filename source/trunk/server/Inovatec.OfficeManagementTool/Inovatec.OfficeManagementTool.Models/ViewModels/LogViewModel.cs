namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class LogViewModel
    {
        public long Id { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public DateTime TimeStamp { get; set; }
        public string User { get; set; }
    }
}

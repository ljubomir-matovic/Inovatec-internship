namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class DataPage<T> where T : class
    {
        public long TotalRecords { get; set; }
        public bool More { get; set; }
        public List<T> Data { get; set; }
    }
}
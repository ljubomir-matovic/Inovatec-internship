namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class CategoryFilterRequest
    {
        private string _name = string.Empty;
        public List<int>? Types { get; set; } = new List<int>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? SortField { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 1;

        public string? Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value == null)
                {
                    _name = string.Empty;
                }
                else
                {
                    _name = value.Trim().ToLower();
                }
            }
        }
    }
}
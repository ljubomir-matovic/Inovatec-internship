namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class ItemFilterRequest
    {
        private string _name = string.Empty;

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

        public int CategoryType { get; set; }

        public List<long>? Categories { get; set; } = new List<long>();

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
        public string? SortField { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 1;
    }
}
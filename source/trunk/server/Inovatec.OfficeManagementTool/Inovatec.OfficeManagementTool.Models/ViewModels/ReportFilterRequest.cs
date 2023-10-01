namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class ReportFilterRequest
    {
        private string _description = string.Empty;
        
        public string? Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (value == null)
                {
                    _description = string.Empty;
                }
                else
                {
                    _description = value.Trim().ToLower();
                }
            }
        }

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
                    _name= string.Empty;
                }
                else
                {
                    _name= value.Trim().ToLower();
                }
            }
        }

        public List<long>? Categories { get; set; } = new List<long>();

        public List<long>? Users { get; set; } = new List<long>();

        public List<byte>? States { get; set; } = new List<byte>();

        public List<byte>? Offices { get; set; } = new List<byte>();

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string? SortField { get; set; } = string.Empty;

        public int SortOrder { get; set; } = 1;
    }
}
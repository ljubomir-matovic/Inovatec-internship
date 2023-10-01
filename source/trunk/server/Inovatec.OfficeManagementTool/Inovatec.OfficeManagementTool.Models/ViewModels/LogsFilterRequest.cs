namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public  class LogsFilterRequest
    {
        private string _message = string.Empty;

        public string? Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (value == null)
                {
                    _message = string.Empty;
                }
                else
                {
                    _message = value.Trim().ToLower();
                }
            }
        }

        private string _exception = string.Empty;

        public string? Exception
        {
            get
            {
                return _exception;
            }
            set
            {
                if (value == null)
                {
                    _exception = string.Empty;
                }
                else
                {
                    _exception = value.Trim().ToLower();
                }
            }
        }

        private string _user = string.Empty;

        public string? User
        {
            get
            {
                return _user;
            }
            set
            {
                if (value == null)
                {
                    _user = string.Empty;
                }
                else
                {
                    _user = value.Trim().ToLower();
                }
            }
        }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string? SortField { get; set; } = string.Empty;

        public int SortOrder { get; set; } = 1;
    }
}

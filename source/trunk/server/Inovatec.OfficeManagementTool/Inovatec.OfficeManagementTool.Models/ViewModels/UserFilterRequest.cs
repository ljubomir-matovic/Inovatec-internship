namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class UserFilterRequest
    {
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _email = string.Empty;
        private string _office = string.Empty;

        public int PageNumber { get; set; }
        public int? PageSize { get; set; } = 10;
        public string? SortField { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 1;

        public string? FirstName { 
            get 
            { 
                return _firstName;
            }
            set
            { 
                if (value == null) 
                { 
                    _firstName = string.Empty;
                }
                else
                {
                    _firstName = value.Trim().ToLower();
                }
            } 
        }
        public string? LastName { 
            get
            {
                return _lastName;
            }
            set
            {
                if (value == null)
                {
                    _lastName = string.Empty;
                }
                else
                {
                    _lastName = value.Trim().ToLower();
                }
            }
        }
        public string? Email
        {
            get
            {
                return _email;
            }
            set
            {
                if (value == null)
                {
                    _email = string.Empty;
                }
                else
                {
                    _email = value.Trim().ToLower();
                }
            }
        }

        public string? Office
        {
            get
            {
                return _office;
            }
            set
            {
                if (value == null)
                {
                    _office = string.Empty;
                }
                else
                {
                    _office = value.Trim().ToLower();
                }
            }
        }

        public List<int>? Roles { get; set; } = new List<int>();
        public List<long>? Offices { get; set; } = new List<long>();
    }
}
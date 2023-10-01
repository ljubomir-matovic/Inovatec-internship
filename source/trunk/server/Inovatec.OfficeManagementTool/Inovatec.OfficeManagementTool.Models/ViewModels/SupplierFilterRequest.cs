namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class SupplierFilterRequest
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

        private string _phoneNumber = string.Empty;

        public string? PhoneNumber
        {
            get
            {
                return _phoneNumber;
            }
            set
            {
                if (value == null)
                {
                    _phoneNumber = string.Empty;
                }
                else
                {
                    _phoneNumber = value.Trim().ToLower();
                }
            }
        }

        private string _country = string.Empty;

        public string? Country
        {
            get
            {
                return _country;
            }
            set
            {
                if (value == null)
                {
                    _country = string.Empty;
                }
                else
                {
                    _country = value.Trim().ToLower();
                }
            }
        }

        private string _city = string.Empty;

        public string? City
        {
            get
            {
                return _city;
            }
            set
            {
                if (value == null)
                {
                    _city = string.Empty;
                }
                else
                {
                    _city = value.Trim().ToLower();
                }
            }
        }

        private string _address = string.Empty;

        public string? Address
        {
            get
            {
                return _address;
            }
            set
            {
                if (value == null)
                {
                    _address = string.Empty;
                }
                else
                {
                    _address = value.Trim().ToLower();
                }
            }
        }


        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? SortField { get; set; } = string.Empty;
        public int SortOrder { get; set; } = -1;
    }
}

using Microsoft.AspNetCore.Http;

namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class CSVUploadRequest
    {
        public List<IFormFile> Files { get; set; }
    }
}
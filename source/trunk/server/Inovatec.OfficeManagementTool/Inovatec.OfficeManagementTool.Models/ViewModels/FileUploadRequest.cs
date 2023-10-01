using Microsoft.AspNetCore.Http;

namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class FileUploadRequest
    {
        public List<IFormFile> Files { get; set; }
        public long OrderId { get; set; }
    }
}
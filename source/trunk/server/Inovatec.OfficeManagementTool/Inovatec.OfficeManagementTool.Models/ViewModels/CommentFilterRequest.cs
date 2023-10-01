namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class CommentFilterRequest
    {
        public List<long>? Users { get; set; } = new List<long>();
        public List<long>? Orders { get; set; } = new List<long>();
        public List<long>? Reports { get; set; } = new List<long>();
        public bool GetOrderComments { get; set; } = false;
        public bool GetReportComments { get; set; } = false;
        public int CommentsSkipCount { get; set; }
        public int CommentsBatchSize { get; set; }
        public int SortOrder { get; set; } = 1;
    }
}
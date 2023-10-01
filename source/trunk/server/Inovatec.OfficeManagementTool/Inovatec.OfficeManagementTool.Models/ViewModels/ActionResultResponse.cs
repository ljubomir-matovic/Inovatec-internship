namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class ActionResultResponse<T>
    {
        public bool ActionSuccess { get; set; }
        public T ActionData { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class ActionResultResponse : ActionResultResponse<object>
    { }
}
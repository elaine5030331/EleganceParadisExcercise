namespace ApplicationCore.Models
{
    public class OperationResult<T> where T : BaseOperationResult
    {
        public OperationResult() { }

        public OperationResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
        public OperationResult(T resultDto)
        {
            IsSuccess = true;
            ResultDTO = resultDto;
        }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public T ResultDTO { get; set; }
    }

    /// <summary>
    /// 提供給需要 OperationResult 的 DTO 繼承使用
    /// </summary>
    public class BaseOperationResult()
    {

    }
}

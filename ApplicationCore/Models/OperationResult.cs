namespace ApplicationCore.Models
{
    /// <summary>
    /// 提供service回傳結果及錯誤訊息
    /// </summary>
    public class OperationResult
    {
        public OperationResult()
        {
            IsSuccess = true;
        }
        public OperationResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }
    }

    /// <summary>
    /// 提供service回傳結果及錯誤訊息，回傳內容會包含DTO
    /// </summary>
    /// <typeparam name="T">`BaseOperationResult`</typeparam>
    public class OperationResult<T> : OperationResult where T : BaseOperationResult
    {
        public OperationResult() : base() { }
        public OperationResult(string errorMessage) : base(errorMessage) { }
        public OperationResult(T resultDto)
        {
            IsSuccess = true;
            ResultDTO = resultDto;
        }

        public T ResultDTO { get; set; }
    }

    /// <summary>
    /// 泛型約束：提供給需要 OperationResult 的 DTO 繼承使用
    /// </summary>
    public class BaseOperationResult()
    {

    }
}

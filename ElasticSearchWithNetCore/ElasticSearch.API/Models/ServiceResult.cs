namespace ElasticSearch.API.Models;

public class ServiceResult
{
    public string Message { get; set; }

    public ProcessState ProcessState { get; set; } = ProcessState.Success;

    public void InitError(Exception exception)
    {
        this.ProcessState = ProcessState.Error;

        Message = string.Join(Environment.NewLine, exception.Message);
    }
}

public class ServiceResult<T> : ServiceResult
{
    public T ProcessResult { get; set; }
}

namespace ConferencePlanner.Service;

public class ExceptionMessageErrorFilter : IErrorFilter
{
    /// <inheritdoc />
    public IError OnError(IError error)
    {
        string exceptionMessage = error.Message;

        if (!string.IsNullOrWhiteSpace(error.Exception?.Message))
        {
            exceptionMessage += $" - {error.Exception.Message}";
        }

        return error.WithMessage(exceptionMessage);
    }
}
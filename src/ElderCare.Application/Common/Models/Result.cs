namespace ElderCare.Application.Common.Models;

public class Result
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();

    public static Result Success(string message = "Operation successful")
    {
        return new Result { IsSuccess = true, Message = message };
    }

    public static Result Failure(string message, List<string>? errors = null)
    {
        return new Result 
        { 
            IsSuccess = false, 
            Message = message, 
            Errors = errors ?? new List<string>() 
        };
    }

    public static Result Failure(string message, string error)
    {
        return new Result 
        { 
            IsSuccess = false, 
            Message = message, 
            Errors = new List<string> { error } 
        };
    }
}

public class Result<T> : Result
{
    public T? Data { get; set; }

    public static Result<T> Success(T data, string message = "Operation successful")
    {
        return new Result<T> 
        { 
            IsSuccess = true, 
            Message = message, 
            Data = data 
        };
    }

    public new static Result<T> Failure(string message, List<string>? errors = null)
    {
        return new Result<T> 
        { 
            IsSuccess = false, 
            Message = message, 
            Errors = errors ?? new List<string>() 
        };
    }

    public new static Result<T> Failure(string message, string error)
    {
        return new Result<T> 
        { 
            IsSuccess = false, 
            Message = message, 
            Errors = new List<string> { error } 
        };
    }

    public new static Result<T> Failure(string message)
    {
        return new Result<T> 
        { 
            IsSuccess = false, 
            Message = message, 
            Errors = new List<string>() 
        };
    }
}

public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

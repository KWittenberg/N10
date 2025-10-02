namespace Spot.DTOs;

//public class Result
//{
//    public bool Success { get; }
//    public string Message { get; }

//    protected Result(bool success, string message)
//    {
//        Success = success;
//        Message = message;
//    }

//    public static Result Success(string message = "Success") => new(true, message);
//    public static Result Failure(string message) => new(false, message);
//}

//public class Result<T> : Result
//{
//    public T? Data { get; }

//    private Result(bool success, string message, T? data) : base(success, message)
//    {
//        Data = data;
//    }

//    public static Result<T> Success(T data, string message = "Success") => new(true, message, data);
//    public static Result<T> Failure(string message) => new(false, message, default);
//}
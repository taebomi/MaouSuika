namespace TBM.Core
{
    public readonly struct Result
    {
        public bool IsSuccess { get; }
        public int ErrorCode { get; }
        public string Message { get; }

        private Result(bool isSuccess, int errorCode, string message)
        {
            IsSuccess = isSuccess;
            ErrorCode = errorCode;
            Message = message;
        }

        public static Result Success() => new(true, 0, null);
        public static Result Failure(int errorCode, string message = null) => new(false, errorCode, message);
    }

    public readonly struct Result<T>
    {
        public bool IsSuccess { get; }
        public int ErrorCode { get; }
        public string Message { get; }
        public T Value { get; }

        private Result(bool isSuccess, T value, int errorCode, string message)
        {
            IsSuccess = isSuccess;
            ErrorCode = errorCode;
            Message = message;
            Value = value;
        }

        public static Result<T> Success(T value) => new(true, value, 0, null);

        public static Result<T> Failure(int errorCode, string message = null) =>
            new(false, default, errorCode, message);
    }
}
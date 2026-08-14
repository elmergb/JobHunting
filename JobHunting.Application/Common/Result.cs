using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Application.Common
{
    public enum ErrorType
    {
        Invalid = 0,
        NotFound = 1,
        Conflict = 2,
        Unauthorized = 3,
        Forbidden = 4,
        InternalServerError = 5
    }

    public class Error
    {
        public ErrorType Type { get; }
        public string Message { get; }
        public string? Code { get; }

        private Error(ErrorType type, string message, string? code = null)
        {
            Type = type;
            Message = message;
            Code = code;
        }
        public static Error Invalid(string message, string? code = null)
            => new(ErrorType.Invalid, message, code);

        public static Error NotFound(string message, string? code = null)
            => new(ErrorType.NotFound, message, code);

        public static Error Conflict(string message, string? code = null)
            => new(ErrorType.Conflict, message, code);

        public static Error Unauthorized(string message, string? code = null)
            => new(ErrorType.Unauthorized, message, code);

        public static Error Forbidden(string message, string? code = null)
            => new(ErrorType.Forbidden, message, code);

        public static Error InternalServerError(string message, string? code = null)
            => new(ErrorType.InternalServerError, message, code);
    }

    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error? Error { get; }

        protected Result(bool isSuccess, Error? error = null)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true);
        public static Result Failure(Error error) => new(false, error);
    }

    public class Result<T> : Result
    {
        public T? Value { get; }

        private Result(T value) : base(true) 
            => Value = value;

        private Result(Error error) : base(false, error) 
            => Value = default;

        public static Result<T> Success(T value) => new(value);
        public static new Result<T> Failure(Error error) => new(error);
    }
}


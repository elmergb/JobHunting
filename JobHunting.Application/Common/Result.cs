using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Application.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string? Error { get; }
        public string? ErrorCode { get; }

        protected Result(bool isSuccess, string? errorCode = null, string? error = null)
        {
            IsSuccess = isSuccess;
            ErrorCode = errorCode;
            Error = error;
        }

        public static Result Success() => new(true);
        public static Result Failure(string code, string error) => new(false, code, error);
    }

    public class Result<T> : Result
    {
        public T? Value { get; }

        private Result(T value) : base(true) => Value = value;
        private Result(string code, string error) : base(false, code, error) { }

        public static Result<T> Success(T value) => new(value);
        public new static Result<T> Failure(string code, string error) => new(code, error);
    }
}

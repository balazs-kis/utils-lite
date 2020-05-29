using System;

namespace UtilsLite.Functional
{
    /// <summary>
    /// Result class with error signaling.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Indicates whether the operation was successful
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Indicates whether the operation has failed
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Description of the error (empty string in the case of no error)
        /// </summary>
        public string Error { get; }

        protected Result(bool isSuccess, string error)
        {
            if (isSuccess && error != string.Empty)
            {
                throw new InvalidOperationException();
            }

            if (!isSuccess && error == string.Empty)
            {
                throw new InvalidOperationException();
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        /// <summary>
        /// Returns a failure result with a specific message
        /// </summary>
        /// <param name="message">The error message</param>
        /// <returns>Result indicating failure</returns>
        public static Result Fail(string message) =>
            new Result(false, message);

        /// <summary>
        /// Returns a failure result with a specific message
        /// </summary>
        /// <param name="message">The error message</param>
        /// <returns>Result indicating failure</returns>
        public static Result<T> Fail<T>(string message) =>
            new Result<T>(default(T), false, message);

        /// <summary>
        /// Returns a successful result
        /// </summary>
        /// <returns>Result indicating success</returns>
        public static Result Ok() =>
            new Result(true, string.Empty);

        /// <summary>
        /// Returns a successful result with content
        /// </summary>
        /// <param name="value">The result object</param>
        /// <returns>Result indicating success</returns>
        public static Result<T> Ok<T>(T value) =>
            new Result<T>(value, true, string.Empty);
    }

    /// <inheritdoc />
    /// <summary>
    /// Wrapper class for a result with error signaling.
    /// </summary>
    public sealed class Result<T> : Result
    {
        private readonly T _value;

        public T Value =>
            IsSuccess ? _value : throw new InvalidOperationException();

        internal Result(T value, bool isSuccess, string error)
            : base(isSuccess, error)
        {
            _value = value;
        }
    }
}

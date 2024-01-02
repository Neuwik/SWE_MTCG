using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.Request_Handling
{
    public abstract class HttpResponseExcetion : Exception
    {
        public string ResponseCode { get; init; }

        public HttpResponseExcetion(string responseCode) : base()
        {
            ResponseCode = responseCode;
        }

        public HttpResponseExcetion(string responseCode, string message) : base(message)
        {
            ResponseCode = responseCode;
        }

        public override string ToString()
        {
            if (Message != null)
                return ResponseCode + ": " + Message;
            return ResponseCode;
        }
    }

    public class InternalServerErrorException : HttpResponseExcetion
    {
        public InternalServerErrorException() : base("500 Internal Server Error") { }
        public InternalServerErrorException(string message) : base("500 Internal Server Error", message) { }
    }

    public class NotImplementedException : HttpResponseExcetion
    {
        public NotImplementedException() : base("501 Not Implemented") { }
        public NotImplementedException(string message) : base("501 Not Implemented", message) { }
    }

    public class BadRequestException : HttpResponseExcetion
    {
        public BadRequestException() : base("404 Not Found") { }
        public BadRequestException(string message) : base("404 Not Found", message) { }

    }

    public class NotLoggedInException : HttpResponseExcetion
    {
        public NotLoggedInException() : base("403 Forbidden") { }
        public NotLoggedInException(string message) : base("403 Forbidden", message) { }
    }

    public class NoAdminException : HttpResponseExcetion
    {
        public NoAdminException() : base("401 Unauthorized") { }
        public NoAdminException(string message) : base("401 Unauthorized", message) { }
    }

    public class AlreadyLoggedInException : HttpResponseExcetion
    {
        public AlreadyLoggedInException() : base("409 Conflict") { }
        public AlreadyLoggedInException(string message) : base("409 Conflict", message) { }
    }

    public class WrongParametersException : HttpResponseExcetion
    {
        public WrongParametersException() : base("422 Unprocessable Content") { }
        public WrongParametersException(string message) : base("422 Unprocessable Content", message) { }
    }

    public class InputNotAllowedException : HttpResponseExcetion
    {
        public InputNotAllowedException() : base("409 Conflict") { }
        public InputNotAllowedException(string message) : base("409 Conflict", message) { }
    }

    public class NothingFoundException : HttpResponseExcetion
    {
        public NothingFoundException() : base("400 Bad Request") { }
        public NothingFoundException(string message) : base("400 Bad Request", message) { }
    }
}

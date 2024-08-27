using System.Collections.Generic;
using AHI.Infrastructure.Exception;

namespace Reporting.Function.Exception
{
    public static class ExceptionHelper
    {
        public static GenericCommonException GenerateCommonException(string message = null, string detailCode = null,
                                            object payload = null, System.Exception innerException = null)
        {
            return new GenericCommonException(message, detailCode,
                        new Dictionary<string, object> { { "payload", payload } }, innerException: innerException);
        }

        public static EntityNotFoundException GenerateEntityNotFoundException(string message = null, string detailCode = null,
                                            object payload = null, System.Exception inneException = null)
        {
            return new EntityNotFoundException(message, detailCode,
                        new Dictionary<string, object> { { "payload", payload } }, inneException);
        }

        public static EntityInvalidException GenerateEntityInvalidException(string message = null, string detailCode = null,
                                            object payload = null, System.Exception innerException = null)
        {
            return new EntityInvalidException(message, detailCode,
                        new Dictionary<string, object> { { "payload", payload } }, innerException);
        }
    }
}
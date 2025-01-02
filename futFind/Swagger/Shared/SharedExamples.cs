using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Filters;

namespace futFind.Swagger.Shared
{
    public class SharedExamples
    {

    }

    public class AuthorizationTokenMissingExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new { message = "Authorization header is missing." };
        }
    }

    public class UnauthorizedExample : IExamplesProvider<object>
    {
        public object GetExamples() { return new { status = 401 }; }
    }

    public class NotFoundExample : IExamplesProvider<object>
    {
        public object GetExamples() { return new { status = 404 }; }
    }
}
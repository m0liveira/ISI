using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using futFind.Models;
using Swashbuckle.AspNetCore.Filters;

namespace futFind.Swagger.Responses
{
    public class AuthResponse
    {
        public required string Token { get; set; }
        public required Users Data { get; set; }
    }

    public class UnauthorizedResponse { public required string Message { get; set; } }

    public class AuthorizedResponseExample : IExamplesProvider<AuthResponse>
    {
        public AuthResponse GetExamples()
        {
            return new AuthResponse
            {
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
                Data = new Users
                {
                    id = 1,
                    email = "user@example.com",
                    name = "John Doe",
                    password = "strongPassword",
                    phone = "912345678",
                }
            };
        }
    }

    public class UnauthorizedResponseExample : IExamplesProvider<UnauthorizedResponse>
    {
        public UnauthorizedResponse GetExamples() { return new UnauthorizedResponse { Message = "Invalid email or password" }; }
    }
}

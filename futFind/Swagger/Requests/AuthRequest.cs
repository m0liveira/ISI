using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Filters;

namespace futFind.Swagger.Requests
{
    public class AuthRequest
    {
        public required string Email { get; set; }

        public required string Password { get; set; }
    }

    public class AuthRequestExample : IExamplesProvider<AuthRequest>
    {
        public AuthRequest GetExamples()
        {
            return new AuthRequest
            {
                Email = "user@example.com",
                Password = "securepassword123"
            };
        }
    }
}
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Tests.Mock
{
    public class AuthenticationManagerMock : AuthenticationManager
    {
        public AuthenticationManagerMock(HttpContext context)
        {
            _HttpContext = context;
        }
        public HttpContext _HttpContext;
        public override HttpContext HttpContext => _HttpContext;

        public override Task AuthenticateAsync(AuthenticateContext context)
        {
            return Task.CompletedTask;
        }

        public override Task ChallengeAsync(string authenticationScheme, AuthenticationProperties properties, ChallengeBehavior behavior)
        {
            return Task.CompletedTask;
        }

        public AuthenticateInfo GetAuthenticateInfoAsyncResult = new AuthenticateInfo();
        public override Task<AuthenticateInfo> GetAuthenticateInfoAsync(string authenticationScheme)
        {
            return Task.FromResult(GetAuthenticateInfoAsyncResult);
        }

        public override IEnumerable<AuthenticationDescription> GetAuthenticationSchemes()
        {
            throw new NotImplementedException();
        }

        public override Task SignInAsync(string authenticationScheme, ClaimsPrincipal principal, AuthenticationProperties properties)
        {
            return Task.CompletedTask;
        }

        public override Task SignOutAsync(string authenticationScheme, AuthenticationProperties properties)
        {
            return Task.CompletedTask;
        }
    }
}

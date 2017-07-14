using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using Autofac.Integration.WebApi;
using System.Security.Principal;
using System.Diagnostics;
using System.Security.Claims;
using System.Collections.Generic;

namespace TestOwinSelfHostWithAutoFac.Attributes
{
   /// <summary>
   ///  Authorization Filter
   /// </summary>
   [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
   public sealed class AuthoriseAttribute : AuthorizationFilterAttribute, IAutofacAuthenticationFilter
   {

      public AuthoriseAttribute()
      {
      }

      public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
      {
         return Task.Run(() => OnAuthenticate(context), cancellationToken);
      }

      public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
      {
         return Task.Run(() => OnChallenge(context), cancellationToken);
      }

      /// <summary>
      /// Authenticate request using Token
      /// </summary>
      /// <param name="context">Authentication context</param>
      public void OnAuthenticate(HttpAuthenticationContext context)
      {
         if (context == null)
            throw new ArgumentNullException("Context cannot be null.");

         if (WebServerConfig.AuthorizationEnabled)
         {
            // Lets Authenticate the Request.
            string authHeader = null;
            var auth = context.Request.Headers.Authorization;
            if (auth != null && auth.Scheme.Equals("bearer", StringComparison.OrdinalIgnoreCase))
            {
               authHeader = auth.Parameter;
            }

            if (string.IsNullOrEmpty(authHeader))
            {
               context.ActionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
               return;
            }

            if (!authHeader.Equals(WebServerConfig.AuthorizationToken))
            {
               context.ActionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
               Console.WriteLine("MonitoringAuthoriseAttribute OnAuthenticate " + string.Format("Request {0} from {1} is unauthorized", context.Request.RequestUri.AbsolutePath, context.Request.RequestUri.Host));
               return;
            }

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, "superstar"));

            var identity = new ClaimsIdentity(claims, "PresharedKey");
            var principal = new ClaimsPrincipal(identity);

            context.Principal = principal;
         }
         else
         {
            // Nothing to do... until we have IdentServer 4 :-)
         }
      }

      /// <summary>
      ///  Invoked when an authentication challenge is required
      /// </summary>
      /// <param name="context">Authentication challenge context </param>
      public void OnChallenge(HttpAuthenticationChallengeContext context)
      {
         IPrincipal incomingPrincipal = context.ActionContext.RequestContext.Principal;
         if (incomingPrincipal != null)
         {
            Debug.WriteLine(String.Format("Incoming principal in custom auth filter ChallengeAsync method is authenticated: {0}", incomingPrincipal.Identity.IsAuthenticated));
         }

      }
   }
}

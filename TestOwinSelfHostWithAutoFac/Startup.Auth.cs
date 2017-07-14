using System;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace TestOwinSelfHostWithAutoFac
{
   public static class MyAuthentication
   {
      public const String ApplicationCookie = "MyProjectAuthenticationType";
   }

   public partial class StartupConfig
   {
      public void ConfigureAuth(IAppBuilder app)
      {
         // need to add UserManager into owin, because this is used in cookie invalidation
         app.UseCookieAuthentication(new CookieAuthenticationOptions
         {
            AuthenticationType = MyAuthentication.ApplicationCookie,
            LoginPath = new PathString("/Login"),
            Provider = new CookieAuthenticationProvider(),
            CookieName = "MyCookieName",
            CookieHttpOnly = true,
            ExpireTimeSpan = TimeSpan.FromHours(12), // adjust to your needs
         });
      }
   }
}

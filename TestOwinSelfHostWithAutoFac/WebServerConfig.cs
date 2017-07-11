namespace TestOwinSelfHostWithAutoFac
{
   /// <summary>
   /// Contains configurations options for Web Api
   /// </summary>
   public static class WebServerConfig
   {
      /// <summary>
      ///  Gets or sets a value indicating whether a request need to be authorized.
      /// </summary>
      public static bool AuthorizationEnabled { get; set; }

      /// <summary>
      /// Gets or sets the token used to authorize the user to perform a request.
      /// </summary>
      public static string AuthorizationToken { get; set; }
   }
}
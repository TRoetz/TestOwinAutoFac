// topshelf startup code
using Autofac;
using Autofac.Integration.WebApi;
using Owin;
using Microsoft.Owin.FileSystems;
using System;
using System.IO;
using System.Reflection;
using System.Web.Http;
using Topshelf;
using Topshelf.Options;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.Hosting;
using TestOwinSelfHostWithAutoFac;

class Program
{
   static void Main(string[] args)
   {

      HostFactory.Run(c =>
      {
         c.RunAsNetworkService();
         c.SetServiceName("OwinHostAPITester");
         c.SetDisplayName("OwinHostAPITester");

         c.Service<StatsService>(s =>
           {
             s.ConstructUsing(name => new StatsService());
             s.WhenStarted((service, control) => service.Start());
             s.WhenStopped((service, control) => service.Stop());
          });

      });
   }
}

// lifted from http://autofac.readthedocs.org/en/latest/integration/webapi.html#owin-integration
public class StartupConfig
{
   public void Configure(IAppBuilder appBuilder)
   {
      var config = new HttpConfiguration();
      config.MapHttpAttributeRoutes();                                                        // using attribute based routing because I prefer it

      var builder = new Autofac.ContainerBuilder();
      builder.RegisterApiControllers(Assembly.GetExecutingAssembly()).PropertiesAutowired();  // Create the container builder.
      builder.RegisterApiControllers(Assembly.GetExecutingAssembly());                        // Register the Web API controllers.
      builder.RegisterWebApiFilterProvider(config);                                           // optional

      builder.RegisterType<Stat>().As<IStat>().InstancePerRequest();


      var container = builder.Build();
      config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

      appBuilder.UseAutofacMiddleware(container);
      appBuilder.UseAutofacWebApi(config);                                                    // Make sure the Autofac lifetime scope is passed to Web API.
      appBuilder.UseWebApi(config);                                                           // enable web-api


      string filedir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "../../web");
      appBuilder.UseFileServer(new FileServerOptions
      {
         EnableDefaultFiles = true,
         DefaultFilesOptions =
            {
                DefaultFileNames = new[] { "Index.html" }
            },
         EnableDirectoryBrowsing = true,
         FileSystem = new PhysicalFileSystem(filedir),
      });
   }
}

// topshelf hosted service to start
public class StatsService
{
   public bool Start()
   {
      if (WebApplication == null)
      {
         WebApplication = WebApp.Start
         (
              new StartOptions
              {
                 Port = 5999
              },
              appBuilder =>
              {
                 new StartupConfig().Configure(appBuilder);
              }
         );
      }

      return true;
   }

   public bool Stop()
   {
      return true;
   }

   protected IDisposable WebApplication
   {
      get;
      set;
   }
}

// test controller
public class StatsController : ApiController
{

   public IStat MyModel;

   public StatsController(IStat stats)
   {
      MyModel = stats;
   }

   [HttpGet]
   [Route("api/emails/{id}")]
   public IHttpActionResult get(int id)
   {
      return Ok(MyModel.GetStats());
   }
}
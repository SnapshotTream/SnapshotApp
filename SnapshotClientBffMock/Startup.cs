using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using SimpleInjector.Lifestyles;
using Snapshot.Client.Bff.Core.Dao;
using Snapshot.Client.Bff.Dao;
using Snapshot.Client.Bff.Mock.Data;
using Snapshot.Client.Bff.Sdk.Dao;
using Swashbuckle.AspNetCore.Swagger;

namespace Snapshot.Client.Bff.Mock {
  /// <summary>
  /// ASPNETスタートアップ
  /// </summary>
  public class Startup {
    private Container mContainer = new Container ();

    private Logger mLogger = LogManager.GetCurrentClassLogger ();

    private IConfiguration Configuration { get; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="configuration"></param>
    public Startup (IConfiguration configuration) {
      Configuration = configuration;
    }

    /// <summary>
    /// This method gets called by the runtime. Use this method to add services to the container.
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices (IServiceCollection services) {
      services.AddMemoryCache ();
      services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_1);
      services.AddSwaggerGen (c => {
        c.SwaggerDoc ("v1", new Info {
          Version = "v1",
            Title = "Foxpict.Client.Web API",
            Description = "A simple example ASP.NET Core Web API",
            Contact = new Contact { Name = "Juan García Carmona", Email = "d.jgc.it@gmail.com", Url = "https://wisegeckos.com" },
        });
        // Set the comments path for the Swagger JSON and UI.
        var basePath = AppContext.BaseDirectory;
        var xmlPath = Path.Combine (basePath, "SnapshotClientBffMock.xml");
        c.IncludeXmlComments (xmlPath);
      });
      services.AddCors ();
      IntegrateSimpleInjector (services);
    }

    /// <summary>
    /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="env"></param>
    public void Configure (IApplicationBuilder app, IHostingEnvironment env) {
      mLogger.Info ("Starting Mock");

      InitializeContainer (app);
      StartApplication ();

      if (env.IsDevelopment ()) {
        app.UseDeveloperExceptionPage ();
      } else {
        app.UseHsts ();
      }

      // Enable middleware to serve generated Swagger as a JSON endpoint.
      app.UseSwagger ();

      // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
      app.UseSwaggerUI (c => {
        c.SwaggerEndpoint ("/swagger/v1/swagger.json", "My API V1");
      });

      app.UseCors (builder => builder
        .AllowAnyOrigin ()
        .AllowAnyMethod ()
        .AllowAnyHeader ());

      app.UseHttpsRedirection ();
      app.UseMvc ();
    }

    private void StartApplication () {
      var appConfig = ApplicationSettingsInfo.FromAppSettings (this.Configuration);

      mContainer.RegisterInstance (new DaoContext { ServerUrl = appConfig.ServiceServerUrl });

      IntegrateDao ();
      mContainer.Verify ();
    }

    private void IntegrateSimpleInjector (IServiceCollection services) {
      mContainer.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle ();
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor> ();
      //services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, QueuedHostedService> ();
      //services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue> ();

      services.AddSingleton<IControllerActivator> (new SimpleInjectorControllerActivator (mContainer));
      services.AddSingleton<IViewComponentActivator> (new SimpleInjectorViewComponentActivator (mContainer));

      services.EnableSimpleInjectorCrossWiring (mContainer);
      services.UseSimpleInjectorAspNetRequestScoping (mContainer);
    }

    private void InitializeContainer (IApplicationBuilder app) {
      mContainer.RegisterMvcControllers (app);
      mContainer.RegisterMvcViewComponents (app);
      mContainer.CrossWire<ILoggerFactory> (app);
    }

    private void IntegrateDao () {
      mContainer.Register<ICategoryDao, CategoryDao> ();
      mContainer.Register<IContentDao, ContentDao> ();
      mContainer.Register<ILabelDao, LabelDao> ();
      mContainer.Register<IEventLogDao, EventLogDao> ();
      mContainer.Register<IThumbnailDao, ThumbnailDao> ();
    }
  }
}

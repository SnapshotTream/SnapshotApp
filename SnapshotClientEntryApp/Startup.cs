using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
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
using NLog;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using SimpleInjector.Lifestyles;
using Snapshot.Client.Bff.Core.Dao;
using Snapshot.Client.Bff.Dao;
using Snapshot.Client.Bff.Mock.Data;
using Snapshot.Client.Bff.Sdk.Dao;
using Snapshot.Client.Entry.App.Data;

namespace Snapshot.Client.Entry.App {

  public class Startup {

    private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger ();

    private Container mContainer = new Container ();

    public Startup (IConfiguration configuration) {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices (IServiceCollection services) {
      services.Configure<CookiePolicyOptions> (options => {
        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.None;
      });

      services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_1);
      IntegrateSimpleInjector (services);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure (IApplicationBuilder app, IHostingEnvironment env) {
      InitializeContainer (app);
      StartApplication ();

      if (env.IsDevelopment ()) {
        app.UseDeveloperExceptionPage ();
      } else {
        app.UseExceptionHandler ("/Home/Error");
        app.UseHsts ();
      }

      app.UseHttpsRedirection ();
      app.UseStaticFiles ();
      app.UseCookiePolicy ();

      app.UseMvc (routes => {
        routes.MapRoute (
          name: "default",
          template: "{controller=Home}/{action=Index}/{id?}");
      });

      if (HybridSupport.IsElectronActive) {
        ElectronBootstrap ();
      }
    }

    public async void ElectronBootstrap () {
      Console.WriteLine ("Execute CreateWindowAsync");
      var browserWindow = await Electron.WindowManager.CreateWindowAsync (new BrowserWindowOptions {
        Width = 1400,
          Height = 900,
          WebPreferences = new WebPreferences {
            WebSecurity = false
          },
          Show = false
      });

      browserWindow.OnReadyToShow += () => browserWindow.Show ();
      browserWindow.SetTitle ("Client");
    }

    private void StartApplication () {
      LOGGER.Debug ("アプリケーション設定ファイルから設定値を読み込みます。");
      var appConfig = ApplicationSettingsInfo.FromAppSettings (this.Configuration);
      LOGGER.Debug("ServiceServerUrl={0}",appConfig.ServiceServerUrl);

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
    }
  }
}

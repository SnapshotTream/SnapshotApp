using System.IO;
using System.Text.RegularExpressions;
using Foxpict.Service.Infra;
using Foxpict.Service.Infra.Model;
using Foxpict.Service.Model;
using Hyperion.Pf.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NLog;

namespace Foxpict.Service.Gateway {
  /// <summary>
  /// サムネイル情報のデータベース
  /// </summary>
  public class ThumbnailDbContext : KatalibDbContext, IThumbnailDbContext {
    private static Logger mLogger = LogManager.GetCurrentClassLogger ();

    private readonly IAppSettings mAppSettings;

    IApplicationContext context;

    public DbSet<AppMetaInfo> AppMetaInfos { get; set; }

    public DbSet<Thumbnail> Thumbnails { get; set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="context"></param>
    public ThumbnailDbContext (IApplicationContext context, IAppSettings appSettings) {
      this.context = context;
      this.mAppSettings = appSettings;
    }

    public IDbContextTransaction BeginTransaction() {
      return this.Database.BeginTransaction();
    }

    protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder) {
      if (!UsingHerokuPostgreSQLServerConnectionString (optionsBuilder)) {
        string databaseFilePath = Path.Combine (this.context.DatabaseDirectoryPath, "thumb.db");
        optionsBuilder.UseSqlite ("Data Source=" + databaseFilePath);
      }
    }

    protected override void OnModelCreating (ModelBuilder modelBuilder) { }

    /// <summary>
    /// Heroku環境下でPostgreSQLを使用するか判定する
    /// </summary>
    /// <returns></returns>
    private bool UsingHerokuPostgreSQLServerConnectionString (DbContextOptionsBuilder optionsBuilder) {
      if (!string.IsNullOrEmpty (this.mAppSettings.ENV_HEROKU_DATABASE_URL)) {
        MatchCollection results = Regex.Matches (this.mAppSettings.ENV_HEROKU_DATABASE_URL, @"postgres://(.+):(.+)@(.+):(\d+)\/(.+)");
        var UserName = results[0].Groups[1].Value;
        var Password = results[0].Groups[2].Value;
        var HostName = results[0].Groups[3].Value;
        var Port = results[0].Groups[4].Value;
        var DatabaseName = results[0].Groups[5].Value;

        optionsBuilder.UseNpgsql ($"Pooling=true;Use SSL Stream=True;SSL Mode=Require;TrustServerCertificate=True;Host={HostName};Port={Port};Username={UserName};Password={Password};Database={DatabaseName}");

        mLogger.Info ($"Heroku PostgreSQLを使用します。 Host={HostName}");
        return true;
      } else {
        return false;
      }
    }
  }
}

using System;
using System.IO;
using System.Text.RegularExpressions;
using Hyperion.Pf.Entity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using NLog;
using Snapshot.Server.Extention.Sdk;
using Snapshot.Server.Service.Infra;
using Snapshot.Server.Service.Infra.Model;
using Snapshot.Server.Service.Model;

namespace Snapshot.Server.Service.Gateway {

  /// <summary>
  /// アプリケーションの永続化データソースのコンテキスト
  /// </summary>
  public class AppDbContext : KatalibDbContext, IAppDbContext {
    private static Logger mLogger = LogManager.GetCurrentClassLogger ();

    private readonly IAppSettings mAppSettings;

    private IApplicationContext context;

    private ExtentionManager extentionManager;

    public DbSet<AppMetaInfo> AppMetaInfos { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Content> Contents { get; set; }

    public DbSet<FileMappingInfo> FileMappingInfos { get; set; }

    public DbSet<Label> Labels { get; set; }

    public DbSet<EavInteger> EavInts { get; set; }

    public DbSet<EavText> EavTexts { get; set; }

    public DbSet<EavBool> EavBools { get; set; }

    public DbSet<EavDate> EavDates { get; set; }

    public DbSet<EventLog> EventLogs { get; set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="context"></param>
    /// <param name="extentionManager"></param>
    /// <param name="appSettings"></param>
    public AppDbContext (IApplicationContext context, ExtentionManager extentionManager, IAppSettings appSettings) {
      this.context = context;
      this.extentionManager = extentionManager;
      this.mAppSettings = appSettings;
    }

    public IDbContextTransaction BeginTransaction () {
      return this.Database.BeginTransaction ();
    }

    protected IApplicationContext Context { get => context; }

    protected override void OnCreate (EntityEntry entry) {
      // NOTE: 各エンティティごとのCreateEntityカットポイントを呼び出します
      if (entry.Entity is IContent) {
        // TODO: コンテント情報を新規登録する際の拡張機能を呼び出します
      } else if (entry.Entity is ICategory) {
        // TODO: カテゴリ情報を新規登録する際の拡張機能を呼び出します
      } else if (entry.Entity is ILabel) {
        // TODO: ラベル情報を新規登録する際の拡張機能を呼び出します
      } else if (entry.Entity is IWorkspace) {
        // TODO: ワークスペース情報を新規登録する際の拡張機能を呼び出します
      }
    }

    protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder) {
      if (!UsingHerokuPostgreSQLServerConnectionString (optionsBuilder)) {
        string databaseFilePath = Path.Combine (this.context.DatabaseDirectoryPath, "pixstock.db");
        optionsBuilder.UseSqlite ("Data Source=" + databaseFilePath);
      }
    }

    protected override void OnModelCreating (ModelBuilder modelBuilder) {
      modelBuilder.Entity<Label2Content> ()
        .HasKey (t => new { t.LabelId, t.ContentId });

      modelBuilder.Entity<Label2Content> ()
        .HasOne (pt => pt.Content)
        .WithMany (p => p.Labels)
        .HasForeignKey (pt => pt.ContentId);

      modelBuilder.Entity<Label2Content> ()
        .HasOne (pt => pt.Label)
        .WithMany (t => t.Contents)
        .HasForeignKey (pt => pt.LabelId);

      modelBuilder.Entity<Label2Category> ()
        .HasKey (t => new { t.LabelId, t.CategoryId });

      modelBuilder.Entity<Label2Category> ()
        .HasOne (pt => pt.Category)
        .WithMany (p => p.Labels)
        .HasForeignKey (pt => pt.CategoryId);

      modelBuilder.Entity<Label2Category> ()
        .HasOne (pt => pt.Label)
        .WithMany (t => t.Categories)
        .HasForeignKey (pt => pt.LabelId);

      // EAV(複合キー)
      modelBuilder.Entity<EavInteger> ()
        .HasKey (c => new { c.CategoryName, c.EntityTypeName, c.Key });
      modelBuilder.Entity<EavText> ()
        .HasKey (c => new { c.CategoryName, c.EntityTypeName, c.Key });
      modelBuilder.Entity<EavBool> ()
        .HasKey (c => new { c.CategoryName, c.EntityTypeName, c.Key });
      modelBuilder.Entity<EavDate> ()
        .HasKey (c => new { c.CategoryName, c.EntityTypeName, c.Key });
    }

    /// <summary>
    /// Heroku環境下でPostgreSQLを使用するか判定する
    /// </summary>
    /// <returns>Herokuを使用する場合はTrueを返す</returns>
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

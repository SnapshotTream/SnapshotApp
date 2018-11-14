using System;
using Microsoft.EntityFrameworkCore.Storage;

namespace Snapshot.Server.Service.Infra {
  /// <summary>
  /// アプリケーション用データベースのコンテキスト
  /// </summary>
  public interface IThumbnailDbContext : IDisposable {
    int SaveChanges ();

    IDbContextTransaction BeginTransaction ();
  }
}

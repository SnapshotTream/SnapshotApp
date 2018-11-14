using System;
using Microsoft.EntityFrameworkCore.Storage;

namespace Foxpict.Service.Infra {
  /// <summary>
  /// アプリケーション用データベースのコンテキスト
  /// </summary>
  public interface IAppDbContext : IDisposable {
    int SaveChanges ();

    IDbContextTransaction BeginTransaction();
  }
}

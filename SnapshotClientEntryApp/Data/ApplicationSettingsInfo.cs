using Microsoft.Extensions.Configuration;

namespace Snapshot.Client.Entry.App.Data {
  /// <summary>
  /// アプリケーション設定情報
  /// /// </summary>
  public class ApplicationSettingsInfo {
    /// <summary>
    /// アプリケーション設定ファイル(appsettings)から読み込んだ値を設定したオブジェクトを返します。
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static ApplicationSettingsInfo FromAppSettings (IConfiguration configuration) {
      var obj = new ApplicationSettingsInfo ();
      configuration.Bind ("AppSettings", obj);
      return obj;
    }

    private ApplicationSettingsInfo () {

    }

    /// <summary>
    /// バックエンドサーバーアドレス
    /// </summary>
    /// <value></value>
    public string ServiceServerUrl { get; set; }
  }
}

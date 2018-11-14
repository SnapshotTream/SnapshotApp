using System;

namespace Foxpict.Service.Infra.Utils {
  public static class VfsLogicUtils {
    /// <summary>
    /// 生成したACLハッシュを取得する
    /// </summary>
    /// <returns></returns>
    public static string GenerateACLHash () {
      return Guid.NewGuid ().ToString ("N");
    }
  }
}

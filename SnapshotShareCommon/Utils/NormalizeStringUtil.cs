using System.Text.RegularExpressions;

namespace Snapshot.Share.Common.Utils {
  /// <summary>
  /// 文字列正規化ユーティリティ
  /// </summary>
  public class NormalizeStringUtil {
    /// <summary>
    /// 全角スペースを半角にする
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ReplaceSpaceZen (string str) {
      if (str == null || str.Length == 0) {
        return str;
      }

      char[] cs = str.ToCharArray ();
      int f = cs.Length;

      for (int i = 0; i < f; i++) {
        char c = cs[i];
        if (c == '　') {
          cs[i] = ' ';
        }
      }

      return new string (cs);
    }

    /// <summary>
    /// 全角半角スペースを除去する
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string RemoveSpace (string str) {
      if (str == null || str.Length == 0) {
        return str;
      }

      return Regex.Replace (str, "[ 　]+", "");
    }

    /// <summary>
    /// 全角記号を半角記号に置き換える
    /// </summary>
    /// <param name="str">変換する String。</param>
    /// <returns>変換された String。</returns>
    /// <remarks>
    /// Kana.ToHankakuメソッドと違い、「￥”’」も変換します。
    /// </remarks>
    public static string ToHankakuSign (string str) {
      if (str == null || str.Length == 0) {
        return str;
      }

      char[] cs = str.ToCharArray ();
      int f = cs.Length;

      for (int i = 0; i < f; i++) {
        char c = cs[i];

        if (c == '！') {
          cs[i] = '!';
        } else if (c == '＃') {
          cs[i] = '#';
        } else if (c == '＄') {
          cs[i] = '$';
        } else if (c == '％') {
          cs[i] = '%';
        } else if (c == '＊') {
          cs[i] = '*';
        } else if (c == 'ー') {
          cs[i] = '-';
        } else if (c == '．') {
          cs[i] = '.';
        } else if (c == '／') {
          cs[i] = '/';
        } else if (c == '：') {
          cs[i] = ':';
        } else if (c == '；') {
          cs[i] = ';';
        } else if (c == '＜') {
          cs[i] = '<';
        } else if (c == '＝') {
          cs[i] = '=';
        } else if (c == '＞') {
          cs[i] = '>';
        } else if (c == '？') {
          cs[i] = '?';
        } else if (c == '＠') {
          cs[i] = '@';
        } else if (c == '［') {
          cs[i] = '[';
        } else if (c == '］') {
          cs[i] = ']';
        } else if (c == '＾') {
          cs[i] = '^';
        } else if (c == '＿') {
          cs[i] = '_';
        } else if (c == '～') {
          cs[i] = '~';
        } else if (c == '￥') {
          cs[i] = '\\';
        } else if (c == '”' || c == '“') {
          cs[i] = '"';
        } else if (c == '’' || c == '‘') {
          cs[i] = '\'';
        }
      }

      return new string (cs);
    }

    /// <summary>
    /// 日本語オリジナル記号を置き換える
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string SanitizeCjkSign (string str) {
      if (str == null || str.Length == 0) {
        return str;
      }

      char[] cs = str.ToCharArray ();
      int f = cs.Length;

      for (int i = 0; i < f; i++) {
        char c = cs[i];
        if (c == '・' || c == '★' || c == '☆' || c == '※' || c == '〒' || c == '…') {
          cs[i] = '_';
        }
      }

      return new string (cs);
    }

    /// <summary>
    /// 全角英数字および記号を半角英数字および記号に変換します。
    /// </summary>
    /// <param name="str">変換する String。</param>
    /// <returns>変換された String。</returns>
    /// <remarks>
    /// Kana.ToHankakuメソッドと違い、「￥”’」も変換します。
    /// </remarks>
    public static string ToHankaku (string str) {
      if (str == null || str.Length == 0) {
        return str;
      }

      char[] cs = str.ToCharArray ();
      int f = cs.Length;

      for (int i = 0; i < f; i++) {
        char c = cs[i];
        // ！(0xFF01) ～ ～(0xFF5E)
        if ('！' <= c && c <= '～') {
          cs[i] = (char) (c - 0xFEE0);
        }
        // 全角スペース(0x3000) -> 半角スペース(0x0020)
        else if (c == '　') {
          cs[i] = ' ';
        } else if (c == '￥') {
          cs[i] = '\\';
        } else if (c == '”' || c == '“') {
          cs[i] = '"';
        } else if (c == '’' || c == '‘') {
          cs[i] = '\'';
        }
      }

      return new string (cs);
    }

    /// <summary>
    /// 連続空白を１つの空白にする
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToSingleSpace (string str) {
      return Regex.Replace (str, " +", " ");
    }
  }
}

using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Snapshot.Share.Common.Utils {
  public class StringUtil {
    /// <summary>
    /// 指定した長さより長い部分を切り捨て、「...」を末尾に追加した文字列を取得します。
    /// </summary>
    /// <param name="s">処理を行いたい文字列</param>
    /// <param name="maximumLength">切り捨て文字列数</param>
    /// <returns></returns>
    public static string Truncate (string s, int maximumLength) {
      return Truncate (s, maximumLength, "...");
    }

    /// <summary>
    /// 指定した長さより長い部分を切り捨て、任意の文字を末尾に追加した文字列を取得します。
    /// </summary>
    /// <param name="s">処理を行いたい文字列</param>
    /// <param name="maximumLength">切り捨て文字列数</param>
    /// <param name="suffix">末尾に追加する文字</param>
    /// <returns></returns>
    public static string Truncate (string s, int maximumLength, string suffix) {
      if (suffix == null)
        throw new ArgumentNullException ("suffix");

      if (maximumLength <= 0)
        throw new ArgumentException ("Maximum length must be greater than zero.", "maximumLength");

      int subStringLength = maximumLength - suffix.Length;

      if (subStringLength <= 0)
        throw new ArgumentException ("Length of suffix string is greater or equal to maximumLength");

      if (s != null && s.Length > maximumLength) {
        string truncatedString = s.Substring (0, subStringLength);
        // incase the last character is a space
        truncatedString = truncatedString.Trim ();
        truncatedString += suffix;

        return truncatedString;
      } else {
        return s;
      }
    }

    /// <summary>
    /// 日本語を含むかどうかを判定します。
    /// </summary>
    /// <param name="strMoji"></param>
    /// <returns></returns>
    public static bool IsKanji (string strMoji) {
      return System.Text.RegularExpressions.Regex.IsMatch (strMoji,
        @"[\p{IsCJKUnifiedIdeographs}" +
        @"\p{IsCJKCompatibilityIdeographs}" +
        @"\p{IsCJKUnifiedIdeographsExtensionA}]|" +
        @"[\uD840-\uD869][\uDC00-\uDFFF]|\uD869[\uDC00-\uDEDF]");
    }

    /// <summary>
    /// 半角全角の英数字かどうかを判定します。
    /// </summary>
    /// <param name="strMoji"></param>
    /// <returns></returns>
    public static bool IsAlpNumSignZenHan (string strMoji) {
      return !Regex.IsMatch (strMoji, @"[^a-zA-z0-9-_Ａ-Ｚa-z 　０-９]");
    }

    /// <summary>
    /// 指定文字列全てが半角文字列かどうかを判定します。
    /// </summary>
    /// <param name="strMoji ">判定対象文字列</param>
    /// <returns>true:半角文字列です。false:半角文字列ではありません（全半角混合、全角のみ）。</returns>
    public static bool IsHankaku (string strMoji) {
      Encoding.RegisterProvider (CodePagesEncodingProvider.Instance);

      Encoding sjisEncoding = Encoding.GetEncoding ("Shift_JIS");
      return (sjisEncoding.GetByteCount (strMoji) == strMoji.Length);
    }
  }
}

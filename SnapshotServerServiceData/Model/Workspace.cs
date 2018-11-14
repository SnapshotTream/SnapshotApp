using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Text.RegularExpressions;
using Foxpict.Service.Infra.Model;

namespace Foxpict.Service.Model {
  [Table ("svp_Workspace")]
  public class Workspace : IWorkspace {
    [Key]
    public long Id { get; set; }

    public string Name { get; set; }

    public string PhysicalPath { get; set; }

    public string VirtualPath { get; set; }

    public string ImportPath { get; set; }

    public DateTime? LastFullBuildDate { get; set; }

    public bool ContainsImport (FileSystemInfo fileSystemInfo) {
      var re = GenerateRegex (this.ImportPath);
      return re.Match (fileSystemInfo.FullName).Success;
    }

    public bool ContainsPhysical (FileSystemInfo fileSystemInfo) {
      var re = GenerateRegex (this.PhysicalPath);
      return re.Match (fileSystemInfo.FullName).Success;
    }

    public bool ContainsVirtual (FileSystemInfo fileSystemInfo) {
      var re = GenerateRegex (this.VirtualPath);
      return re.Match (fileSystemInfo.FullName).Success;
    }

    public string TrimWorekspacePath (string path) {
      // 仮想領域パス、もしくはインポート領域パスのいずれかを除去する。
      Regex re = GenerateRegex (this.VirtualPath);
      if (re.Match (path).Success) {
        string key = re.Replace (path, "");
        return key;
      }

      re = GenerateRegex (this.ImportPath);
      if (re.Match (path).Success) {
        string key = re.Replace (path, "");
        return key;
      }

      return path;
    }

    private Regex GenerateRegex (string path) {
      var escaped = Regex.Escape (path);
      return new Regex ("^" + escaped + @"[/\\]*", RegexOptions.Singleline);
    }

  }
}

using System.Collections.Generic;
using System.IO;
using Foxpict.Service.Infra.Model;

namespace Foxpict.Service.Infra {
  public interface IFileUpdateRunner {
    void file_create_acl (FileSystemInfo item, IWorkspace workspace);
    void file_create_normal (FileSystemInfo item, IWorkspace workspace);
    void file_remove_acl (FileSystemInfo item, IWorkspace workspace);
    void file_rename_acl (FileSystemInfo item, IWorkspace workspace);

    bool EnableCategoryParse { get; set; }
  }
}

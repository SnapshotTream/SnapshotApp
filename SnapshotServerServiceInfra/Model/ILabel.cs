using System.Collections.Generic;

namespace Snapshot.Server.Service.Infra.Model {
  public interface ILabel : Snapshot.Share.Common.Infra.Model.ILabel {

    void SetParentLabel (ILabel label);

    ILabel GetParentLabel ();

    List<ICategory> GetCategoryList ();

    List<IContent> GetContentList ();
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Foxpict.Service.Infra;
using Foxpict.Service.Infra.Model;
using Foxpict.Service.Infra.Repository;
using Foxpict.Service.Model;
using Hyperion.Pf.Entity.Repository;
using Microsoft.EntityFrameworkCore;
using Snapshot.Share.Common.Utils;

namespace Foxpict.Service.Gateway.Repository {
  public class LabelRepository : PixstockAppRepositoryBase<Label, ILabel>, ILabelRepository {
    public LabelRepository (IAppDbContext context) : base ((DbContext) context, "Label") {

    }

    /// <summary>
    /// エンティティの読み込み(静的メソッド)
    /// </summary>
    /// <remarks>
    /// エンティティの読み込みをワンライナーで記述できます。
    /// </remarks>
    /// <param name="context"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public static ILabel Load (IAppDbContext context, long id) {
      var repo = new LabelRepository (context);
      return repo.Load (id);
    }

    public IQueryable<ILabel> FindChildren (long parentLabelId) {
      var set = _dbset
        .Include (prop => prop.ParentLabel)
        .Include (prop => prop.Categories)
        .ThenInclude (category => category.Category)
        .Include (prop => prop.Contents)
        .ThenInclude (content => content.Content);
      return set.Where (x => x.ParentLabel.Id == parentLabelId);
    }

    public IQueryable<ILabel> FindRoot () {
      var set = _dbset
        .Include (prop => prop.ParentLabel)
        .Include (prop => prop.Categories)
        .ThenInclude (category => category.Category)
        .Include (prop => prop.Contents)
        .ThenInclude (content => content.Content);
      return set.Where (x => x.ParentLabel == null);
    }

    string GenerateNormalizeName (string name) {
      string ws = name;

      ws = NormalizeStringUtil.ToHankakuSign (ws);

      ws = NormalizeStringUtil.SanitizeCjkSign (ws);

      ws = NormalizeStringUtil.RemoveSpace (ws);

      ws = NormalizeStringUtil.ToHankaku (ws);

      return ws;
    }

    /// <summary>
    /// エンティティの読み込み
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ILabel Load (long id) {
      var set = _dbset
        .Include (prop => prop.ParentLabel)
        .Include (prop => prop.Categories)
        .ThenInclude (category => category.Category)
        .Include (prop => prop.Contents)
        .ThenInclude (content => content.Content);
      var entity = set.Where (x => x.Id == id).FirstOrDefault ();
      return entity;
    }

    public ILabel LoadByName (string name) {
      var nmzName = GenerateNormalizeName (name);

      var set = _dbset
        .Include (prop => prop.ParentLabel)
        .Include (prop => prop.Categories)
        .ThenInclude (category => category.Category)
        .Include (prop => prop.Contents)
        .ThenInclude (content => content.Content);
      var entity = set.Where (x => x.NormalizeName == nmzName).FirstOrDefault ();
      return entity;
    }

    public ILabel LoadByName (string name, string ownerType) {
      var nmzName = GenerateNormalizeName (name);

      var set = _dbset
        .Include (prop => prop.ParentLabel)
        .Include (prop => prop.Categories)
        .ThenInclude (category => category.Category)
        .Include (prop => prop.Contents)
        .ThenInclude (content => content.Content);
      var entity = set.Where (x => x.NormalizeName == nmzName /*&& x.OwnerType == ownerType*/ ).FirstOrDefault ();
      return entity;
    }

    public ILabel New () {
      var entity = new Label ();
      return this.Add (entity);
    }

    public void UpdateNormalizeName (ILabel label) {
      const int logicVersion = 1;
      Label labelImpl = label as Label;
      if (labelImpl == null)
        throw new ApplicationException ("Labelクラスへのキャストに失敗しました。");

      labelImpl.NormalizeLogicVersion = logicVersion;
      labelImpl.NormalizeName = GenerateNormalizeName (labelImpl.Name);
    }

    IEnumerable<ILabel> ILabelRepository.GetAll () {
      return base.GetAll ().Cast<ILabel> ();
    }
  }
}

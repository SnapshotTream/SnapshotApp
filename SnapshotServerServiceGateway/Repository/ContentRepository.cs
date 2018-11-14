using System.Linq;
using Foxpict.Service.Infra;
using Foxpict.Service.Infra.Model;
using Foxpict.Service.Infra.Repository;
using Foxpict.Service.Model;
using Hyperion.Pf.Entity.Repository;
using Microsoft.EntityFrameworkCore;
using Snapshot.Share.Common.Utils;

namespace Foxpict.Service.Gateway.Repository {
  public class ContentRepository : PixstockAppRepositoryBase<Content, IContent>, IContentRepository {
    public ContentRepository (IAppDbContext context) : base ((DbContext) context, "Content") { }

    /// <summary>
    /// エンティティの読み込み(静的メソッド)
    /// </summary>
    /// <remarks>
    /// エンティティの読み込みをワンライナーで記述できます。
    /// </remarks>
    /// <param name="context"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public static IContent Load (IAppDbContext context, long id) {
      var repo = new ContentRepository (context);
      return repo.Load (id);
    }

    /// <summary>
    /// Contentの読み込み
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IContent Load (long id) {
      var set = _dbset
        .Include (prop => prop.Category)
        .Include (prop => prop.FileMappingInfo);
      return set.Where (x => x.Id == id).FirstOrDefault ();
    }

    public IContent Load (IFileMappingInfo fileMappingInfo) {
      var set = _dbset
        .Include (prop => prop.Category)
        .Include (prop => prop.FileMappingInfo);
      return set.Where (x => x.FileMappingInfo.Id == fileMappingInfo.Id).FirstOrDefault ();
    }

    public IContent New () {
      var entity = new Content ();
      entity.IdentifyKey = RandomAlphameric.RandomAlphanumeric (10);
      return this.Add (entity);
    }
  }
}

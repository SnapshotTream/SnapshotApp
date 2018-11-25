using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using NLog;
using Snapshot.Server.Service.Apo.Builder;
using Snapshot.Server.Service.Infra.Core;
using Snapshot.Server.Service.Infra.Exception;
using Snapshot.Server.Service.Infra.Model;
using Snapshot.Server.Service.Infra.Repository;
using Snapshot.Server.Service.Model;

namespace Foxpict.Service.Web.Controllers {
  /// <summary>
  /// コンテント情報コントローラー
  /// </summary>
  [Route ("api/[controller]")]
  public class ArtifactController : Controller {
    private static Logger _logger = LogManager.GetCurrentClassLogger ();

    readonly ApiResponseBuilder mBuilder;

    readonly ICategoryRepository mCategoryRepository;

    readonly IContentRepository mContentRepository;

    readonly IFileMappingInfoRepository mFileMappingInfoRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="categoryRepository"></param>
    /// <param name="contentRepository"></param>
    /// <param name="fileMappingInfoRepository"></param>
    public ArtifactController (
      ApiResponseBuilder builder,
      ICategoryRepository categoryRepository,
      IContentRepository contentRepository,
      IFileMappingInfoRepository fileMappingInfoRepository
    ) {
      this.mBuilder = builder;
      this.mCategoryRepository = categoryRepository;
      this.mContentRepository = contentRepository;
      this.mFileMappingInfoRepository = fileMappingInfoRepository;
    }

    /// <summary>
    /// コンテント詳細情報取得API
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="id">コンテントID</param>
    /// <returns></returns>
    [HttpGet ("{id}")]
    public ResponseAapi<IContent> GetContent (long id) {
      var response = new ResponseAapi<IContent> ();
      try {
        mBuilder.AttachContentEntity (id, response);
        var content = response.Value;

        // リンクデータ
        // "category"
        if (content.GetCategory () != null)
          response.Link.Add ("category", content.GetCategory ().Id);
      } catch (Exception expr) {
        _logger.Error (expr.Message);
        throw new InterfaceOperationException ();
      }
      return response;
    }

    /// <summary>
    /// コンテント情報のリンクデータ(所属カテゴリ情報)を取得します
    /// </summary>
    /// <param name="id">コンテント情報のキー</param>
    /// <returns>所属カテゴリ情報</returns>
    [HttpGet ("{id}/category")]
    public ResponseAapi<ICategory> GetContentLink_Category (int id) {
      var response = new ResponseAapi<ICategory> ();
      try {
        var content = mContentRepository.Load (id);
        mBuilder.AttachCategoryEntity (mCategoryRepository.Load (content.GetCategory ().Id), response);
      } catch (Exception expr) {
        _logger.Error (expr.Message);
        throw new InterfaceOperationException ();
      }
      return response;
    }

    /// <summary>
    /// プレビューファイルを取得します
    /// </summary>
    /// <param name="id">コンテントID</param>
    /// <returns>コンテントのプレビューファイル</returns>
    [HttpGet ("{id}/preview")]
    public IActionResult FetchPreviewFile (long id) {
      _logger.Trace ("IN");
      var content = mContentRepository.Load (id);
      if (content == null) throw new InterfaceOperationException ("コンテント情報が見つかりません");

      var fmi = content.GetFileMappingInfo ();
      if (fmi == null) throw new InterfaceOperationException ("ファイルマッピング情報が見つかりません1");

      var efmi = mFileMappingInfoRepository.Load (fmi.Id);
      if (efmi == null) throw new InterfaceOperationException ("ファイルマッピング情報が見つかりません2");

      // NOTE: リソースの有効期限等を決定する
      DateTimeOffset now = DateTime.Now;
      var etag = new EntityTagHeaderValue ("\"" + Guid.NewGuid ().ToString () + "\"");
      string filePath = Path.Combine (efmi.GetWorkspace ().PhysicalPath, efmi.MappingFilePath);
      var file = PhysicalFile (
        Path.Combine (efmi.GetWorkspace ().PhysicalPath, efmi.MappingFilePath), efmi.Mimetype, now, etag);

      _logger.Trace ("OUT");
      return file;
    }

    /// <summary>
    /// コンテント情報を永続化します
    /// </summary>
    /// <param name="id">更新対象のコンテントID</param>
    /// <param name="content">更新オブジェクト</param>
    /// <returns></returns>
    [HttpPatch ("{id}")]
    public ResponseAapi<Boolean> UpdateContent (long id, [FromBody] Content content) {
      _logger.Trace ("IN");
      var response = new ResponseAapi<Boolean> ();

      var targetContent = mContentRepository.Load (id);
      if (targetContent == null) throw new InterfaceOperationException ("コンテント情報が見つかりません");

      targetContent.ArchiveFlag = content.ArchiveFlag;
      targetContent.Caption = content.Caption;
      targetContent.Comment = content.Comment;
      targetContent.StarRating = content.StarRating;
      targetContent.Name = content.Name;

      mContentRepository.Save ();
      response.Value = true;
      _logger.Trace ("OUT");
      return response;
    }

    /// <summary>
    /// コンテントのステータス更新処理を実行する
    /// </summary>
    /// <param name="id">更新対象のコンテントID</param>
    /// <param name="operation">更新処理種別</param>
    /// <returns>更新処理が正常終了した場合はtrue</returns>
    [HttpPut ("{id}/exec/{operation}")]
    public ResponseAapi<Boolean> UpdateContentStatus (long id, string operation) {
      _logger.Trace ("IN");
      var response = new ResponseAapi<Boolean> ();

      var content = mContentRepository.Load (id);
      if (content == null) throw new InterfaceOperationException ("コンテント情報が見つかりません");

      bool result = false;
      switch (operation) {
        case "read":
          if (!content.ReadableFlag) {
            content.ReadableFlag = true;
            content.ReadableDate = DateTime.Now;
          }
          content.LastReadDate = DateTime.Now;
          content.ReadableCount = content.ReadableCount + 1;
          result = true;
          break;
        default:
          _logger.Warn ($"不明なオペレーション({@operation})です。");
          break;
      }

      if (result) {
        mContentRepository.Save ();
      }

      response.Value = result;
      _logger.Trace ("OUT");
      return response;
    }
  }
}

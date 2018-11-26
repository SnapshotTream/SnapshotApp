using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NLog;
using ProtoBuf;
using Snapshot.Server.Service.Core.Service.Data;
using Snapshot.Server.Service.Infra;
using Snapshot.Server.Service.Infra.Core;
using Snapshot.Server.Service.Infra.Core.Model.Messaging;
using Snapshot.Server.Service.Infra.Model;
using Snapshot.Server.Service.Infra.Repository;
using Snapshot.Server.Service.Infra.Utils;
using Snapshot.Share.Common.Infra.Data.EventData;
using Snapshot.Share.Common.Utils;

namespace Snapshot.Server.Service.Core.Service {

  /// <summary>
  ///
  /// </summary>
  public class FileUpdateRunner : IFileUpdateRunner {
    /// <summary>
    /// ロガー
    /// </summary>
    /// <returns></returns>
    static Logger LOG = LogManager.GetCurrentClassLogger ();

    public static readonly string MSG_NEWCATEGORY = "Foxpict.MSG_NEWCATEGORY";

    /// <summary>
    /// カテゴリ名からラベル情報を取得するために使用するルールの最大数
    /// </summary>
    public static readonly int MAX_CATEGORYPARSEREGE = 1000;

    public static readonly string CategoryNameParserPropertyKey = "InitializeBuildCategoryNameParser";

    public static readonly string CategoryLabelNameParserPropertyKey = "InitializeBuildCategoryLabelNameParser";

    /// <summary>
    /// カテゴリ名をパースするかどうかのフラグ
    /// </summary>
    bool mEnableCategoryParse;

    readonly IAppAppMetaInfoRepository mAppAppMetaInfoRepository;

    readonly IFileMappingInfoRepository mFileMappingInfoRepository;

    readonly ICategoryRepository mCategoryRepository;

    readonly IContentRepository mContentRepository;

    readonly ILabelRepository mLabelRepository;

    readonly IThumbnailBuilder mTumbnailBuilder;

    readonly IMessagingScopeContext mMessagingScopeContext;

    readonly IEventLogRepository mEventLogRepository;

    readonly IVirtualFileSystemService mVirtualFileSystemService;

    /// <summary>
    /// カテゴリ名のパースを行うか
    /// </summary>
    /// <value></value>
    public bool EnableCategoryParse {
      get { return this.mEnableCategoryParse; }
      set { this.mEnableCategoryParse = value; }
    }

    /// <summary>
    /// コンストラクタ
    /// /// </summary>
    /// <param name="fileMappingInfoRepository"></param>
    /// <param name="categoryRepository"></param>
    /// <param name="contentRepository"></param>
    /// <param name="thumbnailBuilder"></param>
    /// <param name="appAppMetaInfoRepository"></param>
    /// <param name="labelRepository"></param>
    /// <param name="messagingScopeContext"></param>
    /// <param name="eventLogRepository"></param>
    /// <param name="virtualFileSystemService"></param>
    public FileUpdateRunner (
      IFileMappingInfoRepository fileMappingInfoRepository,
      ICategoryRepository categoryRepository,
      IContentRepository contentRepository,
      IThumbnailBuilder thumbnailBuilder,
      IAppAppMetaInfoRepository appAppMetaInfoRepository,
      ILabelRepository labelRepository,
      IMessagingScopeContext messagingScopeContext,
      IEventLogRepository eventLogRepository,
      IVirtualFileSystemService virtualFileSystemService) {
      this.mFileMappingInfoRepository = fileMappingInfoRepository;
      this.mCategoryRepository = categoryRepository;
      this.mContentRepository = contentRepository;
      this.mTumbnailBuilder = thumbnailBuilder;
      this.mAppAppMetaInfoRepository = appAppMetaInfoRepository;
      this.mLabelRepository = labelRepository;
      this.mMessagingScopeContext = messagingScopeContext;
      this.mEventLogRepository = eventLogRepository;
      this.mVirtualFileSystemService = virtualFileSystemService;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="item"></param>
    /// <param name="workspace"></param>
    public void file_create_acl (FileSystemInfo item, IWorkspace workspace) {
      // 1. 対象ファイルが存在するかチェック
      if (!item.Exists)
        throw new ApplicationException ("対象ファイルが指定位置に存在しません。");

      // 3. ACLファイルから、ACLハッシュを取得する
      var aclbin = ReadACLFile (new FileInfo (item.FullName));
      var aclhash = aclbin.FindKeyValue ("ACLHASH");

      // 4. データベースを参照し、ACLハッシュとファイルマッピング情報(AclHash)を突き合わせる
      IFileMappingInfo entity = mFileMappingInfoRepository.LoadByAclHash (aclhash);
      if (entity == null) throw new ApplicationException ();
      if (entity.GetWorkspace () == null || entity.GetWorkspace ().Id != workspace.Id)
        throw new ApplicationException ();

      // 5. ファイルマッピング情報を参照し、物理ファイルが存在するかチェックする
      var phyFileInfo = new FileInfo (Path.Combine (workspace.PhysicalPath, entity.MappingFilePath));
      if (!phyFileInfo.Exists) throw new ApplicationException ();

      // 6. 物理ファイルを、ACLファイルのパスに対応する物理空間のパスへ移動する
      //    移動先が、同じ場所となる場合は処理しない。
      var aclfileLocalPath_Update = workspace.TrimWorekspacePath (item.FullName);
      var extFilePath = Path.Combine (Path.GetDirectoryName (aclfileLocalPath_Update), Path.GetFileNameWithoutExtension (aclfileLocalPath_Update));
      var toFileInfo = new FileInfo (Path.Combine (workspace.PhysicalPath, extFilePath));
      if (phyFileInfo.FullName != toFileInfo.FullName) {
        Directory.CreateDirectory (toFileInfo.Directory.FullName);
        File.Move (phyFileInfo.FullName, toFileInfo.FullName);

        // 7. ファイルマッピング情報をDBに書き込む(コンテキスト初期化)
        entity = mFileMappingInfoRepository.LoadByAclHash (aclhash);
        entity.MappingFilePath = extFilePath; // 新しいファイルマップパス
      }
    }

    /// <summary>
    /// 任意のファイルをVFSに登録する
    /// </summary>
    /// <param name="item">登録対象のファイル</param>
    /// <param name="workspace">ワークスペース</param>
    public void file_create_normal (FileSystemInfo item, IWorkspace workspace) {
      LOG.Trace ("IN");

      if (!(item is FileInfo))
        throw new ApplicationException ("ファイル以外は処理できません。");
      if (!item.Exists)
        throw new ApplicationException ($"対象ファイル({item.FullName})が存在しません。");

      var fileMappingInfo = mVirtualFileSystemService.PersistentFileMapping (workspace, (FileInfo) item);
      var content = UpdateContentFromFileMapping (fileMappingInfo);

      // ファイルの移動
      mVirtualFileSystemService.RegisterFile ((FileInfo) item, fileMappingInfo);

      // サムネイル作成
      GenerateArtifact (content, workspace);
      LOG.Trace ("OUT");
    }

    /// <summary>
    /// LLD-03-05-01:01-02-01
    /// </summary>
    /// <param name="item"></param>
    /// <param name="workspace"></param>
    public void file_remove_acl (FileSystemInfo item, IWorkspace workspace) {
      LOG.Trace ("IN");
      var aclfileLocalPath_Remove = workspace.TrimWorekspacePath (item.FullName);
      var vrPath_Remove = Path.Combine (Path.GetDirectoryName (aclfileLocalPath_Remove), Path.GetFileNameWithoutExtension (aclfileLocalPath_Remove));

      // 1. 削除したファイルパスと一致するファイルマッピング情報を取得する
      var fmi = mFileMappingInfoRepository.LoadByPath (vrPath_Remove);

      // 2. ファイルマッピング情報から、物理空間のファイルを特定する
      var phyFilePath = Path.Combine (workspace.PhysicalPath, vrPath_Remove);

      // 3. 物理空間のファイルを削除する
      File.Delete (phyFilePath);

      // 4. ファイルマッピング情報をデータベースから削除する
      mFileMappingInfoRepository.Delete (fmi);

      LOG.Trace ("OUT");
    }

    /// <summary>
    /// [LLD-03-05-01:01-03-01]
    /// </summary>
    /// <param name="item"></param>
    /// <param name="workspace"></param>
    public void file_rename_acl (FileSystemInfo item, IWorkspace workspace) {
      LOG.Trace ("IN");
      // 1. 対象ファイルが存在するかチェック
      if (!item.Exists)
        throw new ApplicationException ("対象ファイルが指定位置に存在しません。");

      // 3. ACLファイルから、ACLハッシュを取得する
      var aclbin = ReadACLFile (new FileInfo (item.FullName));
      var aclhash = aclbin.FindKeyValue ("ACLHASH");

      // 4. データベースを参照し、ACLハッシュとファイルマッピング情報(AclHash)を突き合わせる
      IFileMappingInfo entity = mFileMappingInfoRepository.LoadByAclHash (aclhash);
      if (entity == null) throw new ApplicationException ();
      if (entity.GetWorkspace () == null || entity.GetWorkspace ().Id != workspace.Id)
        throw new ApplicationException ();

      // 5. ファイルマッピング情報を参照し、物理ファイルが存在するかチェックする
      var phyFileInfo = new FileInfo (Path.Combine (workspace.PhysicalPath, entity.MappingFilePath));
      if (!phyFileInfo.Exists) throw new ApplicationException ();

      // 6. 物理空間のファイルを、リネーム後のACLファイル名と同じ名前に変更する
      var aclfileLocalPath_Update = workspace.TrimWorekspacePath (item.FullName);
      var extFilePath = Path.Combine (Path.GetDirectoryName (aclfileLocalPath_Update), Path.GetFileNameWithoutExtension (aclfileLocalPath_Update));
      var toFileInfo = new FileInfo (Path.Combine (workspace.PhysicalPath, extFilePath));
      if (phyFileInfo.FullName != toFileInfo.FullName) {
        Directory.CreateDirectory (toFileInfo.Directory.FullName);
        File.Move (phyFileInfo.FullName, toFileInfo.FullName);

        // 7. ファイルマッピング情報をDBに書き込む(コンテキスト初期化)
        entity = mFileMappingInfoRepository.LoadByAclHash (aclhash);
        entity.MappingFilePath = extFilePath; // 新しいファイルマップパス
      }
      LOG.Trace ("OUT");
    }

    /// <summary>
    /// ファイルマッピング情報から、コンテント情報の作成または更新を行う。
    /// </summary>
    /// <param name="fileMappingInfo">ファイルマッピング情報</param>
    private IContent UpdateContentFromFileMapping (IFileMappingInfo fileMappingInfo) {
      // FileMappingInfoがContentとの関連が存在する場合、
      // 新規のContentは作成できないので例外を投げる。
      if (fileMappingInfo.Id != 0L && mContentRepository.Load (fileMappingInfo) != null) {
        throw new ApplicationException ("既にコンテント情報が作成済みのFileMappingInfoです。");
      }

      //---
      //!+ パス文字列から、階層構造を持つカテゴリを取得／作成を行うロジック
      //---

      //処理内容
      //   ・パスに含まれるカテゴリすべてが永続化されること
      //   ・Contentが永続化されること

      // パス文字列を、トークン区切りでキュー配列に詰めるロジック
      string pathText = fileMappingInfo.MappingFilePath;

      // 下記のコードは、Akalibへユーティリティとして実装する(パスを区切ってQueueを作成するユーティリティ)
      // 有効なパス区切り文字は、下記のコードでチェックしてください。
      // LOG.Info("トークン文字列1:　Token: {}", Path.AltDirectorySeparatorChar);
      // LOG.Info("トークン文字列2:　Token: {}", Path.DirectorySeparatorChar);
      // LOG.Info("トークン文字列3:　Token: {}", Path.PathSeparator);
      // LOG.Info("トークン文字列4:　Token: {}", Path.VolumeSeparatorChar);
      //
      // Windows環境: Path.DirectorySeparatorChar
      // Unix環境: Path.AltDirectorySeparatorChar
      var pathSplitedList = new Stack<string> (pathText.Split (Path.DirectorySeparatorChar, StringSplitOptions.None));
      var fileName = pathSplitedList.Pop (); // 最後の要素は必ずファイル名となるため、最後の要素を取り出す。
      var category = GenerateHierarchyCategory (new Queue<string> (pathSplitedList.Reverse<string> ()));

      // コンテントを格納するカテゴリのため、アルバムとしてマークする。
      category.AlbumFlag = true;

      var content = mContentRepository.New ();
      content.Name = fileName;
      content.SetFileMappingInfo (fileMappingInfo);
      content.SetCategory (category);

      return content;
    }

    /// <summary>
    /// 子階層に名称が一致するカテゴリがある場合は、そのカテゴリ情報を返します。
    /// ない場合は、新しいカテゴリを作成し、作成したカテゴリ情報を返します。
    /// </summary>
    /// <param name="parentCategory"></param>
    /// <param name="categoryName">検索カテゴリ名。または、新規登録時のカテゴリ名。</param>
    /// <param name="createdFlag">カテゴリを新規作成した場合Trueを設定します。</param>
    /// <returns>カテゴリ情報</returns>
    private ICategory CreateOrSelectCategory (ICategory parentCategory, string categoryName, out bool createdFlag) {
      ICategory category = null;
      foreach (var child in mCategoryRepository.FindChildren (parentCategory)) {
        if (child.Name == categoryName) {
          category = child;
          break;
        }
      }

      if (category == null) {
        category = mCategoryRepository.New ();
        category.Name = categoryName;
        category.SetParentCategory (parentCategory);
        mCategoryRepository.Save ();

        // EventLog登録
        var eventLog = mEventLogRepository.New ();
        eventLog.Datetime = DateTime.Now;
        eventLog.Owner = "SYSTEM";
        eventLog.EventType = "CREATE";
        eventLog.ValueFormat = "CreateEntity-JSON";
        EventDataUtil.ToValue (eventLog, new CreateEntityInfo {
          Name = categoryName,
            EntityName = "Category",
            EntityId = category.Id,
        });
        mEventLogRepository.Save ();

        createdFlag = true;
      } else {
        createdFlag = false;
      }

      return category;
    }

    /// <summary>
    /// サムネイルを生成します。
    /// </summary>
    /// <param name="content">コンテント情報</param>
    /// <param name="workspace">ワークスペース</param>
    private void GenerateArtifact (IContent content, IWorkspace workspace) {
      var sourceFileFullPath = System.IO.Path.Combine (workspace.PhysicalPath, content.GetFileMappingInfo ().MappingFilePath);

      string thumbnailHashKey = null;
      if (!string.IsNullOrEmpty (content.ThumbnailKey)) {
        thumbnailHashKey = content.ThumbnailKey;
      }

      content.ThumbnailKey = mTumbnailBuilder.BuildThumbnail (thumbnailHashKey, sourceFileFullPath);
    }

    /// <summary>
    /// 階層化されたカテゴリ情報をシステムに登録します
    /// </summary>
    /// <param name="directoryTreeNames">階層化カテゴリ名のキュー</param>
    /// <returns>キューの最後の要素を示すカテゴリ情報</returns>
    private ICategory GenerateHierarchyCategory (Queue<string> directoryTreeNames) {
      // パスから取得したトークン文字列と一致するカテゴリを取得します。
      // 該当のカテゴリが存在しない場合はカテゴリ情報を新規登録する。
      var loopCategory = mCategoryRepository.LoadRootCategory ();
      if (loopCategory == null) throw new ApplicationException ("ルートカテゴリが見つかりません");

      // キューから１つずつ要素を取得しエンティティの作成を行う。
      // 直前に作成したカテゴリ情報は、次の要素の親カテゴリに設定する。
      while (directoryTreeNames.Count > 0) {
        var oneText = directoryTreeNames.Dequeue ();
        bool parseSuccessFlag = false;
        string parsedCategoryName = oneText;

        if (mEnableCategoryParse) {
          parseSuccessFlag = AttachParsedCategoryName (oneText, out parsedCategoryName);
        }

        bool categoryCreatedFlag = false;
        loopCategory = CreateOrSelectCategory (loopCategory, parsedCategoryName, out categoryCreatedFlag);

        if (categoryCreatedFlag && parseSuccessFlag) {
          LOG.Info ($"{MSG_NEWCATEGORY}メッセージを配信します。 CategoryId={loopCategory.Id}");
          mMessagingScopeContext.Dispatcher (MSG_NEWCATEGORY, new NewCategoryMessageParameter {
            CategoryId = loopCategory.Id,
              EnableCategoryParse = mEnableCategoryParse
          });
        }
        if (mEnableCategoryParse) {
          AttachParsedLabel (oneText, loopCategory);
        }
      }

      return loopCategory;
    }

    /// <summary>
    /// カテゴリ名をパースし結果を取得する。
    /// パースルールは、アプリケーション設定情報から取得する
    /// </summary>
    /// <param name="categoryName">パース前のカテゴリ名</param>
    /// <param name="parsedCategoryName"></param>
    /// <returns>カテゴリ名がパースルールに適応したかどうか</returns>
    private bool AttachParsedCategoryName (string categoryName, out string parsedCategoryName) {
      // 最大でMAX_CATEGORYPARSEREGE個数の正規表現からラベルのパースを行う。
      // パースに使用する正規表現を定義するキーは、「CategoryNameParserPropertyKey + i」(iは、0〜MAX_CATEGORYPARSEREGE)という名称です。
      // ・連番でなければなりません。
      // ・最初にマッチした正規表現でループは停止します。
      for (int i = 0; i < MAX_CATEGORYPARSEREGE; i++) {
        Regex parserRegex = null;
        var parserApp = mAppAppMetaInfoRepository.LoadByKey (CategoryNameParserPropertyKey + i);
        if (parserApp != null) {
          parserRegex = new Regex (parserApp.Value);

          if (parserRegex != null) {
            var match = parserRegex.Match (categoryName);
            if (!match.Success) continue;
            foreach (Group group in match.Groups) {
              if (group.Name == "CategoryName") {
                parsedCategoryName = group.Value;
                return true;
              }
            }
          }
        } else {
          parsedCategoryName = categoryName;
          return false;
        }
      }

      parsedCategoryName = categoryName;
      return false;
    }

    /// <summary>
    /// カテゴリ名をパースし、ラベル情報との関連付けを行う。必要があれば、ラベル情報を新規作成する。
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    private bool AttachParsedLabel (string parseBase, ICategory category) {
      string categoryName = parseBase;

      // 最大でMAX_CATEGORYPARSEREGE個数の正規表現からラベルのパースを行う。
      // パースに使用する正規表現を定義するキーは、「CategoryLabelNameParserPropertyKey + i」(iは、0〜MAX_CATEGORYPARSEREGE)という名称です。
      // ・連番でなければなりません。
      // ・最初にマッチした正規表現でループは停止します。
      for (int i = 0; i < MAX_CATEGORYPARSEREGE; i++) {
        // アプリケーション設定情報から、ラベルパース用の正規表現を取得する
        // 0から順番に取得し、取得できなかった場合は処理失敗とする。
        Regex parserRegex = null;
        var parserApp = mAppAppMetaInfoRepository.LoadByKey (CategoryLabelNameParserPropertyKey + i);
        if (parserApp != null) {
          parserRegex = new Regex (parserApp.Value);
        } else {
          return false;
        }

        if (parserRegex != null) {
          var match = parserRegex.Match (categoryName);
          if (!match.Success) continue;

          LOG.Debug ("カテゴリ名からラベル生成に使用した正規表現={}", parserApp.Value);

          int currentGroupNumber = 0;
          foreach (Group group in match.Groups) {
            if (currentGroupNumber == 0) {
              currentGroupNumber++;
              continue;
            }

            // 空文字は登録しない
            if (string.IsNullOrWhiteSpace (group.ToString ())) {
              continue;
            }

            var _labelName = group.ToString ();

            // 正規表現グループから、マジックワードが含まれているかチェックする。
            var groupNames = group.Name.Split ("__");
            var groupName = groupNames[0];

            if (groupNames.Length == 2) {
              // マジックワード検証
              var magic = groupNames[1];
              if (magic == "SP1") {
                // 区切り文字で区切る
                char[] del = { ',', '、' };

                foreach (var splitedLabel in _labelName.Split (del)) {
                  RegisterLabel (splitedLabel, groupName);
                }
              }
            } else {
              RegisterLabel (_labelName, groupName);
            }

            void RegisterLabel (string labelName, string labelGroup) {
              var label = this.mLabelRepository.LoadByName (labelName, "Vfs");
              if (label == null) {
                label = this.mLabelRepository.New ();
                label.Name = labelName;
                label.MetaType = labelGroup == currentGroupNumber.ToString () ? "" : labelGroup;
                mLabelRepository.UpdateNormalizeName (label);
                this.mLabelRepository.Save ();
                category.AddLabelRelation (label, LabelCauseType.EXTENTION);
              } else {
                label.MetaType = labelGroup == currentGroupNumber.ToString () ? "" : labelGroup;
                this.mLabelRepository.Save ();
                category.AddLabelRelation (label, LabelCauseType.EXTENTION);
              }
            }

            currentGroupNumber++;
          }

          return true;
        }
      }
      return false;
    }

    private AclFileStructure ReadACLFile (FileInfo aclFillePath) {
      using (var file = File.OpenRead (aclFillePath.FullName)) {
        return Serializer.Deserialize<AclFileStructure> (file);
      }
    }
  }
}

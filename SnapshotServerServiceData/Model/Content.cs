using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Foxpict.Service.Infra.Model;

namespace Foxpict.Service.Model {
  [Table ("svp_Content")]
  public class Content : Infra.Model.IContent {
    [Key]
    public long Id { get; set; }

    [JsonIgnore]
    public List<Label2Content> Labels { get; set; }

    [JsonIgnore]
    public Category Category { get; set; }

    [JsonIgnore]
    public FileMappingInfo FileMappingInfo { get; set; }

    public string Name { get; set; }

    public string IdentifyKey { get; set; }

    public string ContentHash { get; set; }

    public string Caption { get; set; }

    public string Comment { get; set; }

    public bool ArchiveFlag { get; set; }

    public bool ReadableFlag { get; set; }

    public int StarRating { get; set; }

    public string ThumbnailKey { get; set; }

    public int ReadableCount { get; set; }

    public DateTime? LastReadDate { get; set; }

    public DateTime? ReadableDate { get; set; }

    public ICategory GetCategory () => this.Category;

    public void SetCategory (ICategory category) => this.Category = (Category) category;

    public IFileMappingInfo GetFileMappingInfo () => this.FileMappingInfo;

    public void SetFileMappingInfo (IFileMappingInfo fileMappingInfo) => this.FileMappingInfo = (FileMappingInfo) fileMappingInfo;
  }
}

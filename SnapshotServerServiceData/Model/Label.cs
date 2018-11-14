using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Foxpict.Service.Infra.Model;
using Hyperion.Pf.Entity;
using Newtonsoft.Json;

namespace Foxpict.Service.Model {
  [Table ("svp_Label")]
  public class Label : Infra.Model.ILabel, IAuditableEntity {
    [Key]
    public long Id { get; set; }

    public string Name { get; set; }

    public string NormalizeName { get; set; }

    public int NormalizeLogicVersion { get; set; }

    public string Comment { get; set; }

    public string MetaType { get; set; }

    [JsonIgnore]
    public Label ParentLabel { get; set; }

    [JsonIgnore]
    public List<Label2Content> Contents { get; set; }

    [JsonIgnore]
    public List<Label2Category> Categories { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public string UpdatedBy { get; set; }

    public DateTime UpdatedDate { get; set; }

    public ILabel GetParentLabel () => this.ParentLabel;

    public void SetParentLabel (ILabel label) => this.ParentLabel = (Label) label;

    public List<IContent> GetContentList () => this.Contents.Select (p => (IContent) p.Content).ToList ();

    public List<ICategory> GetCategoryList () => this.Categories.Select (p => (ICategory) p.Category).ToList ();
  }
}

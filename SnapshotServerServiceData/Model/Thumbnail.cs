using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Snapshot.Server.Service.Infra.Model;
using Snapshot.Share.Common.Types;

namespace Snapshot.Server.Service.Model
{
    [Table("svp_Thumbnail")]
    public class Thumbnail : Service.Infra.Model.IThumbnail
    {
        [Key]
        public long Id { get; set; }

        public string ThumbnailKey { get; set; }

        public byte[] BitmapBytes { get; set; }

        public string MimeType { get; set; }

        public ThumbnailType ThumbnailType { get; set; }
    }
}

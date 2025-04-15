using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSSA.Models
{
    public class Attachment
    {
        [Key]
        [Column("file_id")]
        public int FileId { get; set; }

        [Required]
        [Column("proposal_id")]
        public int ProposalId { get; set; }

        [Required]
        [Column("file_name")]
        public string FileName { get; set; }

        [Required]
        [Column("file_url")]
        public string FileUrl { get; set; }

        [Required]
        [Column("uploaded_by")]
        public int UploadedBy { get; set; }

        [Column("uploaded_at")]
        public DateTime UploadedAt { get; set; }
    }
}

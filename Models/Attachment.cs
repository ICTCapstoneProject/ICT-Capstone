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
        [Column("file_path")]
        public string FileUrl { get; set; }

        [Required]
        [Column("type_id")]
        public int TypeId { get; set; }

        [ForeignKey("TypeId")]
        public AttachmentType AttachmentType { get; set; }

        [ForeignKey("ProposalId")]
        public virtual Proposal Proposal { get; set; }

    }
}

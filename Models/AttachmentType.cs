using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSSA.Models
{
   public class AttachmentType
    {
        [Key]
        [Column("type_id")]
        public int TypeId { get; set; }

        [Required]
        [Column("type_name")]
        public string TypeName { get; set; }
    }
}

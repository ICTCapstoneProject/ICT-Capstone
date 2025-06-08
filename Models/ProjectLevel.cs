using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSSA.Models
{
    public class ProjectLevel
    {
        [Key]
        [Column("level_id")]
        public int LevelId { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("level_name")]
        public string LevelName { get; set; }
    }
}

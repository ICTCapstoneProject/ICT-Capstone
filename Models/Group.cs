using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSSA.Models
{
    public class Group
    {
        [Key]
        [Column("group_id")]
        public int GroupId { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; }
    }
}

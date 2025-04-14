using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSSA.Models
{
    [Table("user_groups")]
    public class UserGroup
    {
        [Key, Column("user_id", Order = 0)]
        public int UserId { get; set; }

        [Key, Column("group_id", Order = 1)]
        public int GroupId { get; set; }

        [Column("role_id")]
        public int RoleId { get; set; }
    }
}

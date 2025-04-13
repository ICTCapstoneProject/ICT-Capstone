using System.ComponentModel.DataAnnotations.Schema;

namespace FSSA.Models
{
    public class UserGroup
    {
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("group_id")]
        public int GroupId { get; set; }

        [Column("role_id")]
        public int RoleId { get; set; }
    }
}

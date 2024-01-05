using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocnetCorePractice.Data.Entity
{
    [Table("Refresh Token")]
    public class RefreshTokens
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string TokenID { get; set; }

        [ForeignKey("UserId")]
        public UserEntity User { get; set; }
        public string UserId { get; set; }

        public string? Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsActive { get; set; }
    }
}

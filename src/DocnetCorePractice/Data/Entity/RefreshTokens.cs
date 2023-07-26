using System.ComponentModel.DataAnnotations.Schema;

namespace DocnetCorePractice.Data.Entity
{
    public class RefreshTokens
    {
        public string Id { get; set; }

        [ForeignKey("UserId")]
        public UserEntity User { get; set; }
        public string UserId { get; set; }

        public string? Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsActive { get; set; }
    }
}

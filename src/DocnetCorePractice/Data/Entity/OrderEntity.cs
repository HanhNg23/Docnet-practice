using DocnetCorePractice.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocnetCorePractice.Data.Entity
{
    [Table("Order")]
    public class OrderEntity : Entity
    {        
 //     public string OrderID { get; set; }
        public double TotalPrice { get; set; }
        public StatusOrder Status { get; set; }
        
        [ForeignKey("UserId")] 
        public string UserId { get; set; }
        public virtual UserEntity? User { get; set; } //to create foreign key

        
    }
}

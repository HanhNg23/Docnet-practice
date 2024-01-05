using System.ComponentModel.DataAnnotations.Schema;

namespace DocnetCorePractice.Data.Entity
{
    [Table("OrderItem")]
    public class OrderItemEntity : Entity
    {
        [ForeignKey("UserEntityId")]
        public string UserId { get; set; }
        public virtual UserEntity User { get; set; }
        [ForeignKey("CaffeEntityId")]
        public string CaffeId { get; set; }
        public virtual CaffeEntity Caffe { get; set; }
        [ForeignKey("OrderEntityId")]
        public string OrderId { get; set; }
        public virtual OrderEntity Order { get; set; }
        public string CaffeName { get; set; }
        public int Volumn { get; set; }
        public double UnitPrice { get; set; }
        public double ItemPrice { get; set; }
        public bool IsActice { get; set; }
        public bool IsDeleted { get; set; }
    }
}

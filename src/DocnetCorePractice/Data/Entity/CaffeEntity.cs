using DocnetCorePractice.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocnetCorePractice.Data.Entity
{
    [Table("Caffe")]
    public class CaffeEntity : Entity
    {
        public CaffeEntity() 
        { 
//      Id = Guid.NewGuid().ToString();
        }

        //      public string Id { get; set; }
        public string Name { get; set; }
        public ProductType Type { get; set; }
        public double Price { get; set; }
        public int Discount { get; set; }
        public bool IsActive { get; set; }
    }
}

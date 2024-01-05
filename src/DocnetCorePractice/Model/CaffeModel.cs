using DocnetCorePractice.Enum;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DocnetCorePractice.Model
{
    public class CaffeModel
    {
        [AllowNull]
        public string Id { get; set; }
        [Required]
        [NotNull]
        public string Name { get; set; }
        public ProductType Type { get; set; }
        public double Price { get; set; }
        public int Discount { get; set; }
    }

    public class RequestUpdateCaffe
    {
        public double Price { get; set; }
        public int Discount { get; set; }
    }
}

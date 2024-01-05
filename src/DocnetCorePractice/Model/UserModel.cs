using DocnetCorePractice.Enum;
using System.ComponentModel.DataAnnotations;

namespace DocnetCorePractice.Model
{
    public class UserModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Sex Sex { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public double Balance { get; set; }
        public int TotalProduct { get; set; }

        public string Account { get; set; }
    }
}

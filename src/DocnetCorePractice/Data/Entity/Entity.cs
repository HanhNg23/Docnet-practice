using System.ComponentModel.DataAnnotations;

namespace DocnetCorePractice.Data.Entity
{
    public abstract class Entity
    {
        protected Entity()
        {
            Id = Guid.NewGuid().ToString();
            CreateTimes = DateTime.Now;
            LastUpdateTimes = DateTime.Now;
            LastUpdateUser = DateTime.Now;
            CreateUser = "HA";
        }

        [Key]
        public string Id { get; set; }
        public DateTime CreateTimes { get; set; }
        public DateTime LastUpdateTimes { get; set; }
        public String CreateUser { get; set; }
        public DateTime LastUpdateUser { get; set; }
    }
}

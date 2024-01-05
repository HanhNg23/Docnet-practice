using DocnetCorePractice.Data;
using Microsoft.EntityFrameworkCore;

namespace DocnetCorePractice.Repository
{
    public class CaffeRepository
    {
        private readonly DbContext _context;
        public CaffeRepository(DbContext context) 
        {
            _context = context;
        }
    }
}

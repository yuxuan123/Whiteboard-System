using Microsoft.EntityFrameworkCore;
using WhiteboardAPI.Entities;

namespace WhiteboardAPI.Database
{
    public class WhiteboardContext : DbContext
    {
        public WhiteboardContext(DbContextOptions<WhiteboardContext> options) : base(options)
        {

        }

        public DbSet<User> tbl_user { get; set; }
    }
}
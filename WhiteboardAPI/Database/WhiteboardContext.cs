using Microsoft.EntityFrameworkCore;
using WhiteboardAPI.Entities;

namespace WhiteboardAPI.Database
{
    public class WhiteboardContext : DbContext
    {
        public WhiteboardContext(DbContextOptions<WhiteboardContext> options) : base(options)
        {

        }

        public DbSet<UserDE> tbl_user { get; set; }
        public DbSet<CourseDE> tbl_course { get; set; }
        public DbSet<CourseStaffDE> tbl_course_staff { get; set; }
        public DbSet<CourseStudentDE> tbl_course_student { get; set; }
        public DbSet<ContentDE> tbl_content { get; set; }
        public DbSet<PostDE> tbl_db_post { get; set; }
        public DbSet<ReplyDE> tbl_db_reply { get; set; }
        public DbSet<CourseFolderDE> tbl_db_course_folder { get; set; }
        public DbSet<PostFolderDE> tbl_db_post_folder { get; set; }
        public DbSet<ChatDE> tbl_chat { get; set; }
    }
}
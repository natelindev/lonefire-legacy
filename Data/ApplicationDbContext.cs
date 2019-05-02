using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using lonefire.Models;
using lonefire.Models.ArticleViewModels;
using lonefire.Models.CommentViewModels;

namespace lonefire.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ArticleIndexVM> ArticleIndexVM { get; set; }
        public virtual DbSet<Article> Article { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Customize the ASP.NET Core Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Core Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<Article>().ToTable("Articles");
            builder.Entity<Comment>().ToTable("Comments");
            builder.Entity<Tag>().ToTable("Tags");

            builder.Entity<Comment>().Property(p => p.CommentID).ValueGeneratedOnAdd();
            builder.Entity<Comment>().Property(p => p.AddTime).HasDefaultValueSql("GETDATE()").ValueGeneratedOnAdd();
            builder.Entity<Article>().Property(p => p.ArticleID).ValueGeneratedOnAdd();
            builder.Entity<Article>().Property(p => p.AddTime).HasDefaultValueSql("GETDATE()").ValueGeneratedOnAdd();
            builder.Entity<Tag>().Property(p => p.TagID).ValueGeneratedOnAdd();

            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
        }

    }
}

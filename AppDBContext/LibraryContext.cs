using BookManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.AppDBContext
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) 
            : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Authors> Authors { get; set; }
        public DbSet<Books> Books { get; set; }
        public DbSet<BooksBorrowed> BorrowedBooks { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Authors>()
                .HasMany(a => a.Books)
                .WithOne(b => b.Author)
                .HasForeignKey(b => b.AuthorId);
            modelBuilder.Entity<Books>()
                .HasMany(b => b.BooksBorrowed)
                .WithOne(bb => bb.Book)
                .HasForeignKey(bb => bb.BookId);

            base.OnModelCreating(modelBuilder);
        }
    }
}

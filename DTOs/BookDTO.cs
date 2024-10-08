using BookManagement.Models;

namespace BookManagement.DTOs
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public DateTime PublishedDate { get; set; }
        public int TotalCopies { get; set; }         // Total number of copies we have
        public int CopiesAvailable { get; set; }     // Number of copies currently available
        // Foreign Key representing the Author this book belongs to
        public int AuthorId { get; set; }
    }
}

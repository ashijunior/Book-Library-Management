namespace BookManagement.Models
{
    public class Books
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public DateTime PublishedDate { get; set; }
        public int TotalCopies { get; set; }         // Total number of copies we have
        public int CopiesAvailable { get; set; }     // Number of copies currently available

        // Updated IsAvailable property
        public bool IsAvailable
        {
            get { return CopiesAvailable > 0; }
        }

        // Foreign Key representing the Author this book belongs to
        public int AuthorId { get; set; }

        // Navigation Property
        public Authors Author { get; set; }

        // Navigation Property for BooksBorrowed
        public ICollection<BooksBorrowed> BooksBorrowed { get; set; }
    }
}

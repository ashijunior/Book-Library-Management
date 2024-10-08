namespace BookManagement.Models
{
    public class BooksBorrowed
    {
        public int Id { get; set; }
        // Foreign Key for the Book
        public int BookId {  get; set; }
        public Books Book {  get; set; }
        // Foreign Key for the User who borrowed book
        public int UserId { get; set; }
        public Users User { get; set; }
        public DateTime BorrowedDate { get; set; } // Date when the book was borrowed
        public DateTime? ReturnedDate { get; set; }  // Nullable, only populated when the book is returned
    }
}

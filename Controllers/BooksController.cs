using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookManagement.Models;
using BookManagement.AppDBContext;
using BookManagement.DTOs;
using BookManagement.Mapper;
using AutoMapper;

namespace BookManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryContext _context; 
        private readonly IMapper _mapper;

        public BooksController(LibraryContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // 1. Get the number of total copies of a particular book by title
        [HttpGet("count/{title}")]
        public async Task<ActionResult<int>> GetTotalCopiesByTitle(string title)
        {
            int count = await _context.Books
                .Where(b => b.Title.ToLower() == title.ToLower())
                .SumAsync(b => b.TotalCopies); // Sum all copies of the book with the same title

            return Ok(count);
        }

        // 2. Borrow a book
        [HttpPost("{id}/borrow/{userId}")]
        public async Task<IActionResult> BorrowBook(int id, int userId)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null || book.CopiesAvailable <= 0)
            {
                return BadRequest("Book not available for borrowing.");
            }

            var borrowedBook = new BooksBorrowed
            {
                BookId = id,
                UserId = userId,
                BorrowedDate = DateTime.Now,
                ReturnedDate = null // Set to null initially
            };

            // Decrease the available copies
            book.CopiesAvailable--;

            await _context.BorrowedBooks.AddAsync(borrowedBook);
            await _context.SaveChangesAsync();

            return Ok("Book borrowed successfully.");
        }


        // 3. Return a book
        [HttpPost("{id}/return/{userId}")]
        public async Task<IActionResult> ReturnBook(int id, int userId)
        {
            var borrowedBook = await _context.BorrowedBooks
                .Where(bb => bb.BookId == id && bb.UserId == userId && bb.ReturnedDate == null)
                .FirstOrDefaultAsync();

            if (borrowedBook == null)
                return NotFound("No active borrowed record found for this book and user.");

            // Update the ReturnedDate to mark the book as returned
            borrowedBook.ReturnedDate = DateTime.Now;

            // Increase the available copies by 1
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                book.CopiesAvailable++;
            }

            await _context.SaveChangesAsync();

            return Ok($"Book '{book.Title}' has been returned successfully.");
        }

        //Create a Book
        [HttpPost("CreateBook")]
        public async Task<IActionResult> CreateBook([FromBody] BookDTO books)
        {
            try
            {
                // Check if the book DTO is null
                if (books == null)
                {
                    return BadRequest("Book data cannot be null.");
                }

                // Check if the Author exists (assuming the author must exist to create a book)
                var author = await _context.Authors.FindAsync(books.AuthorId);
                if (author == null)
                {
                    return NotFound("Author not found.");
                }

                // Create the book using AutoMapper to map BookDTO to Books entity
                var book = _mapper.Map<Books>(books);

                // Add the book to the context
                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                return Ok($"Book '{book.Title}' has been created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // 5. Get all books
        [HttpGet("GetAllBooks")]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetAllBooks()
        {
            try
            {
                // Fetch all books from the database
                var books = await _context.Books.Include(b => b.Author).ToListAsync();

                // Use AutoMapper to map the books list to BookDTO list
                var booksDTO = _mapper.Map<IEnumerable<BookDTO>>(books);

                return Ok(booksDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // 6. Get book by ID
        [HttpGet("GetBookById/{id}")]
        public async Task<ActionResult<BookDTO>> GetBookById(int id)
        {
            try
            {
                // Fetch the book from the database by its ID, including its related Author
                var book = await _context.Books.Include(b => b.Author)
                                               .FirstOrDefaultAsync(b => b.Id == id);

                // Check if the book exists
                if (book == null)
                {
                    return NotFound($"Book with ID {id} not found.");
                }

                // Use AutoMapper to map the book to BookDTO
                var bookDTO = _mapper.Map<BookDTO>(book);

                return Ok(bookDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // 7. Update a Book
        [HttpPut("UpdateBookById/{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] BookDTO updatedBookDTO)
        {
            if (updatedBookDTO == null || id != updatedBookDTO.Id)
            {
                return BadRequest("Book data is invalid.");
            }

            try
            {
                // Fetch the book from the database
                var book = await _context.Books.FindAsync(id);

                if (book == null)
                {
                    return NotFound($"Book with ID {id} not found.");
                }

                // Use AutoMapper to map the updated DTO to the book entity
                _mapper.Map(updatedBookDTO, book);

                // Save changes to the database
                await _context.SaveChangesAsync();

                return Ok($"Book with ID {id} updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // 8. Delete a Book
        [HttpDelete("RemoveBookById/{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                // Fetch the book from the database
                var book = await _context.Books.FindAsync(id);

                if (book == null)
                {
                    return NotFound($"Book with ID {id} not found.");
                }

                // Remove the book from the database
                _context.Books.Remove(book);

                // Save changes to the database
                await _context.SaveChangesAsync();

                return Ok($"Book with ID {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // 9. Get all Books Borrowed
        [HttpGet("GetAllBooksBorrowed")]
        public async Task<IActionResult> GetBorrowedBooks()
        {
            try
            {
                // Fetch all borrowed books from the database including the related Book and User details
                var borrowedBooks = await _context.BorrowedBooks
                    .Include(bb => bb.Book)
                    .Include(bb => bb.User)
                    .ToListAsync();

                // Use AutoMapper to map the borrowed books list to BooksBorrowedDTO list
                var borrowedBooksDTO = _mapper.Map<IEnumerable<BooksBorrowedDTO>>(borrowedBooks);

                return Ok(borrowedBooksDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // 10. Get all borrowed books by user ID
        [HttpGet("GetBorrowedBooksByUserId/{userId}")]
        public async Task<IActionResult> GetBorrowedBooksByUserId(int userId)
        {
            try
            {
                // Fetch all borrowed books for the specific user, including related Book and User details
                var borrowedBooks = await _context.BorrowedBooks
                    .Where(bb => bb.UserId == userId)
                    .Include(bb => bb.Book)  // Include the related book details
                    .Include(bb => bb.User)  // Include the user details (optional, if needed)
                    .ToListAsync();

                // Check if the user has any borrowed books
                if (borrowedBooks == null || borrowedBooks.Count == 0)
                {
                    return NotFound($"No borrowed books found for User with ID {userId}.");
                }

                // Use AutoMapper to map the borrowed books list to BooksBorrowedDTO list
                var borrowedBooksDTO = _mapper.Map<IEnumerable<BooksBorrowedDTO>>(borrowedBooks);

                return Ok(borrowedBooksDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}

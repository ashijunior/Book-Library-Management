using BookManagement.AppDBContext;
using BookManagement.DTOs;
using BookManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly LibraryContext _libraryContext; //Add database context
        public AuthorsController(LibraryContext libraryContext)
        {
            _libraryContext = libraryContext;
        }

        //Create Author
        [HttpPost("CreateAuthor")]
        public async Task<IActionResult> CreateAuthor([FromBody] AuthorDTO author)
        {
            try
            {
                //Check if result is null/empty
                if (author == null)
                {
                    return BadRequest("Author is empty");
                }
                //Create new Author
                var authors = new Authors
                {
                    Name= author.Name,
                    Bio= author.Bio
                };
                //Add and Save data to the Database
                await _libraryContext.Authors.AddAsync(authors);
                await _libraryContext.SaveChangesAsync();

                return Ok(authors);
            }
            catch (Exception ex) 
            {
                //Log the exception(optional)
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        //Get all Authors
        [HttpGet("GetAllAuthors")]
        public async Task<ActionResult<IEnumerable<Authors>>> GetAuthors()
        {
            try
            {
                return await _libraryContext.Authors.Include(a => a.Books).ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // Get Author by Id
        [HttpGet("GetAuthorById/{id}")]
        public async Task<ActionResult<Authors>> GetAuthorById(int id)
        {
            try
            {
                // Fetch the author by ID, including the related books
                var author = await _libraryContext.Authors
                    .Include(a => a.Books)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (author == null)
                {
                    return NotFound($"Author with ID {id} not found.");
                }

                return Ok(author);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        //Update Author by Id
        [HttpPut("UpdateAuthorById/{Id}")]
        public async Task<IActionResult> UpdateAuthor(int Id, [FromBody] AuthorDTO updateAuthor)
        {
            try
            {
                //Find Author in the database
                var existingAuthor = await _libraryContext.Authors.FindAsync(Id);

                //Check if Author exists
                if(existingAuthor == null)
                {
                    return BadRequest("Author does not exist");
                }

                //Update Author in the database
                existingAuthor.Name = updateAuthor.Name;
                existingAuthor.Bio = updateAuthor.Bio;

                //Save changes to the database
                await _libraryContext.SaveChangesAsync();

                return Ok(existingAuthor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        //Delete Author by Id
        [HttpDelete("DeleteAuthorById/{Id}")]
        public async Task<IActionResult> RemoveAuthor(int Id)
        {
            try
            {
                //Find the Author in the database
                var author = await _libraryContext.Authors.FindAsync(Id);

                //Checks if Author exist in database
                if(author == null)
                {
                    return BadRequest("Author does not exist");
                }
                //Remove the Author from database and then save it
                _libraryContext.Authors.Remove(author);
                await _libraryContext.SaveChangesAsync();

                return Ok("Author deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}

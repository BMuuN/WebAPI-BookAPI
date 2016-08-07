using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Linq.Expressions;

using BookAPI.Models;
using BookAPI.DTOs;

namespace BookAPI.Controllers
{
    [RoutePrefix("api/books")]
    public class BooksController : ApiController
    {
        #region Variables

        private BookAPIContext db = new BookAPIContext();
        
        private static readonly Expression<Func<Book, BookDto>> AsBookDto =
            x => new BookDto
            {
                Title = x.Title,
                Author = x.Author.Name,
                Genre = x.Genre
            };

        #endregion

        #region Public Methods

        /// <summary>
        /// Get all books.
        /// </summary>
        /// <example>
        /// GET: api/books
        /// </example>
        /// <returns>
        /// A collection of <see cref="BookDto"/>.
        /// </returns>
        [Route("")]
        public IQueryable<BookDto> GetBooks()
        {
            return db.Books.Include(b => b.Author).Select(AsBookDto);
        }

        /// <summary>
        /// Get book by id.
        /// </summary>
        /// <example>
        /// GET: api/books/{id}
        /// </example>
        /// <param name="id">The unique id of the book to retrieve.</param>
        /// <returns>
        /// A <see cref="BookDto"/> populated with information relating to the specified book.
        /// </returns>
        [Route("{id:int}")]
        [ResponseType(typeof(BookDto))]
        public async Task<IHttpActionResult> GetBook(int id)
        {
            BookDto book = await db.Books.Include(b => b.Author)
                .Where(b => b.BookId == id)
                .Select(AsBookDto)
                .FirstOrDefaultAsync();
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        /// <summary>
        /// Get book details.
        /// </summary>
        /// <example>
        /// GET: api/books/{id}/details
        /// </example>
        /// <param name="id">The unique id of the book to retrieve.</param>
        /// <returns>
        /// A <see cref="BookDetailDto"/> containing all the details for the specified book.
        /// </returns>
        [Route("{id:int}/details")]
        [ResponseType(typeof(BookDetailDto))]
        public async Task<IHttpActionResult> GetBookDetail(int id)
        {
            var book = await (from b in db.Books.Include(b => b.Author)
                              where b.AuthorId == id
                              select new BookDetailDto
                              {
                                  Title = b.Title,
                                  Genre = b.Genre,
                                  PublishDate = b.PublishDate,
                                  Price = b.Price,
                                  Description = b.Description,
                                  Author = b.Author.Name
                              }).FirstOrDefaultAsync();

            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        /// <summary>
        /// Get books by genre.
        /// </summary>
        /// <example>
        /// GET: /api/books/{genre}
        /// </example>
        /// <param name="genre">The genre for which to retrieve all books.</param>
        /// <returns>
        /// A collection of <see cref="BookDto"/> matching the genre.
        /// </returns>
        [Route("{genre}")]
        public IQueryable<BookDto> GetBooksByGenre(string genre)
        {
            return db.Books.Include(b => b.Author)
                .Where(b => b.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase))
                .Select(AsBookDto);
        }

        /// <summary>
        /// Get books by author.
        /// </summary>
        /// <example>
        /// GET:  /api/authors/{id}/books
        /// </example>
        /// <remarks>
        /// The tilde (~) in the route template overrides the route prefix in the RoutePrefix attribute.
        /// </remarks>
        /// <param name="authorId">The unique id of the author for which to retireve all books.</param>
        /// <returns>
        /// A collection of <see cref="BookDto"/> writen by the author.
        /// </returns>
        [Route("~/api/authors/{authorId}/books")]
        public IQueryable<BookDto> GetBooksByAuthor(int authorId)
        {
            return db.Books.Include(b => b.Author)
                .Where(b => b.AuthorId == authorId)
                .Select(AsBookDto);
        }

        /// <summary>
        /// Get books by publication date.
        /// </summary>
        /// <example>
        /// GET: /api/books/date/{yyyy-mm-dd}
        /// </example>
        /// <remarks>
        /// The following route matches any DateTime such as "Thu, 01 May 2008" and "2000-12-16T00:00:00" therefore we must restrict the route using RegEx
        /// [Route("date/{pubdate:datetime}")]
        /// </remarks>
        /// <param name="pubdate">The publication date of the book(s) to retrieve.</param>
        /// <returns>
        /// A collection of <see cref="BookDto"/> matching the publication date.
        /// </returns>
        [Route("date/{pubdate:datetime:regex(\\d{4}-\\d{2}-\\d{2})}")]
        [Route("date/{*pubdate:datetime:regex(\\d{4}/\\d{2}/\\d{2})}")]
        public IQueryable<BookDto> GetBooks(DateTime pubdate)
        {
            return db.Books.Include(b => b.Author)
                .Where(b => DbFunctions.TruncateTime(b.PublishDate) == DbFunctions.TruncateTime(pubdate))
                .Select(AsBookDto);
        }

        #endregion

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        #endregion
    }
}
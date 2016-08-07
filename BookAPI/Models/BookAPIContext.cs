using System.Data.Entity;

namespace BookAPI.Models
{
    public class BookAPIContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public BookAPIContext() : base("name=BookAPIContext")
        {
        }

        public System.Data.Entity.DbSet<BookAPI.Models.Book> Books { get; set; }

        public System.Data.Entity.DbSet<BookAPI.Models.Author> Authors { get; set; }
    }
}

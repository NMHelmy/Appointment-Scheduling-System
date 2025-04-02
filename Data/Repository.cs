using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AppointmentScheduling.Data
{
    // Concrete implementation of IRepository using Entity Framework Core
    public class Repository : IRepository
    {
        private readonly AppDbContext _context;

        // Constructor accepts AppDbContext through dependency injection
        public Repository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Executes pending database operations asynchronously
        // Returns true if any changes were saved successfully
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        // Adds a new entity to the change tracker
        // Throws ArgumentNullException if entity is null
        public void AddEntity<T>(T entity) where T : class
        {
            _context.Set<T>().Add(entity ?? throw new ArgumentNullException(nameof(entity)));
        }

        // Marks an entity for deletion in the change tracker
        // Throws ArgumentNullException if entity is null
        public void RemoveEntity<T>(T entity) where T : class
        {
            _context.Set<T>().Remove(entity ?? throw new ArgumentNullException(nameof(entity)));
        }

        // Retrieves all entities of type T from the database
        // Uses async/await for non-blocking database access
        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class
        {
            return await _context.Set<T>().ToListAsync();
        }

        // Finds an entity by primary key without throwing exceptions
        // Returns null if no matching entity exists
        public async Task<T?> GetByIdAsync<T>(int id) where T : class
        {
            return await _context.Set<T>().FindAsync(id);
        }

        // Provides IQueryable for building optimized database queries
        // Caller can chain LINQ operations before execution
        public IQueryable<T> GetQueryable<T>() where T : class
        {
            return _context.Set<T>();
        }

        // Used for conflict detection (e.g., overlapping appointments).
        public async Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await _context.Set<T>().AnyAsync(predicate);
        }
    }
}
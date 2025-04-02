using System.Linq.Expressions;

namespace AppointmentScheduling.Data
{
    // Generic repository interface defining common database operations
    public interface IRepository
    {
        Task<bool> SaveChangesAsync(); // Saves all pending changes to the database asynchronously - Returns true if at least one row was affected
        void AddEntity<T>(T entity) where T : class; // Adds a new entity to the database (change is applied after SaveChanges)
        void RemoveEntity<T>(T entity) where T : class; // Marks an entity for removal (change is applied after SaveChanges)
        Task<IEnumerable<T>> GetAllAsync<T>() where T : class; // Retrieves all entities of type T asynchronously
        Task<T?> GetByIdAsync<T>(int id) where T : class; // Finds an entity by its primary key asynchronously - Returns null if not found (nullable return type)
        IQueryable<T> GetQueryable<T>() where T : class; // Provides queryable access to entities for building custom LINQ queries -Useful for adding Where, OrderBy
        Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate) where T : class; // To avoid booking overlapping appointments
    }
}
using ExaminationSystem.Data;
using ExaminationSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace ExaminationSystem.Repositories
{
    public class GeneralRepository<T> where T : BaseModel
    {
        private readonly Context _context;
        private readonly DbSet<T> _dbSet;

        public GeneralRepository(Context context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public bool Add(T entity)
        {
            _context.Add(entity);
            _context.SaveChanges();
            return true;
        }

        public IQueryable<T> GetAllAsync() =>
            _dbSet.Where(c => !c.IsDeleted);

        public IQueryable<T> Get(Expression<Func<T, bool>> predicate) =>
            _dbSet.Where(predicate);

        public async Task<T?> GetById(int id) =>
            await _dbSet.Where(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();

        public async Task<T?> GetByIdWithTracking(int id) =>
            await _dbSet.Where(c => c.Id == id && !c.IsDeleted).AsTracking().FirstOrDefaultAsync();

        public async Task Delete(int id)
        {
            var obj = await GetByIdWithTracking(id);
            if (obj == null)
                throw new Exception("Object not found");
            obj.IsDeleted = true;
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges();
        }

        public void UpdateInclude(T entity, params string[] modifiedProperties)
        {
            if (!_dbSet.Any(x => x.Id == entity.Id))
                return;

            var local = _dbSet.Local.FirstOrDefault(x => x.Id == entity.Id);
            EntityEntry entityEntry;

            if (local is null)
                entityEntry = _context.Entry(entity);
            else
                entityEntry = _context.ChangeTracker.Entries<T>().First(x => x.Entity.Id == entity.Id);

            foreach (var prop in entityEntry.Properties)
            {
                if (modifiedProperties.Contains(prop.Metadata.Name))
                {
                    prop.CurrentValue = entity.GetType().GetProperty(prop.Metadata.Name)?.GetValue(entity);
                    prop.IsModified = true;
                }
            }
            _context.SaveChanges();
        }
    }
}

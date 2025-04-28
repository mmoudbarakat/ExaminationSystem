using ExaminationSystem.Data;
using ExaminationSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace ExaminationSystem.Repositories
{
    public class GeneralRepository<T> where T : BaseModel
    {
        Context _context;
        DbSet<T> _dbSet;
        public GeneralRepository()
        {
            _context = new Context();
            _dbSet = _context.Set<T>();
        }

        

        [HttpPost]
        public bool Add(T entity)
        {
            var crs = _context.Add(entity);
            _context.SaveChanges();
            return true;

        }
        [HttpGet]
        public IQueryable<T> GetAllAsync()
        {
            return _dbSet.Where(c => !c.IsDeleted);
        }
        [HttpGet]
        //Filter using predicate in the form of an expression tree.
        public IQueryable<T> Get(Expression< Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        [HttpGet]
        public async Task<T> GetById(int id)
        {
            return await _dbSet
                .Where(c => c.Id == id && !c.IsDeleted)
                .FirstOrDefaultAsync();
        }

        [HttpGet]
        public async Task<T> GetByIdWithTracking(int id)
        {
            return await _dbSet
                .Where(c => c.Id == id && !c.IsDeleted)
                .AsTracking()
                .FirstOrDefaultAsync();
            //if u want to return a collection of Course
            //return course == null ? Enumerable.Empty<Course>() : new[] { course };
        }

        [HttpDelete]
        public async Task Delete(int id)
        {
            var obj = await GetByIdWithTracking(id);
            if (obj == null)
                throw new Exception("Object not found");
            obj.IsDeleted = true;
                
            _context.SaveChanges();
        }

        [HttpPut]
        
        public void Update(T entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges();
            
        }

        [HttpPut]
        public void UpdateInclude(T entity, params string[] modifiedProperties)
        {
            if (!_dbSet.Any(x => x.Id == entity.Id))
                return;

            var local = _dbSet.Local.FirstOrDefault(x => x.Id == entity.Id);
            EntityEntry entityEntry;

            if (local is null)
                entityEntry = _context.Entry(entity);
            else
                entityEntry = _context.ChangeTracker.Entries<T>().FirstOrDefault(x => x.Entity.Id == entity.Id);

            foreach (var prop in entityEntry.Properties)
            {
                if (modifiedProperties.Contains(prop.Metadata.Name))
                {
                    prop.CurrentValue = entity.GetType().GetProperty(prop.Metadata.Name).GetValue(entity); 
                    prop.IsModified = true;
                }

            }
            _context.SaveChanges();
        }



    }
}

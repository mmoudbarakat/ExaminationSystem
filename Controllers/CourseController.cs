using ExaminationSystem.Data;
using ExaminationSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design.Internal;

namespace ExaminationSystem.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CourseController : Controller
    {
        Context context;
        public CourseController()
        {
            context = new Context();
        }

        [HttpPost]
        public bool Add(Course course)
        {
            var crs =  context.Add(course);
            context.SaveChanges();
            return true;
             
        }

        

        [HttpGet]
        public async Task<IEnumerable<Course>> GetCourse(int id)
        {
            return await context.Courses
                .Where(c => c.Id == id)
                .ToListAsync();
        }

        [HttpDelete]
        public async Task<bool> Delete(int id)
        {
            var crs = await context.Courses
                .AsTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
            crs.Deleted = true;
            await context.SaveChangesAsync();

            return true;
        }

        [HttpPut]
        public async Task<bool> Update(int id, [FromBody] Course course)
        {
            var crs = await context.Courses
                .FirstOrDefaultAsync(c => c.Id == id);
            course.Name = crs.Name;
            course.Description = crs.Description;
            course.Hour = crs.Hour;
            await context.SaveChangesAsync();
            return true;    
        }

    }
}

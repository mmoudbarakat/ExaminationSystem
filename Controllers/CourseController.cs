using System.Security.Claims;
using ExaminationSystem.Data;
using ExaminationSystem.Dtos;
using ExaminationSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Instructor")]
    public class CourseController : ControllerBase
    {
        private readonly Context _context;

        public CourseController(Context context)
        {
            _context = context;
        }

        private int InstructorId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<ActionResult<List<CourseDto>>> GetMyCourses()
        {
            var courses = await _context.Courses
                .Where(c => c.InstructorId == InstructorId && !c.IsDeleted)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Hours = c.Hours,
                    InstructorId = c.InstructorId
                })
                .ToListAsync();

            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetById(int id)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == id && c.InstructorId == InstructorId && !c.IsDeleted);

            if (course == null) return NotFound();

            return Ok(new CourseDto
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                Hours = course.Hours,
                InstructorId = course.InstructorId
            });
        }

        [HttpPost]
        public async Task<ActionResult<CourseDto>> Create(CreateCourseDto dto)
        {
            var course = new Course
            {
                Name = dto.Name,
                Description = dto.Description,
                Hours = dto.Hours,
                InstructorId = InstructorId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = InstructorId
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = course.Id }, new CourseDto
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                Hours = course.Hours,
                InstructorId = course.InstructorId
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCourseDto dto)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == id && c.InstructorId == InstructorId && !c.IsDeleted);

            if (course == null) return NotFound();

            course.Name = dto.Name;
            course.Description = dto.Description;
            course.Hours = dto.Hours;
            course.UpdatedBy = InstructorId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == id && c.InstructorId == InstructorId && !c.IsDeleted);

            if (course == null) return NotFound();

            course.IsDeleted = true;
            course.DeletedBy = InstructorId;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{courseId}/enroll/{studentId}")]
        public async Task<IActionResult> EnrollStudent(int courseId, int studentId)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == courseId && c.InstructorId == InstructorId && !c.IsDeleted);

            if (course == null) return NotFound("Course not found.");

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Id == studentId && !s.IsDeleted);

            if (student == null) return NotFound("Student not found.");

            var exists = await _context.CourseStudents
                .AnyAsync(cs => cs.CourseId == courseId && cs.StudentId == studentId && !cs.IsDeleted);

            if (exists) return BadRequest("Student already enrolled.");

            _context.CourseStudents.Add(new CourseStudent
            {
                CourseId = courseId,
                StudentId = studentId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = InstructorId
            });

            await _context.SaveChangesAsync();
            return Ok(new { message = "Student enrolled successfully." });
        }

        [HttpGet("{courseId}/students")]
        public async Task<IActionResult> GetEnrolledStudents(int courseId)
        {
            var course = await _context.Courses
                .AnyAsync(c => c.Id == courseId && c.InstructorId == InstructorId && !c.IsDeleted);

            if (!course) return NotFound();

            var students = await _context.CourseStudents
                .Where(cs => cs.CourseId == courseId && !cs.IsDeleted)
                .Include(cs => cs.Student)
                .Select(cs => new { cs.StudentId, cs.Student!.Name, cs.Student.Email })
                .ToListAsync();

            return Ok(students);
        }
    }
}

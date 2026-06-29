using ExaminationSystem.Data;
using ExaminationSystem.Dtos;
using ExaminationSystem.Helpers;
using ExaminationSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ExaminationSystem.Services
{
    public class AuthService
    {
        private readonly Context _context;
        private readonly JwtSettings _jwtSettings;

        public AuthService(Context context, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<AuthResponseDto?> RegisterInstructorAsync(RegisterDto dto)
        {
            if (await _context.Instructors.AnyAsync(i => i.Email == dto.Email && !i.IsDeleted))
                return null;

            var instructor = new Instructor
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = PasswordHelper.Hash(dto.Password),
                CreatedAt = DateTime.UtcNow
            };

            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            return BuildResponse(instructor.Id, instructor.Name, instructor.Email, "Instructor");
        }

        public async Task<AuthResponseDto?> RegisterStudentAsync(RegisterDto dto)
        {
            if (await _context.Students.AnyAsync(s => s.Email == dto.Email && !s.IsDeleted))
                return null;

            var student = new Student
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = PasswordHelper.Hash(dto.Password),
                CreatedAt = DateTime.UtcNow
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return BuildResponse(student.Id, student.Name, student.Email, "Student");
        }

        public async Task<AuthResponseDto?> LoginInstructorAsync(LoginDto dto)
        {
            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(i => i.Email == dto.Email && !i.IsDeleted);

            if (instructor == null || !PasswordHelper.Verify(dto.Password, instructor.PasswordHash))
                return null;

            return BuildResponse(instructor.Id, instructor.Name, instructor.Email, "Instructor");
        }

        public async Task<AuthResponseDto?> LoginStudentAsync(LoginDto dto)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Email == dto.Email && !s.IsDeleted);

            if (student == null || !PasswordHelper.Verify(dto.Password, student.PasswordHash))
                return null;

            return BuildResponse(student.Id, student.Name, student.Email, "Student");
        }

        private AuthResponseDto BuildResponse(int id, string name, string email, string role) =>
            new()
            {
                Id = id,
                Name = name,
                Email = email,
                Role = role,
                Token = JwtHelper.GenerateToken(id, role, _jwtSettings)
            };
    }
}

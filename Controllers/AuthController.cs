using ExaminationSystem.Dtos;
using ExaminationSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("instructor/register")]
        public async Task<IActionResult> RegisterInstructor(RegisterDto dto)
        {
            var result = await _authService.RegisterInstructorAsync(dto);
            return result == null ? BadRequest("Email already registered.") : Ok(result);
        }

        [HttpPost("student/register")]
        public async Task<IActionResult> RegisterStudent(RegisterDto dto)
        {
            var result = await _authService.RegisterStudentAsync(dto);
            return result == null ? BadRequest("Email already registered.") : Ok(result);
        }

        [HttpPost("instructor/login")]
        public async Task<IActionResult> LoginInstructor(LoginDto dto)
        {
            var result = await _authService.LoginInstructorAsync(dto);
            return result == null ? Unauthorized("Invalid credentials.") : Ok(result);
        }

        [HttpPost("student/login")]
        public async Task<IActionResult> LoginStudent(LoginDto dto)
        {
            var result = await _authService.LoginStudentAsync(dto);
            return result == null ? Unauthorized("Invalid credentials.") : Ok(result);
        }
    }
}

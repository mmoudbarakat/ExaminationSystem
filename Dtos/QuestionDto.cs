using ExaminationSystem.Models.Enums;

namespace ExaminationSystem.Dtos
{
    public class RegisterDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }

    public class CourseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Hours { get; set; }
        public int InstructorId { get; set; }
    }

    public class CreateCourseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Hours { get; set; }
    }

    public class UpdateCourseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Hours { get; set; }
    }

    public class ChoiceDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }

    public class ChoiceForStudentDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    public class CreateChoiceDto
    {
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }

    public class QuestionDto
    {
        public int Id { get; set; }
        public string Header { get; set; } = string.Empty;
        public QuestionLevel QuestionLevel { get; set; }
        public int CourseId { get; set; }
        public List<ChoiceDto> Choices { get; set; } = new();
    }

    public class QuestionForStudentDto
    {
        public int Id { get; set; }
        public string Header { get; set; } = string.Empty;
        public QuestionLevel QuestionLevel { get; set; }
        public List<ChoiceForStudentDto> Choices { get; set; } = new();
    }

    public class CreateQuestionDto
    {
        public string Header { get; set; } = string.Empty;
        public QuestionLevel QuestionLevel { get; set; }
        public int CourseId { get; set; }
        public List<CreateChoiceDto> Choices { get; set; } = new();
    }

    public class UpdateQuestionDto
    {
        public string Header { get; set; } = string.Empty;
        public QuestionLevel QuestionLevel { get; set; }
    }

    public class ExamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ExamType ExamType { get; set; }
        public int CourseId { get; set; }
        public int QuestionCount { get; set; }
        public bool IsAutoAssign { get; set; }
        public double DurationMinutes { get; set; }
        public List<int> QuestionIds { get; set; } = new();
    }

    public class CreateExamDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ExamType ExamType { get; set; }
        public int CourseId { get; set; }
        public int QuestionCount { get; set; }
        public bool IsAutoAssign { get; set; }
        public double DurationMinutes { get; set; }
        public List<int>? QuestionIds { get; set; }
    }

    public class UpdateExamDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ExamType ExamType { get; set; }
        public int QuestionCount { get; set; }
        public bool IsAutoAssign { get; set; }
        public double DurationMinutes { get; set; }
        public List<int>? QuestionIds { get; set; }
    }

    public class SubmitAnswerDto
    {
        public int QuestionId { get; set; }
        public int ChoiceId { get; set; }
    }

    public class SubmitExamDto
    {
        public List<SubmitAnswerDto> Answers { get; set; } = new();
    }

    public class ExamResultDto
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
        public double Score { get; set; }
        public DateTime? SubmittedAt { get; set; }
    }

    public class StudentExamDto
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public ExamType ExamType { get; set; }
        public bool IsSubmitted { get; set; }
        public double? Score { get; set; }
    }
}

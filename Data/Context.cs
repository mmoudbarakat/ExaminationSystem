using ExaminationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Choice> Choices => Set<Choice>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<Exam> Exams => Set<Exam>();
        public DbSet<ExamQuestion> ExamQuestions => Set<ExamQuestion>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Instructor> Instructors => Set<Instructor>();
        public DbSet<ExamStudent> ExamStudents => Set<ExamStudent>();
        public DbSet<CourseStudent> CourseStudents => Set<CourseStudent>();
        public DbSet<Prerequesit> Prerequesits => Set<Prerequesit>();
        public DbSet<StudentAnswer> StudentAnswers => Set<StudentAnswer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(i => i.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseStudent>()
                .HasIndex(cs => new { cs.CourseId, cs.StudentId })
                .IsUnique();

            modelBuilder.Entity<CourseStudent>()
                .HasOne(cs => cs.Course)
                .WithMany(c => c.CourseStudents)
                .HasForeignKey(cs => cs.CourseId);

            modelBuilder.Entity<CourseStudent>()
                .HasOne(cs => cs.Student)
                .WithMany(s => s.CourseStudents)
                .HasForeignKey(cs => cs.StudentId);

            modelBuilder.Entity<Exam>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Exams)
                .HasForeignKey(e => e.CourseId);

            modelBuilder.Entity<Exam>()
                .HasOne(e => e.Instructor)
                .WithMany(i => i.Exams)
                .HasForeignKey(e => e.InstructorId);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Course)
                .WithMany(c => c.Questions)
                .HasForeignKey(q => q.CourseId);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Instructor)
                .WithMany(i => i.Questions)
                .HasForeignKey(q => q.InstructorId);

            modelBuilder.Entity<Choice>()
                .HasOne(c => c.Question)
                .WithMany(q => q.Choices)
                .HasForeignKey(c => c.QuestionId);

            modelBuilder.Entity<ExamQuestion>()
                .HasIndex(eq => new { eq.ExamId, eq.QuestionId })
                .IsUnique();

            modelBuilder.Entity<ExamQuestion>()
                .HasOne(eq => eq.Exam)
                .WithMany(e => e.ExamQuestions)
                .HasForeignKey(eq => eq.ExamId);

            modelBuilder.Entity<ExamQuestion>()
                .HasOne(eq => eq.Question)
                .WithMany(q => q.ExamQuestions)
                .HasForeignKey(eq => eq.QuestionId);

            modelBuilder.Entity<ExamStudent>()
                .HasIndex(es => new { es.ExamId, es.StudentId })
                .IsUnique();

            modelBuilder.Entity<ExamStudent>()
                .HasOne(es => es.Exam)
                .WithMany(e => e.ExamStudents)
                .HasForeignKey(es => es.ExamId);

            modelBuilder.Entity<ExamStudent>()
                .HasOne(es => es.Student)
                .WithMany(s => s.ExamStudents)
                .HasForeignKey(es => es.StudentId);

            modelBuilder.Entity<Instructor>()
                .HasIndex(i => i.Email)
                .IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.Email)
                .IsUnique();

            modelBuilder.Entity<StudentAnswer>()
                .HasIndex(sa => new { sa.StudentId, sa.ExamId, sa.QuestionId })
                .IsUnique();

            modelBuilder.Entity<Prerequesit>()
                .HasOne(p => p.MainCourse)
                .WithMany(c => c.Prerequesits)
                .HasForeignKey(p => p.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prerequesit>()
                .HasOne(p => p.PrerequesitCourse)
                .WithMany(c => c.MainCourse)
                .HasForeignKey(p => p.PrerequesitId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

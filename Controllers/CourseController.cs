using ExaminationSystem.Data;
using ExaminationSystem.Models;
using ExaminationSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design.Internal;
using PredicateExtensions;
using System.Linq.Expressions;

namespace ExaminationSystem.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CourseController : Controller
    {
        GeneralRepository<Course> _courseRepository;
        public CourseController()
        {
            _courseRepository = new GeneralRepository<Course>();
        }

        [HttpPost]
        public bool Add(Course course)
        {
            return _courseRepository.Add(course);
        }
        [HttpGet]
        public  IEnumerable<Course> GetAllCourses()
        {
            return  _courseRepository.GetAll();
            //Filteration condition
            //return  _courseRepository.Get(x => x.IsNew == true);
            
        }
        //using predicate Builder
        public IEnumerable<Course> Get(int? crsId, string name, int? hours)
        {
            var predicate = BuildPredicate(crsId, name, hours);
            return _courseRepository.Get(predicate).ToList();

        }

        [HttpGet]
        public  Task<Course> GetCourseById(int id)
        {
            return  _courseRepository.GetById(id);
        }

        [HttpGet]
        public async Task<Course> GetCourseByIdWithTracking(int id)
        {
            return await _courseRepository.GetByIdWithTracking(id);
        }

        [HttpDelete]
        public Task Delete(int id)
        {
            return  _courseRepository.Delete(id);
        }

        [HttpPut]
        public void Update([FromBody] Course course)
        {
             _courseRepository.Update(course);
             
        }

        [HttpPut]
        public bool UpdateInclude(Course course)
        {
            _courseRepository.UpdateInclude(course, nameof(Course.Name), nameof(Course.CreatedAt));
            return true;
        }


        private Expression<Func<Course, bool>> BuildPredicate(int? crsId, string name, int? hours)
        {
            var predicate = PredicateExtensions.PredicateExtensions.Begin<Course>(true);
            if (crsId != null)
                predicate = predicate.And(x => x.Id == crsId.Value);
            if (!string.IsNullOrEmpty(name))
                predicate = predicate.And(x => x.Name.Contains(name));
            if (hours != null)
                predicate = predicate.And(x => x.Hours == hours.Value);
            return predicate;


        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using nova_study_api.Models;

namespace nova_study_api.Controllers
{
    [RoutePrefix("api/instructor")]
    public class StudentController : ApiController
    {
        [HttpGet]
        [Route("Students")]
        public IHttpActionResult GetStudents()
        {
            using (var db = new NovaStudyModel())
            {
                return Ok(db.Students.ToList());
            }
        }

        [HttpGet]
        [Route("Students/{studentId}")]
        public IHttpActionResult GetStudent(int studentId)
        {
            using (var db = new NovaStudyModel())
            {
                var student = db.Students.Include("SightWords").SingleOrDefault(s => s.StudentId == studentId);

                if (student == null)
                {
                    return NotFound();
                }

                return Ok(student);
            }
        }

        [HttpPost]
        [Route("Students")]
        public IHttpActionResult CreateStudent([FromBody] Student student)
        {
            using (var db = new NovaStudyModel())
            {
                db.Students.Add(student);
                db.SaveChanges();
            }

            return Ok(student);
        }

        [HttpDelete]
        [Route("Students/{studentId}")]
        public IHttpActionResult DeleteStudent(int studentId)
        {
            using (var db = new NovaStudyModel())
            {
                var student = db.Students.SingleOrDefault(s => s.StudentId == studentId);
                if (student == null)
                {
                    return NotFound();
                }

                db.Students.Remove(student);
                db.SaveChanges();

                return Ok();
            }
        }

    }
}

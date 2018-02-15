using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using nova_study_api.Models;

namespace nova_study_api.Controllers.Instructor
{
    [RoutePrefix("api/instructor")]
    public class InstructorSightWordController : ApiController
    {
        [HttpPost]
        [Route("Students/{studentId}/SightWords")]
        public IHttpActionResult CreateSightWords(int studentId, [FromBody] List<SightWord> sightWords)
        {
            using (var db = new NovaStudyModel())
            {
                var student = db.Students.SingleOrDefault(s => s.StudentId == studentId);

                if (student == null)
                {
                    return NotFound();
                }

                student.SightWords.AddRange(sightWords);
                db.SaveChanges();

                return Ok(sightWords);
            }
        }

        [HttpGet]
        [Route("Students/{studentId}/SightWords")]
        public IHttpActionResult GetSightWords(int studentId)
        {
            using (var db = new NovaStudyModel())
            {
                var student = db.Students.Include("SightWords").SingleOrDefault(s => s.StudentId == studentId);

                if(student == null)
                {
                    return NotFound();
                }

                return Ok(student.SightWords);
            }
        }

        [HttpPut]
        [Route("Students/{studentId}/SightWords")]
        public IHttpActionResult UpdateSightWords(int studentId, [FromBody] List<SightWord> sightWords)
        {
            using(var db = new NovaStudyModel())
            {
                var student = db.Students.Include("SightWords").SingleOrDefault(s => s.StudentId == studentId);

                if(student == null)
                {
                    return NotFound();
                }

                foreach(var sightWord in sightWords)
                {
                    var sightWordFromDB = student.SightWords.SingleOrDefault(sw => sw.SightWordId == sightWord.SightWordId);

                    sightWordFromDB.AnsweredCorrectlyCount = sightWord.AnsweredCorrectlyCount;
                    sightWordFromDB.AnsweredIncorrectlyCount = sightWord.AnsweredIncorrectlyCount;
                }

                db.SaveChanges();
            }

            return Ok(sightWords);
        }
    }
}

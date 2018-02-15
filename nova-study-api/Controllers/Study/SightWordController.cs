using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using nova_study_api.Helpers;
using nova_study_api.Models;

namespace nova_study_api.Controllers.Study
{
    [RoutePrefix("api/Students")]
    public class SightWordController : ApiController
    {
        [HttpGet]
        [Route("{studentId}/SightWords")]
        public IHttpActionResult GetSightWords(int studentId)
        {
            using (var db = new NovaStudyModel())
            {
                var student = db.Students.Include("SightWords").SingleOrDefault(s => s.StudentId == studentId);
                if (student == null)
                {
                    return NotFound();
                }

                return Ok(student.SightWords);
            }
        }

        [HttpPut]
        [Route("{studentId}/SightWords")]
        public IHttpActionResult UpdateSightWord(int studentId, [FromBody]SightWord sightWord)
        {
            using (var db = new NovaStudyModel())
            {
                var student = db.Students.Include("SightWords").SingleOrDefault(s => s.StudentId == studentId);
                var sightWordFromDB = student.SightWords.SingleOrDefault(sw => sw.SightWordId == sightWord.SightWordId) ?? throw new Exception("Invalid Sightword");

                sightWordFromDB.AnsweredCorrectlyCount = sightWord.AnsweredCorrectlyCount;
                sightWordFromDB.AnsweredIncorrectlyCount = sightWord.AnsweredIncorrectlyCount;

                db.SaveChanges();

                return Ok(sightWord);
            }
        }

        //[HttpGet]
        //public IHttpActionResult GetSightWord(int studentId)
        //{

        //  return Ok();
        //}



    }    
}

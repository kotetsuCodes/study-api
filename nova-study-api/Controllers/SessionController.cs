using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using nova_study_api.Models;

namespace nova_study_api.Controllers
{
    [RoutePrefix("api/session")]
    public class SessionController : ApiController
    {
        public IHttpActionResult Login([FromBody] LoginRequestBody login)
        {
            using(var db = new NovaStudyModel())
            {
                
            }

        }

        public class LoginRequestBody
        {
            [JsonProperty("username")]
            public string Username { get; set; }
            [JsonProperty("password")]
            public string Password { get; set; }
        }
    }
}

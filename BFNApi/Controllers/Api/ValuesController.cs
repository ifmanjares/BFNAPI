using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BFNApi.Controllers.Api
{
    public class ValuesController : ApiController
    {
        [HttpGet]
        public string Get()
        {
            return "success";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using TestWebApi.Models;

namespace TestWebApi.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet]
        [Route("api/filter")]
        public IHttpActionResult Filter([FromUri]GeoPoint point = null, [ModelBinder] IList<SizeType> sizes = null, [ModelBinder] string[] types = null)
        {
            var result = 
                string.Format("Location is (Longitude:{0}, Latitude:{1} sizes: {2}, types: {3}", point != null ? point.Longitude : 0.0, 
                point != null ? point.Latitude : 0.0, 
                sizes == null ? string.Empty : string.Join(",", sizes),
                types == null ? string.Empty : string.Join(",", types));            
            return Ok<string>(result);
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}

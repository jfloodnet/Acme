using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Nest;

namespace AreaOfInterest.Controllers
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


    public class AreaOfInterestController : ApiController
    {
        public AreaOfInterest[] Get()
        {
            var node = new Uri("http://localhost:9200");
            var settings = new ConnectionSettings(
                node,
                defaultIndex: "areas-of-interest"
            );

            var client = new ElasticClient(settings);

            return client.Search<AreaOfInterest>(s =>
                s.Query(q => q.GeoShapePoint(p => p.OnField(x => x.PolygonShape).Coordinates(new[] { 100.9, 0.1 }))))
                .Documents.ToArray();

        }

        public HttpResponseMessage Post(string id)
        {
            var node = new Uri("http://localhost:9200");
            var settings = new ConnectionSettings(
                node,
                defaultIndex: "areas-of-interest"
            );

            var client = new ElasticClient(settings);

            AreaOfInterest a = new AreaOfInterest
            {

                User = id,
                PolygonShape =

                new PolygonGeoShape
                {
                    Coordinates = new[] { 
						    new[] { new[] { 100.0, 0.0 }, new[] { 101.0, 0.0 }, new[] { 101.0, 1.0 }, new[] { 100.0, 1.0 }, new [] { 100.0, 0.0 } },
						    new[] { new[] { 100.2, 0.2}, new[] { 100.8, 0.2 }, new[] { 100.8, 0.8}, new[] { 100.2, 0.8 }, new [] { 100.2, 0.2} }
				    }
                }
            };


            client.Index(a);

            return Request.CreateResponse(HttpStatusCode.Accepted);
        }
    }

    public class AreaOfInterest
    {
        public string User { get; set; }
        public PolygonGeoShape PolygonShape { get; set; }
    }
}

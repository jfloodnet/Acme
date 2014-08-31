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
        public string[] Get()
        {
            var node = new Uri("http://localhost:9200");
            var settings = new ConnectionSettings(
                node,
                defaultIndex: "areas-of-interest"
            );

            var client = new ElasticClient(settings);
            var results = client.Search<Interest>(d => d);


            var result2 = client.Search<Interest>(
                  s => s.QueryRaw(

                      @"
{ 
    'filtered': { 
        'query': { 
            'match_all': {} 
        },
        'filter': {
            'geo_shape': {
                'polygonShape': {
                    'shape': {
                        'type' : 'polygon',
                        'coordinates' : [
                            [
                                [51.5014232474337,-0.0896930694580078],
                                [51.5011561006944,-0.0905513763427734],
                                [51.5016369636976,-0.0901222229003906],
                                [51.5014232474337,-0.0896930694580078]
                            ]
                        ]
                    }
                }
            }
        }
    }
}
".Replace('\'','"')));

            return results.Documents.Select(z => z.Id).ToArray();

        }

        public HttpResponseMessage Post(string id, [FromBody]LatLong[][] latLngs)
        {
            var node = new Uri("http://localhost:9200");
            var settings = new ConnectionSettings(
                node,
                defaultIndex: "areas-of-interest"
            );

            var client = new ElasticClient(settings);

            double[][][] coordinates = latLngs.Select(
                latlng => 
                  
                    latlng.Select( x => new[] { x.Lat, x.Lng }).ToArray()).ToArray();



            Interest a = new Interest
            {

                Id = id,
                Area =

                new PolygonGeoShape
                {
                    Coordinates =  coordinates 
                }
            };
            
            client.Index(a, 
                 i => i
                .Id(id)
                .Refresh()
                .Ttl("1m")
                );

            return Request.CreateResponse(HttpStatusCode.Accepted);
        }
    }

    public class Interest
    {
        public string Id { get; set; }
        public PolygonGeoShape Area { get; set; }
    }

    public class LatLong
    {
        public double Lat { get; set; }
        public double Lng { get; set; }

        public override string ToString()
        {
            return string.Format("[{0},{1}]",Lat, Lng);
        }
    }
}

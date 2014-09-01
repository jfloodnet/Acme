using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Nest;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using Newtonsoft.Json;
using System.Web.Http.ValueProviders;
using System.Web.Http.ModelBinding;

namespace AreaOfInterest.Controllers
{
    public class AreaOfInterestController : ApiController
    {
        private readonly ElasticClient client;

        public AreaOfInterestController()
        {
            var node = new Uri("http://localhost:9200");
            var settings = new ConnectionSettings(
                node,
                defaultIndex: "areas-of-interest"
            ).SetDefaultIndex("areas-of-interest");

            client = new ElasticClient(settings);
        }
        public string[] Get([ModelBinder(typeof(PolygonModelBinder))]LatLong[][] latLngs)
        {
            double[][][] coordinates = latLngs.Select(
                latlng => latlng.Select(x => new[] { x.Lat, x.Lng }).ToArray()).ToArray();

            var result1 = client.Search<Interest>(s => s
          	.From(0)
				.Size(10)
				.Query(q => q
					.GeoShapePolygon(qs => qs
						.OnField(p => p.Area)
						.Coordinates(coordinates)
					)
				)
            );
            return result1.Documents.Select(x => x.Id).ToArray();
        }

        public async Task<HttpResponseMessage> Post(string id, [FromBody]LatLong[][] latLngs)
        {
            double[][][] coordinates = latLngs.Select(
                latlng => latlng.Select( x => new[] { x.Lat, x.Lng }).ToArray()).ToArray();

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

    public class PolygonModelBinder : 
        System.Web.Http.ModelBinding.IModelBinder
    {

        public bool BindModel(HttpActionContext actionContext, System.Web.Http.ModelBinding.ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(LatLong[][]))
            {
                return false;
            }

            ValueProviderResult val = bindingContext.ValueProvider.GetValue(
                bindingContext.ModelName);
            if (val == null)
            {
                return false;
            }

            string key = val.RawValue as string;
            if (key == null)
            {
                bindingContext.ModelState.AddModelError(
                    bindingContext.ModelName, "Wrong value type");
                return false;
            }

            var latLong = JsonConvert.DeserializeObject<LatLong[][]>(val.RawValue.ToString(), new JsonSerializerSettings());
            if(latLong !=null){
                bindingContext.Model = latLong;
                return true;
            }

            bindingContext.ModelState.AddModelError(
                bindingContext.ModelName, "Cannot convert value to Location");
            return false;
        }
    }
}

using GalacticTourAgency.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace GalacticTourAgency.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GalacticToursController : ControllerBase
    {
        private static List<GalacticTour> _galacticTours = new()
        {
            new GalacticTour { Id = 1, PlanetName = "Mars", Duration = "2 Ay", Price= 500000 },
            new GalacticTour { Id = 2, PlanetName = "Moon", Duration = "1 Ay", Price= 4000},
        };

        [HttpGet]
        public IEnumerable<GalacticTour> GetAll()
        {
            return _galacticTours;
        }




        // Geçersiz idleri engelliyoruz. -5 id mantıksız olacağı için
        // actionresult belirlenen veri türünün döndürülmesini sağlar, net bir şekilde hangi metodun döndüğünü belirtir
        // Iactionresultsa belirli tür belirtmeksizin yanıt döndürmeye yarar.
        // Eğer veri döndürülüyorsa CRUD işlemi varsa action result tercih edilmelidir. Türü belirlenir derleyicinin tür kontrolünü güçlendirir.
        [HttpGet("{id:int:min(1)}")]
        public ActionResult<GalacticTour> GetTour([FromRoute]int id, [FromHeader(Name ="X-Planet")] string planet)
        {
            var tour = _galacticTours.FirstOrDefault(x => x.Id == id);
            if (tour is null)
            {
                return NotFound($"Tur id {id} bulunamadı");
            }
            return Ok(tour);    
        }





        //Gezegen adlarına göre sorgu yapılır
        // bir planet get edilsin gezegen ismi alfabetik sıraya göre gelsin.
        [HttpGet("planet/{planetName:alpha}")]
        public ActionResult<IEnumerable<GalacticTour>> GetTourByPlanet(string planetName)
        {
            //Equals ile iki planetname karşılaştırılır.
            // StringComparison.OrdinalIgnoreCase büyük/küçük harf duyarsız olmasını sağlar. Yani "Mars" ile "mars" aynı olarak kabul edilir.
            var planetTours = _galacticTours.Where(x => x.PlanetName.Equals(planetName,StringComparison.OrdinalIgnoreCase));
            if (!planetTours.Any())
            {
                return NotFound($"{planetName} için tur bulunamadı..");
            }
           return Ok(planetTours);
        }




        // Fiyat aralığındakileri querystring ile getirirken böyle bir metot tanımladık.
        // Parametreleri FromQuery alacaklardır.
        // Galactiktur koleksiyonu döneceği için
        [HttpGet("prize-range")]
        //api/galactitours/price-range?minPrice=20104-&maxPrice=202342
        public ActionResult<IEnumerable<GalacticTour>> GetToursByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
            // LINQ sorgusunda ise verilen değer minprice ile maxprice aralığındakileri seçmeye yarıyor.
            var filteredTours = _galacticTours.Where(x => x.Price >= minPrice && x.Price <= maxPrice);
            return Ok(filteredTours);
        }

        [HttpPost]
        public ActionResult<GalacticTour> Create([FromBody] GalacticTour tour)
        {
            // Ekleme işlemi yapılıyor.
            var id = _galacticTours.Max(x => x.Id) + 1;
            tour.Id = id;
            _galacticTours.Add(tour);
            // Ekleme işlemi bittikten sonra geriye kullanıcı bilgileri döndürüyoruz.
            // 1-bunun detayına gitmek istersek GetTour'a gidecek
            // 2-Nasıl bir parametreye gideceksin.
            // 3-Oluşan nesneyi geri döndürür
            return CreatedAtAction(nameof(GetTour), new { id = tour.Id }, tour);

        }

        [HttpPost("create-package")]
        public ActionResult<GalacticTour> CreateTourPackage([FromBody]GalacticTourPackage tourPackage)
        {
            // Ekleme işlemi yaptık
            var tour = new GalacticTour()
            {
                Id = _galacticTours.Max(x => x.Id) + 1,
                PlanetName = tourPackage.Destination,
                Duration = $"{tourPackage.DurationInDays} Gün",
                Price = tourPackage.BasePrice * tourPackage.DurationInDays
            };
            _galacticTours.Add(tour);

            return CreatedAtAction(nameof(GetTour), new { id = tour.Id }, tour);
        }
        [HttpPut("update/{id:int:min(1)}/{newPlanetName}")]
        public IActionResult UpdateTourPlanet(int id, string newPlanetName)
        {
            var tour = _galacticTours.FirstOrDefault(x => x.Id == id);
            if (tour is not null)
            {
                return NotFound($"Bu id {id} bulunamadı");
            }
            tour.PlanetName = newPlanetName;
            return NoContent();
        }


        // Tek bir metotla farklı işler yapılabilmektedir. hem idsine göre hmede tourNameine göre silmektedir.
        [HttpDelete("{id:int:min(1)}")]
        [HttpDelete("cancel/{tourName}")]
        public IActionResult CancelTour (int? id , string tourName)
        {
            GalacticTour tourToRemove;

            if (id.HasValue)
            {
                tourToRemove = _galacticTours.FirstOrDefault(x=> x.Id == id);
            }
            else
            {
                tourToRemove = _galacticTours.FirstOrDefault(x => x.PlanetName.Equals(tourName, StringComparison.OrdinalIgnoreCase));
            }

            if (tourToRemove is null)
            {
                return NotFound("Belirtilen tur bulunamadı.");
            }
            _galacticTours.Remove(tourToRemove);
            return NoContent();
        }
        // turun zamanını güncelleyen bir patch metodu
        [HttpPatch("reschedule/{id:int:min(1)}/{newDate:datetime}/")] 
        // jsonpatchdocument bize esnek bir güncelleme imkanı sunuyor.
        public IActionResult RescheduleTour (int id, DateTime newDate, [FromBody] JsonPatchDocument<GalacticTour> patchDocument)
        {
            var tour = _galacticTours.FirstOrDefault(x => x.Id == id);
            if(tour is null)
            {
                return NotFound($"Tur id {id} bulunamadı");
            }
            tour.DepartureDate = newDate;

            patchDocument.ApplyTo(tour);
            return NoContent();
        }




        [HttpPost("complex-search")]
        // POST /api/galactictours/complex-search?name=test&minPrice=400
        //Header : X-Planet : Mars
        // Body :{ "DepartureDate":243-243-01, "Duration": "2 Ay" }
        public ActionResult<IEnumerable<GalacticTour>> ComplexSearch ([FromQuery] string name, [FromQuery] decimal? minPrice, [FromHeader(Name = "X-Planet")] string planet, [FromBody] SearchCriteria searchCriteria )
        {
            //if (string.IsNullOrEmpty(name))

            //    if (minPrice.HasValue)

            return Ok(Enumerable.Empty<GalacticTour>());
        } 

    }
}

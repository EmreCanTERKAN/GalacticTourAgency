using GalacticTourAgency.ModelBindings;
using GalacticTourAgency.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GalacticTourAgency.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GalacticProductsController : ControllerBase
    {

        private static List<GalacticProduct> _galacticProducts = new()
        {
            new GalacticProduct {Id = 1, GalacticRating = 5, ManifacturingDate = DateTime.Now.AddYears(-1), Name = "Product 1",Planet="Mars", Price = 4}
       

        };

        [HttpGet("{id:int:min(1)}")]
        public ActionResult<GalacticProduct> Get (int id )
        {
            var product = _galacticProducts.FirstOrDefault(x => x.Id == id);
            if (product is null)
            {
                return NotFound();
            }

            return Ok(product);

        }


        [HttpPost]
        public ActionResult<GalacticProduct> Post([FromBody] GalacticProduct product)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var maxId = _galacticProducts.Max(x => x.Id);
            product.Id = maxId + 1;

            _galacticProducts.Add(product);
            return CreatedAtAction(nameof(Get), new { id = product.Id }, product);

        }

        // yukarıdaki location ile aşağıdaki location aynı olması gerek
        [HttpGet("products-at-location/{location}")]
        public ActionResult<IEnumerable<GalacticProduct>> GetProductsAtLocation([ModelBinder(BinderType = typeof(GalacticCordinateBinder))]GalacticCordinate location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(_galacticProducts);
        }
    }
}

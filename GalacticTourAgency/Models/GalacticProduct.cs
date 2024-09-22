using GalacticTourAgency.Attributes;
using System.ComponentModel.DataAnnotations;

namespace GalacticTourAgency.Models
{
    public class GalacticProduct
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adı gereklidir.")]
        [StringLength(100,MinimumLength =5,ErrorMessage ="5 ile 100 Karakter olmalıdır.")]
        public string Name { get; set; }
        [Range(0.01,1000,ErrorMessage ="Fiyat 0.01 ile 1000 arasında olmalıdır.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage ="Gezegen Adı boş geçilemez")]
        [RegularExpression("^(Merkür|Venüs|Mars)$",ErrorMessage ="Geçerli bir gezegen adı değildir..")] //Merkür?
        public string Planet { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Üretim Tarihi")]
        public DateTime ManifacturingDate { get; set; }

        [Range(1,100)]
        [Display(Name ="Gezegen Ratingi")]
        public int GalacticRating { get; set; }

        // tasarladığımız attributeları buraya ekledik..
        [GalacticElementComposition(minElements:2,maxElements:5)]
        public string Composition { get; set; }


        public GalacticCordinate Cordinate { get; set; }
    }
}

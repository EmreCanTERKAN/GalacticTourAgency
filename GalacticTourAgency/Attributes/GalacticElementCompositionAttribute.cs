using System.ComponentModel.DataAnnotations;

namespace GalacticTourAgency.Attributes
{
    //Custom bir attribute tanımlanacaksa validationattributedan miras alan bir class olması gerekmektedir.
    // bu validationattrubute classını kontrol etmek için bir metot kullanıyor.
    public class GalacticElementCompositionAttribute : ValidationAttribute
    {
        // Valid elementler haricindekileri kabul etmeyeyim diye bir liste tanımlayalım.
        private readonly string[] _validElements = new[]
        {
            "Hidrojen", "Karbon", "Oksijen","Silicon","Helyum","Neon"
        };

        // atama sadece burada attribute üzerinden olacağı için 
        public int MinElements { get;}
        public int MaxElements { get;}


        // constructor ile buna bir değer ataması yapıyoruz.
        public GalacticElementCompositionAttribute(int minElements = 1, int maxElements = 5)
        {
            MinElements = minElements;
            MaxElements = maxElements;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {

            // value değeri string mi string ise composition değerine ata
            if (value is string composition)
            {

                var elements = composition.Split(',').Select(x => x.Trim()).ToList();


                // element min elementten küçük bide max elemetten büyük mü şartı
                if (elements.Count < MinElements || elements.Count > MaxElements)
                {
                    return new ValidationResult($"Bileşen içeriğin {MinElements} ile {MaxElements} arasında olmalıdır.");
                }

                // elements koleksiyonundaki öğeleri _validElements koleksiyonundaki öğelerle karşılaştırır ve _validElements içinde bulunmayan öğeleri döndürür.
                var invalidElements = elements.Except(_validElements,StringComparer.OrdinalIgnoreCase).ToList();
                if (invalidElements.Any())
                {
                    return new ValidationResult($"Geçersiz Element saptandı. : {string.Join(",", invalidElements)}. Geçerli elementler : {string.Join(",", _validElements)}");
                }
            }
            else
            {
                return new ValidationResult("Geçersiz value, virgüüler ile ayrılmış string değer giriniz.");
            }

            return ValidationResult.Success;
        }


    }
}

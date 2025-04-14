using Models.Entities;
using Models.Enums;

namespace MainOrderly.WebApp.ViewModels
{
    public class AllergenViewModel
    {
        public int Id { get; set; }
        public AllergenName Name { get; set; }

        public static AllergenViewModel ConvertToViewModel(Allergen allergen)
        {
            return new AllergenViewModel
            {
                Id = allergen.Id,
                Name = allergen.Name
            };
        }
    }
}




using System.ComponentModel.DataAnnotations;

namespace Models.Enums
{
    public enum Category
    {
        [Display(Name = "Starters")]
        Starters,

        [Display(Name = "Main Course")]
        Main_Course,

        [Display(Name = "Side Dishes")]
        Side_Dishes,

        [Display(Name = "Desserts")]
        Desserts,

        [Display(Name = "Drinks")]
        Drinks
    }

}


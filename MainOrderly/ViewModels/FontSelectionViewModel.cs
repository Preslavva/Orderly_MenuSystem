using Microsoft.AspNetCore.Mvc.Rendering;

namespace MainOrderly.WebApp.ViewModels
{
    public class FontSelectionViewModel
    {
        public List<SelectListItem> FontFamilies { get; set; }
        public List<SelectListItem> FontVariants { get; set; }
    }
}

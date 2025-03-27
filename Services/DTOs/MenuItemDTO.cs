using Models.Entities;
using Models.Enums;
using Services.DTOs;

public class MenuItemDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public string Picture { get; set; }
    public int Quantity { get; set; }
    public Continent Continent { get; set; }
    public List<NutritionDTO> Nutritions { get; set; }

    // Default constructor
    public MenuItemDTO()
    {
        Nutritions = new List<NutritionDTO>();
    }

    public MenuItemDTO(MenuItem menuItem)
    {
        Id = menuItem.Id;
        Name = menuItem.Name;
        Description = menuItem.Description;
        Price = menuItem.Price;
        IsAvailable = menuItem.IsAvailable;
        Picture = menuItem.Picture;
        Quantity = menuItem.Quantity;
        Continent = menuItem.Continent;
        Nutritions = menuItem.Nutritions.Select(n => new NutritionDTO(n)).ToList();
    }

  
    public static MenuItemDTO ConvertToDTO(MenuItem menuItem)
    {
        return new MenuItemDTO(menuItem);
    }
}
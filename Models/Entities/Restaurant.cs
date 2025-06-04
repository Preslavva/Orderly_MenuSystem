namespace Models.Entities
    {
        public class Restaurant
        {
            public int Id { get; }

            public string Name { get; }

            public string Description { get; }

            public byte[] Logo { get; }

            public string Email { get; }

            public string PhoneNumber { get; }

            public string Address { get;  }

             public bool IsActive { get; }
             public string Font { get; }
             public string ColorButtons { get; }
             public string ColorDefault { get; }
             public string ColorBackground { get; }
            public string KVK { get; set; }
            public bool IncludePayment { get; set; }
            public bool IncludeAntiAbuse { get; set; }

        public List<MenuItem> MenuItems { get; }
            public List<Table> Tables { get; }
            public List<Order> Orders { get; }

        public Restaurant(int id, string name, string description, Byte[] logo, string email, string phoneNumber, string address)
        {
            Id = id;
            Name = name;
            Description = description;
            Logo = logo;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            MenuItems = new List<MenuItem>();
            Tables = new List<Table>();
            Orders = new List<Order>();
        }


        public Restaurant(string name, string description, Byte[] logo, string email, string phoneNumber, string address)
        {
            Name = name;
            Description = description;
            Logo = logo;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            MenuItems = new List<MenuItem>();
            Tables = new List<Table>();
            Orders = new List<Order>();
        }

        public Restaurant(
       int id,
       string name,
       string email,
       string phoneNumber,
       string address,
       string description,
       byte[] logo,
       string font,
       string colorButtons,
       string colorDefault,
       string colorBackground,
       bool isActive,
       string kvk,
        bool includePayment,
        bool includeAntiAbuse)
        {
            Id = id;
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            Description = description;
            Logo = logo;
            Font = font;
            ColorButtons = colorButtons;
            ColorDefault = colorDefault;
            ColorBackground = colorBackground;
            KVK = kvk;
            IsActive = isActive;
            IncludePayment = includePayment;
            IncludeAntiAbuse = includeAntiAbuse;
        }

        public Restaurant(
  int id,
  string name,
  string email,
  string phoneNumber,
  string address,
  string description,
  byte[] logo,
  string font,
  string colorButtons,
  string colorDefault,
  string colorBackground,
  string kvk,
  bool includePayment,
  bool includeAntiAbuse)
        {
            Id = id;
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            Description = description;
            Logo = logo;
            Font = font;
            ColorButtons = colorButtons;
            ColorDefault = colorDefault;
            ColorBackground = colorBackground;
            KVK = kvk;
            IncludePayment = includePayment;
            IncludeAntiAbuse = includeAntiAbuse;
        }

    }


}






























using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            public List<MenuItem> MenuItems { get; }
            public List<Table> Tables { get; }
            public List<Order> Orders { get; }

        public Restaurant(int id, string name, string description, Byte[] logo, string colorTheme, string email, string phoneNumber, string address)
        {
            Id = id;
            Name = name;
            Description = description;
            Logo = logo;
            ColorTheme = colorTheme;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            MenuItems = new List<MenuItem>();
            Tables = new List<Table>();
            Orders = new List<Order>();
        }


        public Restaurant(string name, string description, Byte[] logo, string colorTheme, string email, string phoneNumber, string address)
        {
            Name = name;
            Description = description;
            Logo = logo;
            ColorTheme = colorTheme;
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
       string colorBackground)
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
        }

    }


}






























using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Enums;

namespace Models.Entities
{
    public class Allergen
    {
        public int Id { get; }
        public AllergenName Name { get; }

        public Allergen(int id, AllergenName name)
        {
            this.Id = id;
            this.Name = name;
        }

        public Allergen(AllergenName name)
        {
            this.Name = name;
        }
    }
}

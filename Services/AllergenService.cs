using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;
using MSSQL;

namespace Services
{
    public class AllergenService
    {
        private readonly AllergenRepository _allergenRepository;

        public AllergenService(AllergenRepository allergenRepository)
        {
            _allergenRepository = allergenRepository;
        }


        public List<Allergen> GetAllergenForMenuItem(int id)
        {
            return _allergenRepository.GetAllergensForMenuItem(id)!;
        }
    }
}

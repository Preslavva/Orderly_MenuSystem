using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Models.Entities;

namespace MSSQL
{
    internal class RestaurantRepository: Restaurant
    {
        public RestaurantRepository(IConfiguration configuration) : base(configuration) { }


    }
}

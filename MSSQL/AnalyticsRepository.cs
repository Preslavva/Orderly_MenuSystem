using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Models.Entities;

namespace MSSQL
{
    public class AnalyticsRepository : Repository
    {
        public AnalyticsRepository(IConfiguration configuration) : base(configuration) {}
        

    }
}

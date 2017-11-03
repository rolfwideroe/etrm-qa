using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadStaticData
{
    public class PortfolioInsert
    {
        public Portfolio[] Portfolios;
    }

    public class Portfolio
    {
        public string CompanyExternalId;
        public string PortfolioNameAndExternalId;
        public string ParentPortfolioExternalId;
    }
}

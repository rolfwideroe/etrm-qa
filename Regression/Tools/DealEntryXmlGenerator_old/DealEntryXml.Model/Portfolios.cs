namespace DealEntryXml.Model
{
    public class Portfolios
    {
        public Portfolios(string portfolio, string counterpartyPortfolio, string interconnector)
        {
            this.Portfolio = portfolio;
            this.CounterpartyPortfolio = counterpartyPortfolio;
            this.Interconnector = interconnector;
        }

        public Portfolios(){}

        public string Portfolio { get; set; }

        public string CounterpartyPortfolio { get; set; }

        public string Interconnector { get; set; }
    }
}
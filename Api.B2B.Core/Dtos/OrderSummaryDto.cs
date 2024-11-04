namespace Api.B2B.Core.Dtos
{
    public class OrderSummaryDto
    {
        public decimal TotalSales { get; set; }
        public int OrderCount { get; set; }
        public decimal AverageTicket { get; set; }
    }
}

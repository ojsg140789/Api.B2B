using System.Linq;
using System.Threading.Tasks;
using Api.B2B.Core.Dtos;
using Api.B2B.Core.Interfaces;
using Api.B2B.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.B2B.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderSummaryDto> GetOrderSummaryAsync(int? branchId = null)
        {
            // Filtrar los pedidos excluyendo los eliminados (DeletedAt == null)
            var filteredOrders = _context.Orders
                .Where(o => o.DeletedAt == null);

            // Si branchId tiene valor, filtrar también por branchId
            if (branchId.HasValue)
            {
                filteredOrders = filteredOrders.Where(o => o.BranchId == branchId.Value);
            }

            // Calcular el total de ventas
            var totalSales = await filteredOrders.SumAsync(o => o.Price);

            // Calcular el número total de pedidos
            var orderCount = await filteredOrders.CountAsync();

            // Calcular el ticket promedio
            var averageTicket = orderCount > 0 ? totalSales / orderCount : 0;

            return new OrderSummaryDto
            {
                TotalSales = totalSales,
                OrderCount = orderCount,
                AverageTicket = averageTicket
            };
        }

        public async Task<List<int>> GetDistinctBranchIdsAsync()
        {
            return await _context.Orders
                .Select(o => o.BranchId)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<OperationDto>> GetOperationsAsync()
        {
            return await _context.Orders
                .Where(o => o.DeletedAt == null)
                .Select(o => new OperationDto
                {
                    LoanId = o.LoanId,
                    UpdatedAt = o.UpdatedAt,
                    Status = o.Status,
                    Price = o.Price
                })
                .ToListAsync();
        }
    }
}

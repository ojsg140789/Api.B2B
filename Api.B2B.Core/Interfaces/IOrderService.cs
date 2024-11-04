using System.Threading.Tasks;
using Api.B2B.Core.Dtos;

namespace Api.B2B.Core.Interfaces
{
    public interface IOrderService
    {
        Task<OrderSummaryDto> GetOrderSummaryAsync(int? branchId = null);
        Task<List<int>> GetDistinctBranchIdsAsync();
        Task<List<OperationDto>> GetOperationsAsync();
    }
}

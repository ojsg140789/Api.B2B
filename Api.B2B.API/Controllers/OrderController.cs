using System.Threading.Tasks;
using Api.B2B.Core.Dtos;
using Api.B2B.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.B2B.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<OrderSummaryDto>> GetOrderSummary([FromQuery] int? branchId = null)
        {
            var summary = await _orderService.GetOrderSummaryAsync(branchId);
            return Ok(summary);
        }

        [HttpGet("distinct-branches")]
        public async Task<ActionResult<List<int>>> GetDistinctBranchIds()
        {
            var branchIds = await _orderService.GetDistinctBranchIdsAsync();
            return Ok(branchIds);
        }

        [HttpGet("operations")]
        public async Task<ActionResult<List<OperationDto>>> GetOperations()
        {
            var operations = await _orderService.GetOperationsAsync();
            return Ok(operations);
        }
    }
}

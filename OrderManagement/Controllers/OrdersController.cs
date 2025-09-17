using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using OrderManagement.DTOs;

namespace OrderManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;
        public OrdersController(IOrderService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            if (dto == null)
                return BadRequest("Order data is missing.");

            try
            {
                var order = await _service.CreateOrderAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }


        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var order = await _service.GetOrderByIdAsync(id);
            return order == null ? NotFound() : Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var orders = await _service.ListOrdersAsync();
            return Ok(orders);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var order = await _service.UpdateOrderStatusAsync(id, dto);
            return order == null ? NotFound() : Ok(order);
        }

        [HttpGet("summary")]
        public async Task<ActionResult<List<OrderSummaryDto>>> GetSummary([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var request = new OrderSummaryRequestDto
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            var summary = await _service.GetSummaryAsync(request);
            return Ok(summary);
        }

    }
}

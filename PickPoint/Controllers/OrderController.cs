using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PickPoint.Core;
using PickPoint.Core.Contracts;
using PickPoint.Data.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PickPoint.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IDeliveryService deliveryService;

        public OrderController(IDeliveryService deliveryService)
        {
            this.deliveryService = deliveryService;
        }
        
        [HttpGet]
        [Route("api/[controller]/{number}")]
        public async Task<Order> Get(int number)
        {
            return await this.deliveryService.GetOrder(number);
        }
        
        [HttpPost]
        [Route("api/[controller]")]
        public async Task<IActionResult> Post([FromBody] Order order)
        {
            var result = await this.deliveryService.CreateOrder(order);

            return GetActionResult(result);
        }

        [HttpPut()]
        [Route("api/[controller]")]
        public async Task<IActionResult> Put([FromBody] Order order)
        {
            var result = await this.deliveryService.UpdateOrder(order);

            return GetActionResult(result);
        }
        
        [HttpPost]
        [Route("api/[controller]/cancel/{number}")]
        public async Task<IActionResult> Cancel(int number)
        {
            var result = await this.deliveryService.CancelOrder(number);

            if (result == OperationResult.ServerError)
            {
                return StatusCode(500, "Something went wrong");
            }

            if (result == OperationResult.NotFound)
            {
                return this.NotFound();
            }

            return this.Ok();
        }

        private IActionResult GetActionResult(OperationResult result)
        {
            switch (result)
            {
                case OperationResult.Locked:
                    return this.Conflict();
                case OperationResult.ValidationFailed:
                    return this.BadRequest();
                case OperationResult.OutOfService:
                    return this.Forbid();
                case OperationResult.NotFound:
                    return this.NotFound();
                case OperationResult.ClientError:
                    return this.BadRequest();
                case OperationResult.ServerError:
                    return StatusCode(500);
                default:
                    return StatusCode(201);
            }
        }
    }
}

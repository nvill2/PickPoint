using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PickPoint.Core.Contracts;
using PickPoint.Data.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PickPoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryPointController : ControllerBase
    {
        private readonly IDeliveryService deliveryService;

        public DeliveryPointController(IDeliveryService deliveryService)
        {
            this.deliveryService = deliveryService;
        }

        [HttpGet]
        public async Task<IEnumerable<DeliveryPoint>> Get()
        {
            return await this.deliveryService.GetWorkingDeliveryPoints();
        }
        
        [HttpGet("{number}")]
        public async Task<IActionResult> Get(string number)
        {
            var dp = await this.deliveryService.GetDeliveryPoint(number);

            if (dp == null)
            {
                return this.NotFound();
            }

            return new JsonResult(dp);
        }
    }
}

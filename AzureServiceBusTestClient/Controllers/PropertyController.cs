using AzureServiceBusTestClient.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace AzureServiceBusTestClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly IServiceBus _serviceBus;

        public PropertyController(IServiceBus serviceBus)
        {
            _serviceBus = serviceBus;
        }

        /// <summary>
        /// Send Property Details to SBQ
        /// </summary>
        [HttpPost("PropertyDetails")]
        public async Task<IActionResult> PropertyDetails(PropertyDetails propertyDetails)
        {
            bool importall = true;
            List<PropertyDetails> properties = new()
            {
                new PropertyDetails { Id = "4b92f56b-38e3-40c4-91f4-0c6de9317bc1", RefId = "duke", ClientId = "MRIQWEB" },
                //new PropertyDetails { Id = "e893a54a-6b87-4f85-8cf1-7cce6eca422c", RefId = "nss", ClientId = "MRIQWEB" }
            };

            if (propertyDetails != null)
            {
                await _serviceBus.SendMessageAsync(properties, importall);
            }
            return Ok();
        }
    }
}

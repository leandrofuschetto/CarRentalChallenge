using CarRental.Service.Vehicles;
using CarRental.WebAPI.DTOs.Vehicle;
using CarRental.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.WebAPI.Controllers
{
    [ApiController]
    [AuthorizeCustomAttribute]
    [Route("api/v1/[controller]")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehiclesService _vehiclesService;
        private readonly ILogger<VehiclesController> _logger;

        public VehiclesController(
            IVehiclesService vehiclesService,
            ILogger<VehiclesController> logger)
        {
            _vehiclesService = vehiclesService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetVehicleResponse>>>GetVehicles(
            bool active = true)
        {
            var vehicles = await _vehiclesService.GetAllVehiclesAsync(active);

            List<GetVehicleResponse> listVehiclesResponse = new();
            foreach (var vehicle in vehicles)
            {
                listVehiclesResponse.Add(new GetVehicleResponse().FromDomain(vehicle));
            };

            return Ok(listVehiclesResponse);
        }

        [HttpGet("{id}", Name = "GetVehicleById")]
        public async Task<ActionResult<GetVehicleResponse>> GetVehicleById(int id)
        {
            var vehicle = await _vehiclesService.GetVehicleByIdAsync(id);
            var vehicleResponse = new GetVehicleResponse().FromDomain(vehicle);

            return Ok(vehicleResponse);
        }

        [HttpPost]
        public async Task<ActionResult<GetVehicleResponse>> CreateVehicle(
            [FromBody] CreateVehicleRequest createVehicleRequest)
        {
            var vehicle = createVehicleRequest.ToDomain();

            var newVehicle = await _vehiclesService.CreateVehicleAsync(vehicle);
            var vehicleResponse = new GetVehicleResponse().FromDomain(newVehicle);

            return CreatedAtRoute(
                nameof(GetVehicleById),
                new { id = newVehicle.Id }, 
                vehicleResponse);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteVehicle(int id)
        {
            if (!await _vehiclesService.DeleteByIdAsync(id))
                throw new Exception($"An error occur while deleting vehicle with id {id}");

            return NoContent();
        }
    }
}

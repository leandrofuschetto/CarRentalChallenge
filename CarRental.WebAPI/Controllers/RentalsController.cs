using CarRental.Service.Rentals;
using CarRental.WebAPI.DTOs.Rental;
using CarRental.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.WebAPI.Controllers
{
    [AuthorizeCustomAttribute]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalsService _rentalService;
        private readonly ILogger<RentalsController> _logger;

        public RentalsController(
            IRentalsService rentalService,
            ILogger<RentalsController> logger)
        {
            _rentalService = rentalService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetRentalResponse>>> GetRentals
            (bool active = true)
        {
            var rentals = await _rentalService.GetAllRentalsAsync(active);

            List<GetRentalResponse> listRentalsResponse = new();
            foreach (var ren in rentals)
            {
                listRentalsResponse.Add(GetRentalResponse.FromDomain(ren));
            };

            return Ok(listRentalsResponse);
        }

        [HttpGet("{id}", Name = "GetRentalById")]
        public async Task<ActionResult<GetRentalResponse>> GetRentalById(int id)
        {
            var rental = await _rentalService.GetRentalByIdAsync(id);
            var rentalResponse = GetRentalResponse.FromDomain(rental);

            return Ok(rentalResponse);
        }

        [HttpPost]
        public async Task<ActionResult<GetRentalResponse>> CreateRental
            ([FromBody] CreateRentalRequest rentalCreateRequest)
        {
            var rental = rentalCreateRequest.ToDomain();

            var newRental = await _rentalService.CreateRentalAsync(rental);
            var rentalResponse = GetRentalResponse.FromDomain(newRental);

            return CreatedAtRoute(
                nameof(GetRentalById),
                new { id = newRental.Id }, 
                rentalResponse);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRental(int id)
        {
            if (!await _rentalService.DeleteByIdAsync(id))
                throw new Exception($"An error occur while cancelling rental with id {id}");

            return NoContent();
        }
    }
}

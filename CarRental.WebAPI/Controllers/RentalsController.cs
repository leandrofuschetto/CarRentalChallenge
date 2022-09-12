using CarRental.Service.Rentals;
using CarRental.WebAPI.DTOs.Rental;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalsService _rentalService;
        
        public RentalsController(IRentalsService rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpGet("{id}", Name = "GetRentalById")]
        public async Task<ActionResult<GetRentalResponse>> GetRentalById(int id)
        {
            var rental = await _rentalService.GetRentalByIdAsync(id);
            var rentalResponse = new GetRentalResponse().FromDomain(rental);

            return Ok(rentalResponse);
        }

        [HttpPost]
        public async Task<ActionResult<GetRentalResponse>> CreateRental
            ([FromBody] CreateRentalRequest rentalCreateRequest)
        {
            var rental = rentalCreateRequest.ToDomain();

            var newRental = await _rentalService.CreateRentalAsync(rental);
            var rentalResponse = new GetRentalResponse().FromDomain(rental);

            return CreatedAtRoute(
                nameof(GetRentalById),
                new { id = newRental.Id }, 
                rentalResponse);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (!await _rentalService.DeleteByIdAsync(id))
                throw new Exception($"An error occur while cancelling a rental with id {id}");

            return NoContent();
        }
    }
}

using CarRental.Service.Clients;
using CarRental.WebAPI.DTOs.Client;
using CarRental.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.WebAPI.Controllers
{
    [AuthorizeCustomAttribute]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientsService _clientsService;
        private readonly ILogger<ClientsController> _logger;

        public ClientsController(
            IClientsService clientsService,
            ILogger<ClientsController> logger)
        {
            _clientsService = clientsService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetClientResponse>>> GetClients(
            bool active = true)
        {
            var clients = await _clientsService.GetAllClientsAsync(active);

            List<GetClientResponse> listClientsResponse = new();
            foreach (var cli in clients)
            {
                listClientsResponse.Add(GetClientResponse.FromDomain(cli));
            };
            
            return Ok(listClientsResponse);
        }

        [HttpGet("{id}", Name = "GetClientById")]
        public async Task<ActionResult<GetClientResponse>> GetClientById(int id)
        {
            var client = await _clientsService.GetClientByIdAsync(id);
            var clientResponse = GetClientResponse.FromDomain(client);

            return Ok(clientResponse);
        }

        [HttpPost]
        public async Task<ActionResult<GetClientResponse>> CreateClient(
            [FromBody] CreateClientRequest clientCreateRequest)
        {
            var client = clientCreateRequest.ToDomain();

            var newClient = await _clientsService.CreateClientAsync(client);
            var clientResponse = GetClientResponse.FromDomain(newClient);

            return CreatedAtRoute(
                nameof(GetClientById), 
                new { id = newClient.Id }, 
                clientResponse);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteClient(int id)
        {
            if (!await _clientsService.DeleteByIdAsync(id))
                throw new Exception($"An error occur while deleting client with id {id}");

            return NoContent();
        }
    }
}

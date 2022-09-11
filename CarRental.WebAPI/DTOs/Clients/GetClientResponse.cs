namespace CarRental.WebAPI.DTOs.Client
{
    public class GetClientResponse
    {
        public GetClientResponse(Domain.Models.Client client)
        { 
            this.ClientId = client.ClientId;
            this.Fullname = client.Fullname;
            this.Email = client.Email;  
        }

        public int ClientId { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
    }
}   

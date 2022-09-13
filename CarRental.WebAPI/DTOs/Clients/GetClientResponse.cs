namespace CarRental.WebAPI.DTOs.Client
{
    public class GetClientResponse
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }

        public static GetClientResponse FromDomain(Domain.Models.Client client)
        {
            GetClientResponse response = new();
            
            response.Id = client.Id;
            response.Fullname = client.Fullname;
            response.Email = client.Email;
            response.Active = client.Active;

            return response;
        }
    }
}   

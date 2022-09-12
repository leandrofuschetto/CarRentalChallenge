namespace CarRental.WebAPI.DTOs.Client
{
    public class GetClientResponse
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }

        public GetClientResponse FromDomain(Domain.Models.Client client)
        {
            this.Id = client.Id;
            this.Fullname = client.Fullname;
            this.Email = client.Email;

            return this;
        }
    }
}   

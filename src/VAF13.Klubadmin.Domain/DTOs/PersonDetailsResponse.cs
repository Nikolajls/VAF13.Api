namespace VAF13.Klubadmin.Domain.DTOs;

public record PersonDetailsResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Birthday { get; set; } = string.Empty;
    public string Club { get; set; } = string.Empty;
    public string ContactRelation { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public int Certificate { get; set; }

    public void Cleanup()
    {
        if (Address == "(skjult)")
        {
            Address = "";
        }
        if (City == "(skjult)")
        {
            City = "";
        }
        if (Zip == "(skjult)")
        {
            Zip = "";
        }
        if (Birthday == "(skjult)")
        {
            Birthday = "";
        }
        if (Phone == "(skjult)")
        {
            Phone = "";
        }
        if (Email == "(skjult)")
        {
            Email = "";
        }
    }
}
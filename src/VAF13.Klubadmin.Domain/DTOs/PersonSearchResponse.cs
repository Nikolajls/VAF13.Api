using Newtonsoft.Json;

namespace VAF13.Klubadmin.Domain.DTOs;

public record PersonSearchResponse
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Club { get; set; } = string.Empty;
    public string DateAdded { get; set; } = string.Empty;
    public string DateRemoved { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int DFUNo { get; set; }
    public string Birthday { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Certificate { get; set; }
    public int Id { get; set; }

    public PersonSearchResponse(KlubadminResults.PersonSearchResult person)
    {
        var fullNameSplit = person.Name.Split(" ");
        var firstName = (fullNameSplit.Length > 0 ? fullNameSplit[0] : string.Empty).Trim();
        var lastName = (fullNameSplit.Length > 1 ? string.Join(" ", fullNameSplit[1..]) : string.Empty).Trim();

        FirstName = firstName;
                    LastName = lastName;
        Club = person.Club;
        DateAdded = person.DateAdded;
        DateRemoved = person.DateRemoved;
        Address = person.Address;
        DFUNo = int.TryParse(person.DFUNo, out var dfuNo) ? dfuNo : 0;
        Birthday = person.Birthday;
        Phone = person.Phone;
        Email = person.Email;
        Type = person.Type;
        Certificate = int.TryParse(person.Certificate, out var cert) ? cert : 0;
        Id = int.TryParse(person.Id, out var id) ? id : 0;
        Cleanup();
    }

    public PersonSearchResponse() { }
    public void Cleanup()
    {
        if (Address == "(skjult)")
        {
            Address = "";
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
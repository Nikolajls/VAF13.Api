namespace VAF13.Klubadmin.Domain.DTOs
{
  public record PersonDetails
  {
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string Zip { get; set; }
    public string Country { get; set; }
    public string Mail { get; set; }
    public string Phone { get; set; }
    public string Birthday { get; set; }
    public string Club { get; set; }
    public string ContactRelation { get; set; }
    public string ContactName { get; set; }
    public string ContactPhone { get; set; }
    public string Gender { get; set; }

  }
}
namespace VAF13.Klubadmin.Domain.DTOs;

public class SearchResult
{
  public int draw { get; set; }
  public int recordsTotal { get; set; }
  public int recordsFiltered { get; set; }
  public PersonSearchResult[] data { get; set; } = Array.Empty<PersonSearchResult>();
}
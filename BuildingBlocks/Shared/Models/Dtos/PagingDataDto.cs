namespace ReviewVerse.Shared.Models.Dtos;

public class PagingDataDto<T>
{
    public List<T> Data { get; set; } = new();
    public PagingDto Paging { get; set; } = new();
}

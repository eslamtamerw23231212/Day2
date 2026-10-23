public record ProductDto(
    int Id,
    string Name,
    decimal Price,
    string CategoryName,
    byte[] RowVersion);
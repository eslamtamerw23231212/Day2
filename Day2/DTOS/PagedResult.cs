namespace Day2.DTOS
{
    public record PagedResult<T>(
     IReadOnlyList<T> Items,
     int PageNumber,
     int PageSize,
     int TotalCount)
    {
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}

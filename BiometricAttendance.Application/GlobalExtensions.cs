namespace BiometricAttendance.Application;

internal static class GlobalExtensions
{
    extension(RequestFilters filters)
    {
        internal RequestFilters Check(HashSet<string> allowedSearchColumns)
        {
            string? searchColumn = string.Empty;

            if (!string.IsNullOrEmpty(filters.SearchValue))
                searchColumn = allowedSearchColumns
                    .FirstOrDefault(x => string.Equals(x, filters.SearchColumn, StringComparison.OrdinalIgnoreCase))
                    ?? allowedSearchColumns.First();

            var newFilters = filters with
            {
                SearchColumn = searchColumn,
            };

            return newFilters;
        }
    }
}

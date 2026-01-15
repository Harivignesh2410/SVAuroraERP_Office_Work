namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Production
{
    public static class DataTableExtensions
    {
        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            var properties = typeof(T).GetProperties();
            var list = new List<T>();

            foreach (DataRow row in table.Rows)
            {
                var item = new T();
                foreach (var prop in properties)
                {
                    var columnName = prop.GetCustomAttributes(typeof(ColumnAttribute), true)
                                         .Cast<ColumnAttribute>()
                                         .FirstOrDefault()?.Name ?? prop.Name;

                    if (table.Columns.Contains(columnName) && row[columnName] != DBNull.Value)
                    {
                        try
                        {
                            var value = row[columnName];
                            var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                            object convertedValue = ConvertValue(value, targetType);
                            prop.SetValue(item, convertedValue);
                        }
                        catch (Exception ex)
                        {
                            // Log the error or handle it as needed
                            Console.WriteLine($"Error converting column {columnName}: {ex.Message}");
                        }
                    }
                }
                list.Add(item);
            }
            return list;
        }

        private static object ConvertValue(object value, Type targetType)
        {
            if (value == null || value == DBNull.Value)
                return null;

            if (targetType.IsAssignableFrom(value.GetType()))
                return value;

            if (targetType == typeof(Guid))
            {
                if (value is string stringValue)
                    return Guid.Parse(stringValue);
                if (value is byte[] byteArray)
                    return new Guid(byteArray);
            }

            if (targetType == typeof(DateTime))
            {
                if (value is string dateString)
                    return DateTime.Parse(dateString);
            }

            if (targetType.IsEnum)
            {
                if (value is string enumString)
                    return Enum.Parse(targetType, enumString);
                return Enum.ToObject(targetType, value);
            }

            try
            {
                return Convert.ChangeType(value, targetType);
            }
            catch (InvalidCastException)
            {
                return value;
            }
        }
    }
}

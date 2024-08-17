using Api.Services.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Reflection;

namespace Api.Services {
    public class CsvService : ICsvService {
        public List<T> Read<T>(string filePath) {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) {
                HeaderValidated = null,
                MissingFieldFound = null,
            });
            return csv.GetRecords<T>().ToList();
        }

        public List<T> Read<T>(IFormFile file, out int countFailedToRead) where T : new() {
            var records = new List<T>();
            countFailedToRead = 0;
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) {
                HeaderValidated = null  
            });

            // skip past headers
            csv.Read();
            csv.ReadHeader();

            while(csv.Read()) {
                var record = CreateRecord<T>(csv, properties);
                if (record != null)
                    records.Add(record);
                else
                    countFailedToRead++;
            }

            return records;
        }

        private T CreateRecord<T>(CsvReader csv, PropertyInfo[] properties) where T : new() {
            var record = new T();
            foreach(var property in properties) {
                if(!TrySetPropertyValue(csv, property,record)) {
                    return default;
                }
            }
            return record;
        }

        private bool TrySetPropertyValue<T>(CsvReader csv, PropertyInfo property, T record) {
            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            if(type == typeof(DateTime)) {
                return TrySetDateTimeProperty(csv, property, record);
            }

            if(csv.TryGetField(type, property.Name, out object value)) {
                property.SetValue(record, value);
                return true;
            }

            Console.WriteLine($"Invalid data for property {property.Name} on line {csv.Parser.Row}");
            return false;
        }

        private bool TrySetDateTimeProperty<T>(CsvReader csv, PropertyInfo property, T record) {
            var formatAttribute = property.GetCustomAttribute<FormatAttribute>();
            string format = formatAttribute?.Formats.FirstOrDefault() ?? "G"; // G is default format

            if(!csv.TryGetField(property.Name, out string dateString)) {
                Console.WriteLine($"Invalid data for property {property.Name} on line {csv.Parser.Row}");
                return false;
            }

            if(!DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime)) {
                Console.WriteLine($"Invalid data for property {property.Name} on line {csv.Parser.Row} | expected format {format}");
                return false;
            }

            property.SetValue(record, dateTime);
            return true;
        }
    }
}
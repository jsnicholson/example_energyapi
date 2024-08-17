using Microsoft.AspNetCore.Http;

namespace Api.Services.Interfaces {
    public interface ICsvService {
        List<T> Read<T>(IFormFile file, out int countFailedToRead) where T : new();
        List<T> Read<T>(string filePath);
    }
}
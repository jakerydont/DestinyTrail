using System.Net.Http;
using System.Threading.Tasks;
using DestinyTrail.Engine.Abstractions;

namespace DestinyTrail.Blazor;
    public class FileReader : IFileReader
    {
        private readonly HttpClient _httpClient;

        public FileReader(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

    public string ReadAllText(string path)
    {
      throw new NotImplementedException();
    }

    public async Task<string> ReadAllTextAsync(string path)
        {
            return await _httpClient.GetStringAsync(path);
        }
    }

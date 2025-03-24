using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace poplensUserProfileApi.Services {
    public interface IUserAuthenticationApiProxyService {
        Task<Dictionary<Guid, string>> GetUsernamesByIdsAsync(List<string> userIds, string token);
    }

    public class UserAuthenticationApiProxyService : IUserAuthenticationApiProxyService {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _userAuthenticationApiUrl = "http://poplensUserAuthenticationApi:8080/api/";

        public UserAuthenticationApiProxyService(IHttpClientFactory httpClientFactory) {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateHttpClientWithAuthorization(string token) {
            var client = _httpClientFactory.CreateClient();

            // Add token to Authorization header
            if (!string.IsNullOrEmpty(token)) {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            } else {
                throw new UnauthorizedAccessException("Token is missing.");
            }

            return client;
        }

        public async Task<Dictionary<Guid, string>> GetUsernamesByIdsAsync(List<string> userIds, string token) {
            var userGuids = userIds.Select(id => Guid.Parse(id)).ToList();

            var client = CreateHttpClientWithAuthorization(token);
            var response = await client.PostAsJsonAsync($"{_userAuthenticationApiUrl}UserAuthentication/GetUsernamesByIdsAsync", userIds);

            if (!response.IsSuccessStatusCode) {
                return null;
            }

            var usernamesJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Dictionary<Guid, string>>(usernamesJson);
        }
    }
}

using BierpongProjectWebApp.Domain.Entities;

namespace BierpongProjectWebApp.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<User?> GetUserAsync(string username)
        {
            return await _httpClient.GetFromJsonAsync<User>($"User/{username}");
        }

        public async Task<bool> AddUserAsync(User user)
        {
            var response = await _httpClient.PostAsJsonAsync("User", user);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("User/validate", new { Username = username, Password = password });
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateUserAsync(string username, User user)
        {
            var response = await _httpClient.PutAsJsonAsync($"User/{username}", user);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteUserAsync(string username)
        {
            var response = await _httpClient.DeleteAsync($"User/{username}");
            return response.IsSuccessStatusCode;
        }
    }
}

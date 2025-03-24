using Newtonsoft.Json;

public class ApiClient
{
    private readonly string _baseUrl;
    private readonly HttpClient _httpClient;

    public ApiClient(string baseUrl)
    {
        _baseUrl = baseUrl;
        _httpClient = new HttpClient();
    }

    public async Task<T> GetAsync<T>(string endpoint, long userId, Dictionary<string, string> queryParams = null)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, BuildUrl(endpoint, queryParams));
        requestMessage.Headers.Add("X-TG-UID", userId.ToString());

        var response = await _httpClient.SendAsync(requestMessage);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }
        else
        {
            throw new ApiException(response.StatusCode, await response.Content.ReadAsStringAsync());
        }
    }

    public async Task<T> PostAsync<T>(string endpoint, long userId, object data)
    {
        var url = $"{_baseUrl}{endpoint}";
        var jsonData = JsonConvert.SerializeObject(data);
        var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = content
        };
        requestMessage.Headers.Add("X-TG-UID", userId.ToString());

        var response = await _httpClient.SendAsync(requestMessage);

        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseData);
        }
        else
        {
            throw new ApiException(response.StatusCode, await response.Content.ReadAsStringAsync());
        }
    }

    public async Task<T> DeleteAsync<T>(string endpoint, long userId)
    {
        var url = $"{_baseUrl}{endpoint}";
        var requestMessage = new HttpRequestMessage(HttpMethod.Delete, url);
        requestMessage.Headers.Add("X-TG-UID", userId.ToString());

        var response = await _httpClient.SendAsync(requestMessage);

        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseData);
        }
        else
        {
            throw new ApiException(response.StatusCode, await response.Content.ReadAsStringAsync());
        }
    }

    private string BuildUrl(string endpoint, Dictionary<string, string> queryParams)
    {
        if (queryParams == null || queryParams.Count == 0)
        {
            return $"{_baseUrl}{endpoint}";
        }

        var queryString = "?";
        foreach (var param in queryParams)
        {
            queryString += $"{param.Key}={param.Value}&";
        }

        return $"{_baseUrl}{endpoint}{queryString.TrimEnd('&')}";
    }
}

public class ApiException : Exception
{
    public ApiException(System.Net.HttpStatusCode statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public System.Net.HttpStatusCode StatusCode { get; }
}

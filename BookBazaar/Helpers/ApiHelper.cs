using BookBazaar.Helpers;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace BookBazaar.Helpers
{
    public class ApiHelper
    {
        private readonly string _baseUrl;
        public ApiHelper(IConfiguration configuration)
        {
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? string.Empty;
        }
        
        private bool TryValidateUri(string endpoint, out Uri? uri)
        {
            var stringUri = _baseUrl + endpoint;
            var isProperUrl = Uri.IsWellFormedUriString(stringUri, (UriKind)1);

            if (isProperUrl)
            {
                uri = new Uri(stringUri);
            }
            else uri = null;

            return isProperUrl;
        }

        public async Task<ResponseModel<T>> ApiCall<T>(string endpoint, object param = null)
        {
            if(!TryValidateUri(endpoint, out Uri? uri))
            {
                return new ResponseModel<T> { Success = false };
            }

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response;

            if (param == null)
            {
                // GET request
                response = await client.GetAsync(uri);
            }
            else
            {
                // POST request
                var json = JsonConvert.SerializeObject(param);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = await client.PostAsync(uri, content);
            }

            string responseJson = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var deserialized = JsonConvert.DeserializeObject<ResponseModel<T>>(responseJson);

                    if (deserialized == null)
                    {
                        return new ResponseModel<T>
                        {
                            Success = false,
                            Message = "Deserialization returned null. Response content was empty or malformed."
                        };
                    }

                    return deserialized;
                }
                catch (Exception ex)
                {
                    return new ResponseModel<T>
                    {
                        Success = false,
                        Message = $"Deserialization error: {ex.Message}\nRaw JSON: {responseJson}"
                    };
                }
            }
            else
            {
                return new ResponseModel<T>
                {
                    Success = false,
                    //Message = $"API call failed: {(int)response.StatusCode} {response.ReasonPhrase}"
                    Message = JsonConvert.DeserializeObject<string>(responseJson)
                };
            }
        }
    }
}



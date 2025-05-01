using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using teste_cliente.Models;
using teste_cliente.Services.IServices;

namespace teste_cliente.Services
{
    public class BaseService : IBaseService
    {
        public APIResponse responseModel { get; set; }
        public IHttpClientFactory httpClient { get; set; }
        public BaseService(IHttpClientFactory httpClient)
        {
            this.responseModel = new();
            this.httpClient = httpClient;
        }
        public async Task<T> SendAsync<T>(APIRequest apiRequest)
        {
            try
            {
                var client = httpClient.CreateClient("MagicAPI");
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.Url);
                if (apiRequest.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),
                        Encoding.UTF8, "application/json");
                }
                switch (apiRequest.ApiType)
                {
                    case SD.ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case SD.ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case SD.ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;

                }

                HttpResponseMessage apiResponse = null;

                if (!string.IsNullOrEmpty(apiRequest.Token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.Token);
                }

                apiResponse = await client.SendAsync(message);

                var apiContent = await apiResponse.Content.ReadAsStringAsync();

                // ─── 1) TRATE 401 UNAUTHORIZED ───────────────────────────────────────────
                if (apiResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // monta um APIResponse com a mensagem de credenciais inválidas
                    var error = new APIResponse
                    {
                        StatusCode = System.Net.HttpStatusCode.Unauthorized,
                        IsSuccess = false,
                        ErrorMessages = new List<string> { apiContent }
                    };
                    // serializa e desserializa para T (neste caso T = APIResponse)
                    var fakeJson = JsonConvert.SerializeObject(error);
                    return JsonConvert.DeserializeObject<T>(fakeJson);
                }

                // ─── 2) TRATE BADREQUEST e NOTFOUND como antes ───────────────────────────
                if (apiResponse.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                    apiResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // tenta desserializar um APIResponse vindo da API
                    var bad = JsonConvert.DeserializeObject<APIResponse>(apiContent);
                    bad.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    bad.IsSuccess = false;
                    var fakeJson = JsonConvert.SerializeObject(bad);
                    return JsonConvert.DeserializeObject<T>(fakeJson);
                }

                // ─── 3) CASO NORMAL: desserialize diretamente em T ────────────────────────
                return JsonConvert.DeserializeObject<T>(apiContent);

            }
            catch(Exception e)
            {
                var dto = new APIResponse
                {
                    ErrorMessages = new List<string> { Convert.ToString(e.Message) },
                    IsSuccess = false
                };
                var res = JsonConvert.SerializeObject(dto);
                var APIResponse = JsonConvert.DeserializeObject<T>(res);
                return APIResponse;
            }
        }
    }
}

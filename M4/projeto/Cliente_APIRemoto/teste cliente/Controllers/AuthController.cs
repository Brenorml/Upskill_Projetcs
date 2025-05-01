using Microsoft.AspNetCore.Mvc;
using teste_cliente.DTOs;
using teste_cliente.Models;
using teste_cliente.Services.IServices;
using teste_cliente.Services;
using JobPortal_API.DTOs;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;

namespace teste_cliente.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        
        public AuthController(IAuthService authService)
        {
            _authService = authService;
           
        }
       [HttpGet]
       public IActionResult Login()
        {
            LoginRequestDTO obj = new();
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Login (LoginRequestDTO obj)
        {
            APIResponse response = await _authService.LoginAsync<APIResponse>(obj);

            Console.WriteLine("=========== DEBUG APIResponse ===========");
            Console.WriteLine($"response: {JsonConvert.SerializeObject(response)}");
            Console.WriteLine($"response.Result: {response?.Result}");
            Console.WriteLine("=========================================");

            if (response != null && response.IsSuccess)
            {
                var json0 = Convert.ToString(response.Result);

                LoginResponseDTO model = (response.Result as JObject)?.ToObject<LoginResponseDTO>();
                
                Console.WriteLine(model);

                
                ///////////////
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name,(model.User.UserName).Trim()));
                identity.AddClaim(new Claim(ClaimTypes.Role, model.User.Role));

                if (model.User.Role == SD.Role_Candidato)
                {
                    // lê o IdCandidato que já veio dentro do token JWT
                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(model.Token);
                    var idCandidato = jwt.Claims.First(c => c.Type == "IdCandidato").Value;
                    identity.AddClaim(new Claim("IdCandidato", idCandidato));
                }

                if (model.User.Role == SD.Role_Empresa)
                {
                    // lê o IdEmpresa que já veio dentro do JWT
                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(model.Token);
                    var idEmpresa = jwt.Claims.First(c => c.Type == "IdEmpresa").Value;
                    identity.AddClaim(new Claim("IdEmpresa", idEmpresa));
                }
                /////////////////

                // adiciona o claim com o token
                identity.AddClaim(new Claim("JWToken", model.Token));

                var principal = new ClaimsPrincipal(identity);
                var props = new AuthenticationProperties
                {
                    IsPersistent = true,                           // persiste além da sessão atual
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(3) 
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                // se a API devolveu 401 (credenciais inválidas)
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ModelState.AddModelError(string.Empty, "Utilizador ou password inválidos. Tente novamente.");
                }
                else
                {
                    // fallback para qualquer outra mensagem de erro
                    var msg = response.ErrorMessages.FirstOrDefault()
                              ?? "Ocorreu um erro inesperado. Tente novamente.";
                    ModelState.AddModelError(string.Empty, msg);
                }
                return View(obj);
            }                
        }

        [HttpGet]
        public IActionResult Register()
        {
            //LoginRequestDTO obj = new();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterationRequestDTO obj)
        {
            using (var httpClient = new HttpClient())
            {
                // Serializa o objeto de registro em JSON
                StringContent content = new StringContent(
                    JsonConvert.SerializeObject(obj),
                    Encoding.UTF8,
                    "application/json"
                );

                // Chamada para o endpoint centralizado da API
                using (var response = await httpClient.PostAsync("https://localhost:7211/api/Auth/register", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Resposta da API: {apiResponse}"); // Log para depuração

                    if (response.IsSuccessStatusCode)
                    {
                        string role = obj.Role?.Trim(); // Usa a role enviada no formulário como fallback

                        try
                        {
                            // Tenta desserializar como APIResponse
                            var responseData = JsonConvert.DeserializeObject<APIResponse>(apiResponse);
                            var result = responseData?.Result as JObject;

                            // Extrai a role do JSON, se disponível
                            if (result != null)
                            {
                                // Tenta direto no result (ex.: { "role": "Admin" })
                                role = result["role"]?.ToString()?.Trim();
                                // Tenta em um objeto user aninhado (ex.: { "user": { "role": "Admin" } })
                                if (string.IsNullOrEmpty(role))
                                {
                                    role = result["user"]?["role"]?.ToString()?.Trim();
                                }
                            }

                            Console.WriteLine($"Role extraída: {role}");
                        }
                        catch (JsonException ex)
                        {
                            // Se a desserialização falhar, usa a role do formulário
                            Console.WriteLine($"Erro ao desserializar resposta: {ex.Message}\nResposta: {apiResponse}");
                            Console.WriteLine($"Usando role do formulário: {role}");
                        }

                        // Define a URL de redirecionamento com base na role
                        string redirectUrl = (role == SD.Role_Candidato || role == SD.Role_Empresa)
                            ? Url.Action("Login", "Auth")
                            : Url.Action("Index", "Home");

                        // Exibe o modal de sucesso
                        return View("Success", ("Conta criada com sucesso!!!", redirectUrl));
                    }
                    else
                    {
                        // Mostra o erro vindo da API
                        Console.WriteLine($"Erro na API: {apiResponse}");
                        ModelState.AddModelError(string.Empty, "Erro ao registrar usuário: " + apiResponse);
                        return View(obj);
                    }
                }
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Register(RegisterationRequestDTO obj)
        //{
        //    using (var httpClient = new HttpClient())
        //    {
        //        // Serializa o objeto de registro em JSON
        //        StringContent content = new StringContent(
        //            JsonConvert.SerializeObject(obj),
        //            Encoding.UTF8,
        //            "application/json"
        //        );

        //        // Chamada para o endpoint centralizado da API
        //        using (var response = await httpClient.PostAsync("https://localhost:7211/api/Auth/register", content))
        //        {
        //            string apiResponse = await response.Content.ReadAsStringAsync();
        //            Console.WriteLine($"Resposta da API: {apiResponse}"); // Log para depuração

        //            if (response.IsSuccessStatusCode)
        //            {
        //                string role = obj.Role?.Trim(); // Usa a role enviada no formulário como fallback

        //                try
        //                {
        //                    // Tenta desserializar como APIResponse
        //                    var responseData = JsonConvert.DeserializeObject<APIResponse>(apiResponse);
        //                    var result = responseData?.Result as JObject;

        //                    // Extrai a role do JSON, se disponível
        //                    if (result != null)
        //                    {
        //                        // Tenta direto no result (ex.: { "role": "Admin" })
        //                        role = result["role"]?.ToString()?.Trim();
        //                        // Tenta em um objeto user aninhado (ex.: { "user": { "role": "Admin" } })
        //                        if (string.IsNullOrEmpty(role))
        //                        {
        //                            role = result["user"]?["role"]?.ToString()?.Trim();
        //                        }
        //                    }

        //                    Console.WriteLine($"Role extraída: {role}");
        //                }
        //                catch (JsonException ex)
        //                {
        //                    // Se a desserialização falhar (ex.: resposta é "User created successfully"), usa a role do formulário
        //                    Console.WriteLine($"Erro ao desserializar resposta: {ex.Message}\nResposta: {apiResponse}");
        //                    Console.WriteLine($"Usando role do formulário: {role}");
        //                }

        //                // Verifica a role e redireciona
        //                if (!string.IsNullOrEmpty(role))
        //                {
        //                    if (role == SD.Role_Candidato || role == SD.Role_Empresa)
        //                    {
        //                        return RedirectToAction("Login");
        //                    }
        //                    else if (role == SD.Role_Admin)
        //                    {
        //                        return RedirectToAction("Index", "Home");
        //                    }
        //                }

        //                // Fallback para sucesso genérico
        //                Console.WriteLine("Role não encontrada, usando fallback para Login");
        //                return RedirectToAction("Login");
        //            }
        //            else
        //            {
        //                // Mostra o erro vindo da API
        //                Console.WriteLine($"Erro na API: {apiResponse}");
        //                ModelState.AddModelError(string.Empty, "Erro ao registrar usuário: " + apiResponse);
        //                return View(obj);
        //            }
        //        }
        //    }
        //}

        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
            //await HttpContext.SignOutAsync();
            //HttpContext.Session.SetString(SD.SessionToken, "");
            //return RedirectToAction("Index", "OfertaEmprego");
        }

        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }

    }
}

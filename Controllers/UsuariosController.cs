using System;
using System.Net;
using System.Net.Http.Headers;
using APIMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APIMvc.Controllers
{
   
    public class UsuariosController : Controller
    {
            private readonly HttpClient _httpClient;
         private readonly string uriBase= "http://localhost:5260/Usuarios/";

         public UsuariosController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(uriBase);
        }
        

        [HttpGet]
        public ActionResult Index()
        {
            return View("CadastrarUsuario");
        }

        
        [HttpPost]
        public async Task<ActionResult> RegistrarAsync(UsuarioViewModel u)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(u));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(uriBase + "Registrar", content);

                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    TempData["Mensagem"] = $"Usuário {u.Username} registrado com sucesso! Faça o login para acessar.";
                    return RedirectToAction("AutenticarUsuario"); // Redireciona para a ação de autenticação
                }
                else
                {
                    string errorMessage = $"Erro ao registrar usuário. StatusCode: {response.StatusCode}. Content: {serialized}";
                    throw new Exception(errorMessage); // Lança uma exceção com detalhes do erro
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao registrar usuário: {ex.Message}";
                return RedirectToAction("AutenticarUsuario"); // Redireciona para a ação de autenticação com mensagem de erro
            }
        }
       

        // GET: /Usuarios/AutenticarUsuario
        
    

        // GET: /Usuarios/AutenticarUsuario
        public IActionResult AutenticarUsuario()
        {
            return View();
        }

         [HttpPost]
        public async Task<ActionResult> AutenticarAsync(UsuarioViewModel u)
        {
            try
            {
                HttpClient httpClient = new  HttpClient();
                string uriComplementar = "Autenticar";

                var content = new StringContent(JsonConvert.SerializeObject(u));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await httpClient.PostAsync(uriBase + uriComplementar, content);

                string serialized = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    UsuarioViewModel uLogado = JsonConvert.DeserializeObject<UsuarioViewModel>(serialized);
                    HttpContext.Session.SetString("SessionTokenUsuario", uLogado.Token);
                    TempData["Mensagem"] = string.Format("Bem vindo {0}!!!", uLogado.Username);
                    return RedirectToAction("Index", "Personagem");
                }
                else{
                    throw new System.Exception(serialized);
                }
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
                //return IndexLogin();              
            }
        
        
        }
        [HttpGet]
        public ActionResult IndexLogin()
        {
            return View("AutenticarUsuario");
        }
   
    }

}
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text; // Importe este namespace para resolver o erro CS0103
using System.Threading.Tasks;
using APIMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APIMvc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FrasesController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string uriBase = "http://localhost:5260/Frases/";

        public FrasesController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(uriBase);
        }

        [HttpGet("Index/{id}")]
        public async Task<ActionResult> IndexAsync(int id)
        {
            try
            {
                string token = HttpContext.Session.GetString("SessionTokenusuario");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient.GetAsync(uriBase + id.ToString());
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<FMViewModel> listaFrases = JsonConvert.DeserializeObject<List<FMViewModel>>(serialized);
                    return View(listaFrases);
                }
                else
                {
                    throw new System.Exception(serialized);
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao carregar frases: {ex.Message}";
                return View(new List<FMViewModel>());
            }
        }

        [HttpGet("Create")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        public async Task<ActionResult> CreateAsync(FMViewModel p)
        {
            try
            {
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var content = new StringContent(JsonConvert.SerializeObject(p), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync(uriBase, content);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TempData["Mensagem"] = string.Format("Frase criada com sucesso! Id: {0}", p.Id);
                    return RedirectToAction("Index", new { id = p.Id });
                }
                else
                {
                    throw new System.Exception(serialized);
                }
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return View("Create");
            }
        }

        [HttpGet("Edit/{id}")]
        public async Task<ActionResult> EditAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            try
            {
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient.GetAsync(uriBase + id.ToString());
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    FMViewModel p = JsonConvert.DeserializeObject<FMViewModel>(serialized);
                    return View(p);
                }
                else
                {
                    throw new System.Exception(serialized);
                }
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet("Delete/{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            try
            {
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient.DeleteAsync(uriBase + id.ToString());
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TempData["Mensagem"] = string.Format("Frase de Id {0} removida com sucesso!", id);
                    return RedirectToAction("Index");
                }
                else
                {
                    throw new System.Exception(serialized);
                }
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}

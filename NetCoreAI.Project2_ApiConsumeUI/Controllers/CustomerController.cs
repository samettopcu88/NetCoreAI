using Microsoft.AspNetCore.Mvc;
using NetCoreAI.Project2_ApiConsumeUI.Dtos;
using Newtonsoft.Json;
using System.Text;

namespace NetCoreAI.Project2_ApiConsumeUI.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory; // Http işlemleri için
        public CustomerController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> CustomerList()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:7178/api/Customers"); 
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultCustomerDto>>(jsonData);
                return View(values);
            }
            return View();
        }

    }
}

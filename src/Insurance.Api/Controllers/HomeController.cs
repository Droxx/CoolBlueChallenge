using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Insurance.Api.Controllers
{
    public class HomeController: Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("api/insurance/product")]
        public InsuranceDto CalculateInsurance([FromBody] InsuranceDto toInsure)
        {
            int productId = toInsure.ProductId;

            BusinessRules.GetProductType(ProductApi, productId, ref toInsure);
            BusinessRules.GetSalesPrice(ProductApi, productId, ref toInsure);

            float insurance = 0f;

            if (toInsure.SalesPrice < 500)
                if (toInsure.ProductTypeName == "Laptops")
                {
                    toInsure.InsuranceValue = 500;
                }
                else
                {
                    toInsure.InsuranceValue = 0;
                }
            else
            {
                if (toInsure.SalesPrice > 500 && toInsure.SalesPrice < 2000)
                    if (toInsure.ProductTypeHasInsurance)
                        toInsure.InsuranceValue += 1000;
                if (toInsure.SalesPrice >= 2000)
                    if (toInsure.ProductTypeHasInsurance)
                        toInsure.InsuranceValue += 2000;
                if (toInsure.ProductTypeName == "Laptops" || toInsure.ProductTypeName == "Smartphones" && toInsure.ProductTypeHasInsurance)
                    toInsure.InsuranceValue += 500;
            }

            return toInsure;
        }

        public class InsuranceDto
        {
            public int ProductId { get; set; }
            public float InsuranceValue { get; set; }
            [JsonIgnore]
            public string ProductTypeName { get; set; }
            [JsonIgnore]
            public bool ProductTypeHasInsurance { get; set; }
            [JsonIgnore]
            public float SalesPrice { get; set; }
        }

        private const string ProductApi = "http://localhost:5002";
    }
}
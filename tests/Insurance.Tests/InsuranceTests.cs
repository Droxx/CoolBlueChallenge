using System;
using System.Collections.Generic;
using System.Linq;
using Insurance.Api.Controllers;
using Insurance.Api.ProductApiModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Xunit;

namespace Insurance.Tests
{
    public class InsuranceTests: IClassFixture<ControllerTestFixture>
    {
        private readonly ControllerTestFixture _fixture;
        private readonly ILoggerFactory _loggerFactory;

        public InsuranceTests(ControllerTestFixture fixture)
        {
            _fixture = fixture;
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            _loggerFactory = serviceProvider.GetService<ILoggerFactory>();
        }

        [Fact]
        public void CalculateInsurance_GivenSalesPriceBetween500And2000Euros_ShouldAddThousandEurosToInsuranceCost()
        {
            const float expectedInsuranceValue = 1000;

            var dto = new HomeController.InsuranceDto
                      {
                          ProductId = 1,
                      };
            var logger = _loggerFactory.CreateLogger<HomeController>();
            var sut = new HomeController(logger);

            var result = sut.CalculateInsurance(dto);

            Assert.Equal(
                expected: expectedInsuranceValue,
                actual: result.InsuranceValue
            );
        }

        [Fact]
        public void Task1_CalculateInsurance_GivenLaptopUnder500_InsurancePriceShouldBe500()
        {
            const float expectedInsuranceValue = 500;

            var dto = new HomeController.InsuranceDto
            {
                ProductId = 2,
            };
            var logger = _loggerFactory.CreateLogger<HomeController>();
            var sut = new HomeController(logger);

            var result = sut.CalculateInsurance(dto);

            Assert.Equal(
                expected: expectedInsuranceValue,
                actual: result.InsuranceValue
            );
        }
    }

    public class ControllerTestFixture: IDisposable
    {
        private readonly IHost _host;

        public ControllerTestFixture()
        {
            _host = new HostBuilder()
                   .ConfigureWebHostDefaults(
                        b => b.UseUrls("http://localhost:5002")
                              .UseStartup<ControllerTestStartup>()
                    )
                   .Build();

            _host.Start();
        }

        public void Dispose() => _host.Dispose();
    }

    public class ControllerTestStartup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(
                ep =>
                {
                    ep.MapGet(
                        "products/{id:int}",
                        context =>
                        {
                            int productId = int.Parse((string) context.Request.RouteValues["id"]);
                            var product = products.FirstOrDefault(p => p.Id == productId);
                            return context.Response.WriteAsync(JsonConvert.SerializeObject(product));
                        }
                    );
                    ep.MapGet(
                        "product_types",
                        context =>
                        {
                            var productTypes = new[]
                                               {
                                                   new
                                                   {
                                                       id = 1,
                                                       name = "Test type",
                                                       canBeInsured = true
                                                   },
                                                   new
                                                   {
                                                       id = 2,
                                                       name = "Laptops",
                                                       canBeInsured = true
                                                   }
                                               };
                            return context.Response.WriteAsync(JsonConvert.SerializeObject(productTypes));
                        }
                    );
                }
            );
        }

        private List<Product> products = new List<Product>() {
                                            new Product()
                                                {
                                                    Id = 1,
                                                    Name = "Test Product",
                                                    ProductTypeId = 1,
                                                    SalesPrice = 750
                                                },
                                            new Product()
                                                {
                                                    Id = 2,
                                                    Name = "Test Laptop",
                                                    ProductTypeId = 2,
                                                    SalesPrice = 450
                                                }
                                            };
    }
}
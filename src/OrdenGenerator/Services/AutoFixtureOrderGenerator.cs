using AutoFixture;
using System;
using System.Collections.Generic;
using System.Text;

namespace SagasDemo.Generator.Services
{
    public class AutoFixtureOrderGenerator : IOrderGenerator
    {
        private const int MAX_INT = 15;
        private readonly Fixture fixture;

        public AutoFixtureOrderGenerator()
        {
            var randomGenerator = new Random();
            this.fixture = new Fixture();
            this.fixture.Register<int>(() => randomGenerator.Next(0, MAX_INT));
        }
        public Order Generate() =>
            new Order
            {
                Cart = this.GenerateShoppingCart(),
                Customer = this.GenerateCustomer(),
                Status = Status.Submitted
            };

        private Customer GenerateCustomer() =>
            this.fixture.Create<Customer>();        
        private ShoppingCart GenerateShoppingCart()
        {
            var products = new List<Product>();
            var productsCount = this.fixture.Create<int>();
            for(var i=0;i< productsCount;i++)
            {
                var product = this.fixture.Create<Product>();
                products.Add(product);
            }
            return new ShoppingCart { Products = products };
        }
    }
}

using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.Extensions.Configuration;
using Stripe;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public PaymentService(IBasketRepository basketRepository, IUnitOfWork unitOfWork, IConfiguration config)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _config = config;
        }

        public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];

            var basket = await _basketRepository.GetBasketAsync(basketId);

            if (basket == null)
                return null;

            var shippingPrice = 0m;

            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync((int)basket.DeliveryMethodId);
                shippingPrice = deliveryMethod.Price;
            }

            foreach (var item in basket.Items)
            {
                var productItem = await _unitOfWork.Repository<Core.Entities.Product>().GetByIdAsync(item.Id);
                if (item.Price != productItem.Price)
                {
                    item.Price = productItem.Price;
                }
            }
            
            var paymentService = new PaymentIntentService();

            PaymentIntent intent;

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var paymentOptions = new PaymentIntentCreateOptions
                {
                    Amount = (long)basket.Items.Sum(i => i.Quantity * (i.Price * 100)) + ((long)shippingPrice * 100),
                    Currency = "inr",
                    PaymentMethodTypes = new List<string> { "card" },
                    Shipping = new ChargeShippingOptions
                    {
                        Name = "Test user",
                        Address = new AddressOptions
                        {
                            Line1 = "Test line 1",
                            PostalCode = "422210",
                            City = "Nashik",
                            State = "MH",
                            Country = "IND",
                        },
                    },
                    Description = "Test description"
                };
                intent = await paymentService.CreateAsync(paymentOptions);
                basket.PaymentIntentId = intent.Id;
                basket.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var paymentOptions = new PaymentIntentUpdateOptions
                {
                    Amount = (long)basket.Items.Sum(i => (i.Quantity * (i.Price * 100))) + (long)(shippingPrice * 100),
                    Shipping = new ChargeShippingOptions
                    {
                        Name = "Jenny Rosen",
                        Address = new AddressOptions
                        {
                            Line1 = "510 Townsend St",
                            PostalCode = "98140",
                            City = "San Francisco",
                            State = "MH",
                            Country = "IND",
                        },
                    },
                    Description = "Software development services"
                };
                await paymentService.UpdateAsync(basket.PaymentIntentId, paymentOptions);
            }

            await _basketRepository.UpdateBasketAsync(basket);

            return basket;
        }

        public async Task<Core.Entities.OrderAggregate.Order> UpdateOrderPaymentFailed(string paymentIntentId)
        {
            var spec = new OrderByPaymentIntentWithItemsSpecification(paymentIntentId);
            var order = await _unitOfWork.Repository<Core.Entities.OrderAggregate.Order>().GetEntityWithSpec(spec);

            if (order == null) 
                return null;

            order.Status = OrderStatus.PaymentFailed;
            _unitOfWork.Repository<Core.Entities.OrderAggregate.Order>().Update(order);

            await _unitOfWork.Complete();

            return order;
        }

        public async Task<Core.Entities.OrderAggregate.Order> UpdateOrderPaymentSucceeded(string paymentIntentId)
        {
            var spec = new OrderByPaymentIntentWithItemsSpecification(paymentIntentId);
            var order = await _unitOfWork.Repository<Core.Entities.OrderAggregate.Order>().GetEntityWithSpec(spec);

            if (order == null) 
                return null;

            order.Status = OrderStatus.PaymentReceived;
            _unitOfWork.Repository<Core.Entities.OrderAggregate.Order>().Update(order);

            await _unitOfWork.Complete();

            return order;
        }
    }
}

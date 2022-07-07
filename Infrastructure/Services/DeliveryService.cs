public class DeliveryService: IDeliveryService
    {
        private Random _random;
        private readonly HttpClient _client;
        private readonly IdentityHubSettings _settings;

        public DeliveryService(IOptions<IdentityHubSettings> settingsAccessor, IHttpClientFactory httpClientFactory)
        {
            _random = new Random();
            _settings = settingsAccessor.Value;
            _client = httpClientFactory.CreateClient("iSHARE-TCNS");
        }

        public async Task<string> VerifyTokenWithPartyIdAsync(string token, string deliveryKey)
        {
            string shipperPartyId = _settings.MyPartyId;
            string actorPartyId = GetActorFromToken(token);
            try
            {                
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);                
                string urlParams = $"deliveryKey={deliveryKey}";
                _client.DefaultRequestHeaders.Add("Token", token);
                var response = await _client.GetAsync($"{_settings.HostTcnsIdentityHub}/api/DelegationEvidence/Deliveries?{urlParams}&showToken=false");
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Log.Error($"Error when post a delegation : {ex.Message}", ex);
                throw new Exception($"Authentication Failures and Invalid Tokens, please try again later.");
            }

            return await Task.FromResult("");
        } 
        
        public DeliveryInfo[] CreateSampleDeliveryInfo()
        {            
            var results = new List<DeliveryInfo>();
            for (int i = 0; i < _random.Next(6, 17); i++)
            {
                DeliveryInfo delivery = AddSampleDelivery();
                results.Add(delivery);
            }
            
            return results.ToArray();
        }
       
        private DeliveryInfo AddSampleDelivery()
        {
            var orderDetails = AddSampleOrderDetails();
            double totalAmounts = Math.Round(orderDetails.Sum(x => x.Quantity * x.PriceAfterSale), 2);
            string deliveryAddress = $"Location {_random.Next(2, 210)} {Guid.NewGuid()}";
            return new DeliveryInfo
            {
                CustomerInformation = AddSampleCustomerInformation(deliveryAddress),
                LspInformation = AddSampleLspInformation(),
                DeliveryInstructions = AddSampleDeliveryInstructions(),
                DeliveryTime = AddSampleDeliveryTime(),
                GeneralInformation = AddSampleGeneralInformation(totalAmounts),
                OrderDetails = orderDetails,
                ReceiverAddress = AddSampleReceiverAddress(deliveryAddress)
            };
        }

        private LspInformation AddSampleLspInformation()
        {
            return new LspInformation
            {
                Address = $"Address {_random.Next(1, 109)} {Guid.NewGuid()}",
                Email = $"Your LSP email",
                Name = $"LSP {_random.Next(250):X}",
                Phone = $"+3120 {_random.Next(101, 999)} {_random.Next(1002, 9567)}",
            };
        }

        private Location AddSampleReceiverAddress(string deliveryAddress)
        {
            return new Location
            {
                Address = deliveryAddress,
                Name = $"Customer {_random.Next(250):X} address",
                City = _random.Next(10) % 2 == 1 ?  "Bleiswijk" : "Amsterdam",
                Country = AddSampleCountry()
            };
        }

        private Country AddSampleCountry()
        {
            return new Country
            {
                Name = "Netherlands",
                Region = "Western Europe"
            };
        }

        private OrderDetail[] AddSampleOrderDetails()
        {
            var results = new List<OrderDetail>();
            for (int i = 0; i < 3; i++)
            {
                OrderDetail item = AddSampleOrderDetail();
                results.Add(item);
            }

            return results.ToArray();
        }

        private OrderDetail AddSampleOrderDetail()
        {
            string str = $"{Guid.NewGuid()}";
            double price = Math.Round((_random.NextDouble() + 0.2) * _random.Next(10, 250), 2);
            return new OrderDetail
            {
                Item = $"Product {str.Substring(3, 8)}{ _random.Next(10)}",
                Price = price,
                Discount = AddSampleDiscount5Percent(),
                CashRefundOffer = 0,
                Quantity = _random.Next(1, 10),
                PriceAfterSale = Math.Round(price * 0.95, 2)
            };
        }

        private Discount AddSampleDiscount5Percent()
        {
            return new Discount
            {
                Name = "5 %",
                Percents = 5
            };
        }

        private GeneralInformation AddSampleGeneralInformation(double totalAmounts)
        {
            return new GeneralInformation
            {
                MethodOfDelivery = $"Unattended to {Guid.NewGuid()}",
                OrderNumber= $"{Guid.NewGuid()}",
                OrderStatus = "New",
                OriginalMethodOfPayment = _random.Next(10) % 2 == 1 ? "Cash payment" : "Pay on Delivery Orders",
                Store = AddSampleStore(),
                Supplies = $"Supplies {_random.Next(250):X}",
                TotalAmount = totalAmounts
            };
        }

        private Store AddSampleStore()
        {
            return new Store
            {
                Department = "Sales and Marketing",
                Name = $"{Guid.NewGuid()}",
                Storekeeper = $"Storekeeper {_random.Next(250):X}"
            };
        }

        private DeliveryTime AddSampleDeliveryTime()
        {
            int days = _random.Next(10);
            return new DeliveryTime
            {
                EstimatedDeliveryTime = DateTime.UtcNow.AddDays(days),
                ExpetedDeliveryFromDate = DateTime.UtcNow,
                ExpetedDeliveryToDate = DateTime.UtcNow.AddDays(days + 1),                
            };
        }

        private DeliveryInstruction[] AddSampleDeliveryInstructions()
        {
            return new DeliveryInstruction[] { };
        }

        private CustomerInformation AddSampleCustomerInformation(string deliveryAddress)
        {
            return new CustomerInformation
            {
                Address = $"Address {_random.Next(1, 109)} {Guid.NewGuid()}",
                DeliveryAddress = deliveryAddress,
                Email = "Your customer email",
                ExpetedDeliveryDate = DateTime.UtcNow.AddDays(_random.Next(2, 10)),
                Name = $"Customer {_random.Next(250):X}",
                Phone = $"+3120 {_random.Next(101, 999)} {_random.Next(1002, 9567)}",
            };
        }
    }

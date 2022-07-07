public class DeliveryInfo
    {
        public CustomerInformation CustomerInformation { get; set; }
        public LspInformation LspInformation { get; set; }
        public GeneralInformation GeneralInformation { get; set; }
        public Location ReceiverAddress { get; set; }
        public DeliveryTime DeliveryTime { get; set; }
        public DeliveryInstruction[] DeliveryInstructions { get; set; }
        public ShippingCost[] ShippingCosts { get; set; }
        public OrderDetail[] OrderDetails { get; set; }
    }

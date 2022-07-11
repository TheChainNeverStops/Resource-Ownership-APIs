    [ApiController]
    [Route("api/[controller]")]
    public class DeliveriesController : ControllerBase
    {
        private IDeliveryService _service;
        public DeliveriesController(IDeliveryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync([FromHeader] string token, [FromQuery] string genericKey)
        {
            if (string.IsNullOrEmpty(token)) return BadRequest();

            try
            {
                var str = await _service.VerifyPartyIdHasPermitOnGenericKeyAsync(token, genericKey);
                if (!str.Equals("permit", StringComparison.CurrentCultureIgnoreCase))
                {
                    Log.Error($"Actor doesn't have permission access this resources {genericKey}");
                    throw new Exception("You don't have permission access this resources");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }

            var data = await _service.ExposedDeliveriesBasedOnGenericKeyAsync(genericKey);
            return new ContentResult
            {
                ContentType = "application/json",
                StatusCode = 200,
                Content = JsonConvert.SerializeObject(data, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented
                })
            };
        }
    }

namespace Coupon.API.Controllers
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Dtos;
    using Infrastructure.Models;
    using Infrastructure.Repositories;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    // [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CouponController : ControllerBase
    {
        private readonly ILogger<CouponController> _logger;
        private readonly ICouponRepository _couponRepository;
        private readonly IMapper<CouponDto, Coupon> _mapper;
        private readonly ExceptionTrigger _exceptionTrigger;

        public CouponController(
            ILogger<CouponController> logger,
            ICouponRepository couponRepository,
            IMapper<CouponDto, Coupon> mapper,
            ExceptionTrigger exceptionTrigger)
        {
            _logger = logger;
            _couponRepository = couponRepository;
            _mapper = mapper;
            _exceptionTrigger = exceptionTrigger;
        }

        [HttpGet("{code}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CouponDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CouponDto>> GetCouponByCodeAsync(string code)
        {
            _logger.LogInformation("----- Get coupon {CouponCode}", code);

            var result = _exceptionTrigger.Process(code);

            if (result.shouldFire)
            {
                throw new Exception($"Exception code received: {code}");
            }

            if (result.configured)
            {
                return NotFound($"CONFIG: {result.message}");
            }

            var coupon = await _couponRepository.FindCouponByCodeAsync(code);

            if (coupon.Consumed)
            {
                return NotFound("ERROR: The coupon has been redeemed already");
            }

            var couponDto = _mapper.Translate(coupon);

            return Ok(couponDto);
        }
    }
}

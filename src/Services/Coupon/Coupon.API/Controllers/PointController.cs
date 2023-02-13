using System.Net;
using Coupon.API.Dtos;
using Coupon.API.Enums;
using Coupon.API.Infrastructure.Models;
using Coupon.API.Infrastructure.Repositories;
using Coupon.API.Infrastructure.Repositories.Point;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Coupon.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class PointController : ControllerBase
{
    private readonly ILogger<PointController> _logger;
    private readonly IPointRepository _pointRepository;
    private readonly IMapper<PointDto, Point> _mapper;
    private readonly ExceptionTrigger _exceptionTrigger;

    public PointController(ILogger<PointController> logger,
        IPointRepository pointRepository,
        IMapper<PointDto, Point> mapper,
        ExceptionTrigger exceptionTrigger)
    {
        _logger = logger;
        _pointRepository = pointRepository;
        _mapper = mapper;
        _exceptionTrigger = exceptionTrigger;
    }

    [HttpGet("{userId}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(PointDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PointDto>> GetPointsByUserIdAsync(string userId)
    {
        _logger.LogInformation("----- Get user {UserId}", userId);

        var result = _exceptionTrigger.Process(userId);

        if (result.shouldFire)
        {
            throw new Exception($"Exception code received: {userId}");
        }

        if (result.configured)
        {
            return NotFound($"CONFIG: {result.message}");
        }

        var point = await _pointRepository.GetPointsByUserId(userId);

        if (point is null)
        {
            return NotFound("ERROR: The user not found");
        }

        var pointDto = _mapper.Translate(point);
        return Ok(pointDto);
    }
    
    // [HttpPost("{userId}")]
    // [ProducesResponseType((int)HttpStatusCode.NotFound)]
    // [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    // [ProducesResponseType(typeof(PointDto), (int)HttpStatusCode.OK)]
    // public async Task<ActionResult<PointDto>> PostPointsByUserIdAsync(string userId)
    // {
    //     _logger.LogInformation("----- Get user {UserId}", userId);
    //
    //     var result = _exceptionTrigger.Process(userId);
    //
    //     if (result.shouldFire)
    //     {
    //         throw new Exception($"Exception code received: {userId}");
    //     }
    //
    //     if (result.configured)
    //     {
    //         return NotFound($"CONFIG: {result.message}");
    //     }
    //
    //     var point = await _pointRepository.CreatePointsBalanceByUserId(userId);
    //
    //     var pointDto = _mapper.Translate(point);
    //
    //     return Ok(pointDto);
    // }
    //
    // [HttpPut("{userId}")]
    // [ProducesResponseType((int)HttpStatusCode.NotFound)]
    // [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    // [ProducesResponseType(typeof(PointDto), (int)HttpStatusCode.OK)]
    // public async Task<ActionResult<PointDto>> UpdatePointsByUserIdAsync(string userId, [FromQuery] int points, [FromQuery] Operation operation)
    // {
    //     _logger.LogInformation("----- Get user {UserId}", userId);
    //
    //     var result = _exceptionTrigger.Process(userId);
    //
    //     if (result.shouldFire)
    //     {
    //         throw new Exception($"Exception code received: {userId}");
    //     }
    //
    //     if (result.configured)
    //     {
    //         return NotFound($"CONFIG: {result.message}");
    //     }
    //
    //     if (points == default)
    //     {
    //         throw new Exception($"The points can't be 0: {userId}");
    //     }
    //     var balance = await _pointRepository.GetPointsByUserId(userId);
    //     if (balance is null)
    //     {
    //         return NotFound($"Balance of user: {userId} wasn't find");
    //     }
    //     if (operation == Operation.Adding)
    //     {
    //         await _pointRepository.AddPointsToBalanceByUser(balance.Id, userId, points);
    //     }
    //     else
    //     {
    //         await _pointRepository.SubtractPointsFromBalanceByUser(balance.Id, userId, points);
    //     }
    //     
    //
    //     // var pointDto = _mapper.Translate(point);
    //
    //     return Ok();
    // }
}
using System.ComponentModel;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using MockReferralsAPI.Dto;
using MockReferralsAPI.Exceptions;
using MockReferralsAPI.Models;
using MockReferralsAPI.Services;

namespace MockReferralsAPI.Controllers;

[Route("/referrals")]
[ApiController]
public class ReferralsController(
    ILogger<ReferralsController> logger,
    IStoreReferralRecords datastore,
    IGenerateReferralLinks linkGenerator
) : ControllerBase
{
    [HttpGet("GetUserReferralContext")]
    [EndpointSummary("Referrals Index")]
    [EndpointDescription("Provides the referral code, shareable referral link, and all referrals for a given user ID")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<UserReferralsResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public ActionResult<UserReferralsResponseDto> Index(
        [FromHeader]
        [Description("ID of a user")]
        string userId
    )
    {
        logger.LogInformation("Received request for referrals for user {ID}", userId);

        UserReferralCode userReferralCode;

        try
        {
            userReferralCode = datastore.GetCodeForUser(userId);
        }
        catch (RecordNotFoundException)
        {
            logger.LogError("Referral code not found for user {ID}", userId);
            return NotFound();
        }

        var referrals = datastore
            .GetReferralsForCode(userReferralCode.Code)
            .Select(refCode => new ReferralDto(refCode))
            .ToList();

        var response = new UserReferralsResponseDto
        {
            Code = userReferralCode.Code,
            ShareableLink = linkGenerator.ForReferralCode(userId),
            Referrals = referrals
        };
 
        logger.LogInformation(
            "Responded with results for code {CODE}",
            userReferralCode.Code);

        return new ActionResult<UserReferralsResponseDto>(response);
    }

    [HttpPut("PutNewReferral")]
    [EndpointSummary("Create New Referral")]
    [EndpointDescription("Creates a new, open referral associated with the referral code of the given user")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType<CreatedResult>(StatusCodes.Status201Created)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public ActionResult Create(
        [FromHeader]
        [Description("ID of a user")]
        string userId,

        [FromBody]
        [Description("JSON representation of a referral")]
        ReferralDto referralDto
    )
    {
        logger.LogInformation("Received referral creation request for user {ID}", userId);
        UserReferralCode userReferralCode;

        try
        {
            userReferralCode = datastore.GetCodeForUser(userId);
        }
        catch (RecordNotFoundException)
        {
            logger.LogError("Referral code not found for user {ID}", userId);
            return NotFound();
        }

        datastore.CreateReferral(userReferralCode.Code, referralDto);

        logger.LogInformation("Successfully created a referral for user {ID}", userId);

        return Created();
    }

    [HttpPost("RedeemReferral")]
    [EndpointSummary("Redeem a Referral")]
    [EndpointDescription("Redeems an unclaimed referral for a new user with the given referral code")]
    [ProducesResponseType<NoContentResult>(StatusCodes.Status204NoContent)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    public ActionResult Redeem(
        [FromHeader]
        [Description("ID of a user")]
        string userId,

        [FromBody]
        [Description("JSON representation of a redemption request, including referral code")]
        RedemptionDto redemptionDto
    )
    {
        logger.LogInformation(
            "Received redemption request for user {ID} and code {CODE}",
            userId, redemptionDto.ReferralCode);

        try
        {
            datastore.Redeem(userId, redemptionDto.ReferralCode);

            logger.LogInformation(
                "Successfully redeemed referral for user {ID} and code {CODE}",
                userId, redemptionDto.ReferralCode);

            return NoContent();
        }
        catch (RecordNotFoundException)
        {
            logger.LogError(
                "Referral not found for user {ID} and code {CODE}",
                userId, redemptionDto.ReferralCode);

            return NotFound();
        }
        catch (InvalidRedemptionException)
        {
            logger.LogError(
                "User {ID} cannot claim their own referral with code {CODE}",
                userId, redemptionDto.ReferralCode);

            return Unauthorized();
        }
    }
}
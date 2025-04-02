using Microsoft.AspNetCore.Mvc;
using MockReferralsAPI.Dto;
using MockReferralsAPI.Exceptions;
using MockReferralsAPI.Models;
using MockReferralsAPI.Services;

namespace MockReferralsAPI.Controllers;

[Microsoft.AspNetCore.Components.Route("/referrals")]
[ApiController]
public class ReferralsController(
    ILogger<ReferralsController> logger,
    IStoreReferralRecords datastore,
    IGenerateReferralLinks linkGenerator
) : ControllerBase
{
    [HttpGet]
    public ActionResult<UserReferralsResponseDto> Index(
        [FromHeader] string userId
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

    [HttpPut]
    public ActionResult Create(
        [FromHeader] string userId,
        [FromBody] ReferralDto referralDto
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

    [HttpPost]
    public ActionResult Redeem(
        [FromHeader] string userId,
        [FromBody] RedemptionDto redemptionDto
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
            logger.LogInformation(
                "Referral not found for user {ID} and code {CODE}",
                userId, redemptionDto.ReferralCode);

            return NotFound();
        }
        catch (InvalidRedemptionException)
        {
            logger.LogInformation(
                "User {ID} cannot claim their own referral with code {CODE}",
                userId, redemptionDto.ReferralCode);

            return Unauthorized();
        }
    }
}
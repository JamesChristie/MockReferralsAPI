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
    IStoreReferralRecords datastore
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

        return Created();
    }
}
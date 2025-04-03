using System.ComponentModel;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using MockReferralsAPI.Dto;
using MockReferralsAPI.Exceptions;
using MockReferralsAPI.Services;

namespace MockReferralsAPI.Controllers;

[Route("/redeem")]
[ApiController]
public class RedemptionController(
    ILogger<ReferralsController> logger,
    IStoreReferralRecords datastore
) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Redirect to App Store")]
    [EndpointDescription("Redirects to the app on the app store from a referral link")]
    [ProducesResponseType<RedirectResult>(StatusCodes.Status302Found)]
    public RedirectResult FromLink(
        [FromQuery]
        [Description("ID of a referral")]
        string? referralId,

        [FromQuery]
        [Description("Referral code")]
        string referralCode
    )
    {
        logger.LogInformation(
            "Received request for ID {ID} and {CODE}",
            referralId, referralCode);
        // NOTE (jchristie@8thlight.com) To fully implement a deferred deep
        // link, this would require responding with an HTML document with
        // attached javascript behavior to add the deep link to the
        // clipboard. At the moment, we are simply redirecting.
        return Redirect(Constants.AppLink);
    }

    [HttpPost]
    [EndpointSummary("Redeem a Referral")]
    [EndpointDescription("Redeems an unclaimed referral for a new user with the given referral code")]
    [Consumes(MediaTypeNames.Application.Json)]
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
            datastore.Redeem(
                userId, redemptionDto.ReferralId, redemptionDto.ReferralCode);

            logger.LogInformation(
                "Successfully redeemed referral for user {ID} and code {CODE}",
                userId, redemptionDto.ReferralCode);

            return NoContent();
        }
        catch (RecordNotFoundException)
        {
            logger.LogError(
                "Referral not found for referral {ID} and code {CODE}",
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
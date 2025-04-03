using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockReferralsAPI;
using MockReferralsAPI.Controllers;
using MockReferralsAPI.Dto;
using MockReferralsAPI.Exceptions;
using MockReferralsAPI.Services;
using Moq;
using NUnit.Framework;

namespace MockReferralsApi.Tests.Controllers;

[TestFixture]
[TestOf(typeof(RedemptionController))]
public class RedemptionControllerTest
{
    // Mocks
    private Mock<ILogger<ReferralsController>> _mockLogger;
    private Mock<IStoreReferralRecords> _mockDatastore;

    // Subject
    private RedemptionController _redemptionController;

    [SetUp]
    public void SetUp()
    {
        // Set Up Mocks
        _mockLogger = new Mock<ILogger<ReferralsController>>();
        _mockDatastore = new Mock<IStoreReferralRecords>();

        // Create Test Subject
        _redemptionController = new RedemptionController(
            _mockLogger.Object, _mockDatastore.Object);
    }

    [
        Test(
            Description = "Given a request for an existing referral, " +
                          "it redirects to the app store"
        )
    ]
    public void RedemptionLink_Redirects()
    {
        // Exercise Method
        var response = _redemptionController
            .FromLink(Constants.ReferralId, Constants.ReferralCode);

        // Verify
        Assert.That(response, Is.TypeOf<RedirectResult>());
        Assert.That(response.Url, Is.EqualTo(Constants.AppLink));
    }

    [
        Test(
            Description = "Given a request from a new user with a valid code, " +
                          "it responds with HTTP 204"
        )
    ]
    public void NewUserWithValidCode_NoContent()
    {
        // Fixed Values
        var redemptionDto = new RedemptionDto
        {
            ReferralCode = Constants.ReferralCode
        };

        // Exercise Method
        var response = _redemptionController
            .Redeem(Constants.NewUserId, redemptionDto);
        
        // Validate
        Assert.That(response, Is.TypeOf<NoContentResult>());
        
        _mockDatastore
            .Verify(store => store.Redeem(Constants.NewUserId, Constants.ReferralCode));
    }

    [
        Test(
            Description = "Given a request from anew user with an invalid code, " +
                          "it responds with HTTP 404"
        )
    ]
    public void NewUserWithInvalidCode_NotFound()
    {
        // Fixed Values
        const string invalidCode = "invalid-referral-code";
        var redemptionDto = new RedemptionDto
        {
            ReferralCode = invalidCode
        };
        
        // Prepare Datastore Mock
        _mockDatastore
            .Setup(store => store.Redeem(Constants.NewUserId, invalidCode))
            .Throws(new RecordNotFoundException("mock-message"));

        // Exercise Method
        var response = _redemptionController
            .Redeem(Constants.NewUserId, redemptionDto);
        
        // Validate
        Assert.That(response, Is.TypeOf<NotFoundResult>());
    }

    [
        Test(
            Description = "Given a request from an existing user with their own code, " +
                          "it responds with HTTP 401"
        )
    ]
    public void ExistingUserWithOwnCode_Unauthorized()
    {
        // Fixed Values
        var redemptionDto = new RedemptionDto
        {
            ReferralCode = Constants.ReferralCode
        };

        // Prepare Datastore Mock
        _mockDatastore
            .Setup(store =>
                store.Redeem(Constants.UserIdWithReferrals, Constants.ReferralCode))
            .Throws(new InvalidRedemptionException("mock-message"));
        
        // Exercise Method
        var response = _redemptionController
            .Redeem(Constants.UserIdWithReferrals, redemptionDto);
        
        // Validate
        Assert.That(response, Is.TypeOf<UnauthorizedResult>());
    }
}
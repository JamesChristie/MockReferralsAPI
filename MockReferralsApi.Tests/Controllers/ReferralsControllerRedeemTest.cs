using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockReferralsAPI;
using MockReferralsAPI.Controllers;
using MockReferralsAPI.Dto;
using MockReferralsAPI.Exceptions;
using MockReferralsAPI.Models;
using MockReferralsAPI.Services;
using Moq;
using NUnit.Framework;

namespace MockReferralsApi.Tests.Controllers;

[TestFixture]
[TestOf(typeof(ReferralsController))]
public class ReferralsControllerRedeemTest
{
    // Mocks
    private Mock<ILogger<ReferralsController>> _mockLogger;
    private Mock<IStoreReferralRecords> _mockDatastore;
    private Mock<IGenerateReferralLinks> _mockLinkGenerator;

    // Subject
    private ReferralsController _referralsController;

    [SetUp]
    public void SetUp()
    {
        // Set Up Mocks
        _mockLogger = new Mock<ILogger<ReferralsController>>();
        _mockDatastore = new Mock<IStoreReferralRecords>();
        _mockLinkGenerator = new Mock<IGenerateReferralLinks>();

        // Create Test Subject
        _referralsController = new ReferralsController(
            _mockLogger.Object, _mockDatastore.Object, _mockLinkGenerator.Object);
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
        var response = _referralsController
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
        var response = _referralsController
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
        var response = _referralsController
            .Redeem(Constants.UserIdWithReferrals, redemptionDto);
        
        // Validate
        Assert.That(response, Is.TypeOf<UnauthorizedResult>());
    }
}

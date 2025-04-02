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
public class ReferralsControllerCreateTest
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
            Description = "Given a request from a user with a code, " +
                          "it creates a referral and replies with HTTP 201"
        )
    ]
    public void UserWithCode_CreateNewReferral_Created()
    {
        // Fixed Values
        var referralDto = new ReferralDto
        {
            Name = "Firstname McLastname",
            Phone = "8675309",
            Email = "user@exmaple.com"
        };
        
        // Prepare Datastore Mock
        var mockReferralCodeRecord = new UserReferralCode()
        {
            Code = Constants.ReferralCode,
            UserId = Constants.UserIdNoReferrals
        };

        _mockDatastore
            .Setup(store => store.GetCodeForUser(Constants.UserIdNoReferrals))
            .Returns(mockReferralCodeRecord);
        
        // Exercise Method
        var response = _referralsController
            .Create(Constants.UserIdNoReferrals, referralDto);
        
        // Validate
        Assert.That(response, Is.TypeOf<CreatedResult>());
        _mockDatastore
            .Verify(store => store.CreateReferral(Constants.ReferralCode, referralDto));
    }

    [
        Test(
            Description = "Given a request from a user without a referral code, " +
                          "it replies with HTTP 404"
        )
    ]
    public void UserWithoutCode_CreateNewReferral_NotFound()
    {
        // Fixed Values
        var referralDto = new ReferralDto
        {
            Name = "Firstname McLastname",
            Phone = "8675309",
            Email = "user@exmaple.com"
        };
        
        // Prepare Datastore Mock
        _mockDatastore
            .Setup(store => store.GetCodeForUser(Constants.UserIdWithoutCode))
            .Throws(new RecordNotFoundException("mock-message"));
        
        // Exercise Method
        var response = _referralsController
            .Create(Constants.UserIdWithoutCode, referralDto);
        
        // Validate
        Assert.That(response, Is.TypeOf<NotFoundResult>());
    }
}
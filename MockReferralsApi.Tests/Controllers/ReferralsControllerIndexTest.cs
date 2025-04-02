using System.Collections.Generic;
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
public class ReferralsControllerIndexTest
{
    // Fixed Values
    private readonly string _mockShareableLink = string.Format(
        Constants.LinkTemplate, Constants.ReferralCode);

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
            Description = "Given a request from an existing user with a code " +
                          "and no existing referral records, " +
                          "it replies with the referral code and an empty list"
        )
    ]
    public void UserWithoutReferrals_CodeWithEmptyList()
    {
        // Prepare Mocks
        var mockReferralCodeRecord = new UserReferralCode()
        {
            Code = Constants.ReferralCode,
            UserId = Constants.UserIdNoReferrals
        };

        _mockDatastore
            .Setup(store => store.GetCodeForUser(Constants.UserIdNoReferrals))
            .Returns(mockReferralCodeRecord);

        _mockDatastore
            .Setup(store => store.GetReferralsForCode(Constants.ReferralCode))
            .Returns([]);

        _mockLinkGenerator
            .Setup(generator => generator.ForReferralCode(Constants.UserIdNoReferrals))
            .Returns(_mockShareableLink);

        // Exercise Method
        var response = _referralsController.Index(Constants.UserIdNoReferrals);

        // Validate
        Assert.That(response.Value, Is.Not.Null);

        var referralsResponse = response.Value;

        Assert.That(referralsResponse.Code,
                    Is.EqualTo(Constants.ReferralCode),
                    "Referral code should equal the expected code");

        Assert.That(referralsResponse.ShareableLink,
                    Is.EqualTo(_mockShareableLink),
                    "Shareable link should equal the expected link");

        Assert.That(referralsResponse.Referrals,
                    Is.Empty,
                    "Referrals list should be empty");
    }

    [
        Test(
            Description = "Given a request from an existing user with a code " +
                          "and existing referral records, " +
                          "it replies with the referral code and referrals"
        )
    ]
    public void UserWithReferrals_CodeWithReferrals()
    {
        // Prepare Mocks
        var mockReferralCodeRecord = new UserReferralCode()
        {
            Code = Constants.ReferralCode,
            UserId = Constants.UserIdWithReferrals
        };

        var referralRecords = new List<Referral> {
            Constants.PendingReferral,
            Constants.RedeemedReferral
        };

        _mockDatastore
            .Setup(store => store.GetCodeForUser(Constants.UserIdWithReferrals))
            .Returns(mockReferralCodeRecord);

        _mockDatastore
            .Setup(store => store.GetReferralsForCode(Constants.ReferralCode))
            .Returns(referralRecords);

        _mockLinkGenerator
            .Setup(generator => generator.ForReferralCode(Constants.UserIdWithReferrals))
            .Returns(_mockShareableLink);

        // Exercise Method
        var response = _referralsController.Index(Constants.UserIdWithReferrals);

        // Validate
        Assert.That(response.Value, Is.Not.Null);

        var referralsResponse = response.Value;

        Assert.That(referralsResponse.Code,
                    Is.EqualTo(Constants.ReferralCode),
                    "Referral code should equal the expected code");

        Assert.That(referralsResponse.ShareableLink,
                    Is.EqualTo(_mockShareableLink),
                    "Shareable link should equal the expected link");

        Assert.That(referralsResponse.Referrals.Count,
                    Is.EqualTo(2),
                    "Referrals list should contain two elements");

        var firstReferral = referralsResponse.Referrals[0];
        var secondReferral = referralsResponse.Referrals[1];

        var firstExpectedReferral = new ReferralDto(Constants.PendingReferral);
        var secondExpectedReferral = new ReferralDto(Constants.RedeemedReferral);

        // Equality is provided via the `record` keyword on the DTO class
        Assert.That(firstReferral, Is.EqualTo(firstExpectedReferral));
        Assert.That(secondReferral, Is.EqualTo(secondExpectedReferral));
    }

    [
        Test(
            Description = "Given a request from a user without a code, " +
                          "it replies with HTTP 404"
        )
    ]
    public void UserWithoutCode_NotFound()
    {
        // Prepare Datastore Mock
        _mockDatastore
            .Setup(store => store.GetCodeForUser(Constants.UserIdWithoutCode))
            .Throws(new RecordNotFoundException("mock-message"));

        // Exercise Method
        var response = _referralsController.Index(Constants.UserIdWithoutCode);
        
        // Validate
        Assert.That(response.Result, Is.TypeOf<NotFoundResult>());
    }
}
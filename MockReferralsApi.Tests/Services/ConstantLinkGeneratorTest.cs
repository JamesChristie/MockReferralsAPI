using MockReferralsAPI;
using MockReferralsAPI.Services;
using NUnit.Framework;

namespace MockReferralsApi.Tests.Services;

[TestFixture]
[TestOf(typeof(ConstantLinkGenerator))]
public class ConstantLinkGeneratorTest
{
    [Test(Description = "Given a pending referral, it returns a link from the template")]
    public void GivenPending_ReturnsLink()
    {
        var generator = new ConstantLinkGenerator();
        var result = generator.ForReferral(Constants.PendingReferral);

        var expectedLink = string.Format(
            Constants.LinkTemplate,
            Constants.PendingReferral.Id,
            Constants.PendingReferral.Code);
        
        Assert.That(result, Is.EqualTo(expectedLink));
    }

    [Test(Description = "Given a redeemed referral, it returns an empty string")]
    public void GivenRedeemed_ReturnsEmpty()
    {
        var generator = new ConstantLinkGenerator();
        var result = generator.ForReferral(Constants.RedeemedReferral);
        
        Assert.That(result, Is.TypeOf<string>());
        Assert.That(result, Is.Empty);
    }
}
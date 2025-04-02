using MockReferralsAPI;
using MockReferralsAPI.Services;
using NUnit.Framework;

namespace MockReferralsApi.Tests.Services;

[TestFixture]
[TestOf(typeof(ConstantLinkGenerator))]
public class ConstantLinkGeneratorTest
{
    [Test(Description = "Given a code, it returns a link with the constant template")]
    public void GivenCode_ReturnsLink()
    {
        var generator = new ConstantLinkGenerator();
        var result = generator.ForReferralCode(Constants.ReferralCode);

        var expectedLink = string.Format(Constants.LinkTemplate, Constants.ReferralCode);
        
        Assert.That(result, Is.EqualTo(expectedLink));
    }
}
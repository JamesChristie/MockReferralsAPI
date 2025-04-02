namespace MockReferralsAPI.Services;

public class ConstantLinkGenerator : IGenerateReferralLinks
{
    public string ForReferralCode(string referralCode)
    {
        return string.Format(Constants.LinkTemplate, referralCode);
    }
}
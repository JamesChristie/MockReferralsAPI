using MockReferralsAPI.Models;

namespace MockReferralsAPI.Services;

public class ConstantLinkGenerator : IGenerateReferralLinks
{
    public string ForReferral(Referral referral)
    {
        return referral.Redeemed
            ? string.Empty
            : string.Format(Constants.LinkTemplate, referral.Id, referral.Code);
    }
}
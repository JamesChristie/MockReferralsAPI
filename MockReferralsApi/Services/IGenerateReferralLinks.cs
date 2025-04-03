using MockReferralsAPI.Models;

namespace MockReferralsAPI.Services;

public interface IGenerateReferralLinks
{
    public string ForReferral(Referral referral);
}
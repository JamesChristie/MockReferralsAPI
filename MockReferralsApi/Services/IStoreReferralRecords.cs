using MockReferralsAPI.Dto;
using MockReferralsAPI.Models;

namespace MockReferralsAPI.Services;

public interface IStoreReferralRecords
{
    UserReferralCode GetCodeForUser(string userId);
    List<Referral> GetReferralsForCode(string referralCode);
    void CreateReferral(string referralCode, ReferralDto referralDto);
    void Redeem(string userId, string referralId, string exampleReferralCode);
}
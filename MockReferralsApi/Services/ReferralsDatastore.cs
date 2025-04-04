using MockReferralsAPI.Dto;
using MockReferralsAPI.Exceptions;
using MockReferralsAPI.Models;

namespace MockReferralsAPI.Services;

public class ReferralsDatastore(
    ReferralsContext referralsContext
) : IStoreReferralRecords
{
    public UserReferralCode GetCodeForUser(string userId)
    {
        try
        {
            return referralsContext.ReferralCodes
                .First(userCode => userCode.UserId == userId);
        }
        catch (InvalidOperationException invalidOperationException)
        {
            throw new RecordNotFoundException(
                $"No referral codes found for user ID {userId}",
                invalidOperationException);
        }
    }

    public List<Referral> GetReferralsForCode(string referralCode)
    {
        return referralsContext.Referrals
            .Where(referral => referral.Code == referralCode)
            .ToList();
    }

    public void CreateReferral(string referralCode, ReferralDto referralDto)
    {
        var referral = new Referral(referralDto)
        {
            Code = referralCode
        };

        referralsContext.Referrals.Add(referral);
        referralsContext.SaveChanges();
    }

    public void Redeem(string userId, string referralId, string referralCode)
    {
        Referral referral;

        try
        {
            referral = referralsContext.Referrals
                .First(record =>
                    record.Id == referralId
                    && record.Code == referralCode
                    && record.RedeemingUserId == null);
        }
        catch (InvalidOperationException invalidOperationException)
        {
            throw new RecordNotFoundException(
                $"No unredeemed referrals found for user ID {userId}",
                invalidOperationException);
        }

        UserReferralCode userReferralCode;

        try
        {
            userReferralCode = referralsContext.ReferralCodes
                .First(record => record.Code == referralCode);
        }
        catch (InvalidOperationException invalidOperationException)
        {
            throw new RecordNotFoundException(
                $"No user referral codes found for code {referralCode}",
                invalidOperationException);
        }

        if (userReferralCode.UserId == userId)
        {
            throw new InvalidRedemptionException(
                $"User {userId} tried to redeem their referral with ID {referralCode}");
        }

        referral.RedeemingUserId = userId;
        referral.Redeemed = true;

        referralsContext.SaveChanges();
    }
}
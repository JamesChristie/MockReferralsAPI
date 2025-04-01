using MockReferralsAPI.Models;

namespace MockReferralsAPI;

// NOTE (jchristie@8thlight.com) This file contains fixed values for
// various aspects of operation. In a production app, these would
// live in the database or environment configuration.
public static class Constants
{
    public const string ReferralCode = "example-referral-code";
    public const string UserIdWithoutReferrals = "used-id-without-referrals";
    public const string UserIdWithReferrals = "used-id-with-referrals";
    public const string UserIdWithoutCode = "user-id-without-code";

    public static readonly Referral PendingReferral = new()
    {
        Code = ReferralCode,
        Name = "Pending User",
        Email = "pending.user@example.com",
        Phone = "8675309",
        Redeemed = false
    };

    public static readonly Referral RedeemedReferral = new()
    {
        Code = ReferralCode,
        Name = "Redeemed User",
        Email = "redeemed.user@example.com",
        Phone = "5551111",
        Redeemed = true
    };
}
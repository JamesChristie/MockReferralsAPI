using Microsoft.EntityFrameworkCore;
using MockReferralsAPI.Dto;

namespace MockReferralsAPI.Models;

[Index(nameof(Phone), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
[Index(nameof(RedeemingUserId), IsUnique = true)]
public class Referral
{
    public required string Code;
    public string? Name;
    public string? Phone;
    public string? Email;
    public string? RedeemingUserId;
    public bool Redeemed;
    
    public Referral() { }

    public Referral(ReferralDto referralDto)
    {
        Name = referralDto.Name;
        Phone = referralDto.Phone;
        Email = referralDto.Email;
        Redeemed = false;
    }
}

using MockReferralsAPI.Dto;

namespace MockReferralsAPI.Models;

public class Referral
{
    public required string Code;
    public string? Name;
    public string? Phone;
    public string? Email;
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

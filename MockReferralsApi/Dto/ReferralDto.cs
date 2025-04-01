using MockReferralsAPI.Models;

namespace MockReferralsAPI.Dto;

public record ReferralDto
{
    public string? Name;
    public string? Phone;
    public string? Email;
    public bool Redeemed;
    
    public ReferralDto() { }

    public ReferralDto(Referral referral)
    {
        Name = referral.Name;
        Phone = referral.Phone;
        Email = referral.Email;
        Redeemed = referral.Redeemed;
    }
}
using MockReferralsAPI.Models;

namespace MockReferralsAPI.Dto;

public record ReferralDto
{
    public string? Name { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public bool Redeemed { get; set; }
    
    public ReferralDto() { }

    public ReferralDto(Referral referral)
    {
        Name = referral.Name;
        Phone = referral.Phone;
        Email = referral.Email;
        Redeemed = referral.Redeemed;
    }
}
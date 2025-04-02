namespace MockReferralsAPI.Dto;

public class UserReferralsResponseDto
{
    public required string Code;
    public required string ShareableLink; 
    public required List<ReferralDto> Referrals;
}
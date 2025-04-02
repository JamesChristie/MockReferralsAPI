namespace MockReferralsAPI.Dto;

public record UserReferralsResponseDto
{
    public required string Code { get; init; }
    public required string ShareableLink { get; init; }
    public required List<ReferralDto> Referrals { get; init; }
}
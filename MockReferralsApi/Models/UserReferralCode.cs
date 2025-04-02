using Microsoft.EntityFrameworkCore;

namespace MockReferralsAPI.Models;

[Index(nameof(UserId), IsUnique = true)]
public class UserReferralCode
{
    public required string UserId;
    public required string Code;
}

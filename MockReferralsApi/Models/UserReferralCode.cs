using Microsoft.EntityFrameworkCore;

namespace MockReferralsAPI.Models;

[Index(nameof(UserId), IsUnique = true)]
public class UserReferralCode
{
    public string UserId;
    public string Code;
}

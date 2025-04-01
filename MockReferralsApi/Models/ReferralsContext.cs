using Microsoft.EntityFrameworkCore;

namespace MockReferralsAPI.Models;

public class ReferralsContext(DbContextOptions<ReferralsContext> options) : DbContext(options)
{
    public virtual DbSet<Referral> Referrals { get; set; }
    public virtual DbSet<UserReferralCode> ReferralCodes { get; set; }
}

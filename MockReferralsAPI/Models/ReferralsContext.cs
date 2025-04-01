using Microsoft.EntityFrameworkCore;

namespace MockReferralsAPI.Models;

public class ReferralsContext : DbContext
{
    public DbSet<Referral> Referrals { get; set; }
    public DbSet<ReferralCode> ReferralCodes { get; set; }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RealEstateCRMWinForms.Data;
using RealEstateCRMWinForms.Models;

namespace RealEstateCRMWinForms.Services
{
    /// <summary>
    /// Provides helper methods for computing revenue metrics with a monthly reset window.
    /// </summary>
    public static class RevenueService
    {
        /// <summary>
        /// Calculates the projected revenue (all closed deals) for the current month.
        /// Includes both active and archived deals so that clearing the pipeline does not
        /// erase the current month's totals prematurely.
        /// </summary>
        public static decimal GetProjectedRevenueForCurrentMonth(DateTime? referenceDateUtc = null)
        {
            var (monthAnchorUtc, _) = GetMonthRangeUtc(referenceDateUtc);
            var targetYear = monthAnchorUtc.Year;
            var targetMonth = monthAnchorUtc.Month;

            using var db = DbContextHelper.CreateDbContext();
            var userRoles = BuildUserRoleLookup(db);

            var closedDeals = db.Deals
                .AsNoTracking()
                .Include(d => d.Property)
                .Where(d =>
                    d.ClosedAt != null ||
                    (!string.IsNullOrEmpty(d.Status) &&
                     (d.Status.Contains("Closed") || d.Status.Contains("Done"))))
                .ToList();

            closedDeals = closedDeals
                .Where(d => IsWithinReferenceMonth(d, targetYear, targetMonth))
                .ToList();

            return SumCommissions(closedDeals, userRoles);
        }

        /// <summary>
        /// Calculates an agent or broker's revenue for the current month.
        /// </summary>
        public static decimal GetUserRevenueForCurrentMonth(User user, DateTime? referenceDateUtc = null)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            string fullName = $"{user.FirstName} {user.LastName}".Trim();
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return 0m;
            }

            var (monthAnchorUtc, _) = GetMonthRangeUtc(referenceDateUtc);
            var targetYear = monthAnchorUtc.Year;
            var targetMonth = monthAnchorUtc.Month;

            using var db = DbContextHelper.CreateDbContext();
            var userRoles = BuildUserRoleLookup(db);

            var closedDeals = db.Deals
                .AsNoTracking()
                .Include(d => d.Property)
                .Where(d =>
                    d.ClosedAt != null ||
                    (!string.IsNullOrEmpty(d.Status) &&
                     (d.Status.Contains("Closed") || d.Status.Contains("Done"))))
                .ToList()
                .Where(d => string.Equals((d.CreatedBy ?? string.Empty).Trim(),
                                          fullName,
                                          StringComparison.OrdinalIgnoreCase))
                .Where(d => IsWithinReferenceMonth(d, targetYear, targetMonth))
                .ToList();

            return SumCommissions(closedDeals, userRoles);
        }

        private static decimal SumCommissions(IEnumerable<Deal> deals, IDictionary<string, UserRole> userRoles)
        {
            decimal total = 0m;
            foreach (var deal in deals)
            {
                total += CalculateCommission(deal, userRoles);
            }

            return total;
        }

        private static decimal CalculateCommission(Deal? deal, IDictionary<string, UserRole> userRoles)
        {
            if (deal == null)
                return 0m;

            if (deal.Value.HasValue && deal.Value.Value > 0)
            {
                return deal.Value.Value;
            }

            decimal propertyPrice = deal.Property?.Price ?? 0m;
            if (propertyPrice <= 0m)
            {
                return 0m;
            }

            decimal commissionRate = 0.05m; // Default agent rate
            if (!string.IsNullOrWhiteSpace(deal.CreatedBy))
            {
                var key = deal.CreatedBy.Trim().ToLowerInvariant();
                if (userRoles.TryGetValue(key, out var role) && role == UserRole.Broker)
                {
                    commissionRate = 0.10m;
                }
            }

            return Math.Round(propertyPrice * commissionRate, 2, MidpointRounding.AwayFromZero);
        }

        private static Dictionary<string, UserRole> BuildUserRoleLookup(AppDbContext dbContext)
        {
            return dbContext.Users
                .AsNoTracking()
                .Select(u => new { u.FirstName, u.LastName, u.RoleInt })
                .ToList()
                .ToDictionary(
                    u => $"{u.FirstName} {u.LastName}".Trim().ToLowerInvariant(),
                    u => Enum.IsDefined(typeof(UserRole), u.RoleInt)
                        ? (UserRole)u.RoleInt
                        : UserRole.Agent);
        }

        private static (DateTime StartUtc, DateTime EndUtc) GetMonthRangeUtc(DateTime? referenceDateUtc)
        {
            var reference = referenceDateUtc?.ToUniversalTime() ?? DateTime.UtcNow;
            var startUtc = new DateTime(reference.Year, reference.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var endUtc = startUtc.AddMonths(1);
            return (startUtc, endUtc);
        }

        private static bool IsWithinReferenceMonth(Deal deal, int targetYear, int targetMonth)
        {
            var effectiveDate = GetEffectiveClosedDate(deal);
            return effectiveDate.Year == targetYear && effectiveDate.Month == targetMonth;
        }

        private static DateTime GetEffectiveClosedDate(Deal deal)
        {
            if (deal.ClosedAt.HasValue) return deal.ClosedAt.Value;
            if (deal.UpdatedAt.HasValue) return deal.UpdatedAt.Value;
            return deal.CreatedAt;
        }
    }
}

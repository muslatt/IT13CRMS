using System.Collections.Generic;
using System.Linq;
using RealEstateCRMWinForms.Data;
using RealEstateCRMWinForms.Models;

namespace RealEstateCRMWinForms.Services
{
    public static class AgentDirectory
    {
        public static List<User> GetActiveAgents()
        {
            using var db = DbContextHelper.CreateDbContext();
            return db.Users
                .Where(u => u.IsActive && u.IsEmailVerified && u.RoleInt == (int)UserRole.Agent)
                .OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
                .ToList();
        }

        public static List<string> GetAgentDisplayNames()
        {
            return GetActiveAgents()
                .Select(u => $"{(u.FirstName ?? string.Empty).Trim()} {(u.LastName ?? string.Empty).Trim()}".Trim())
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct()
                .ToList();
        }
    }
}


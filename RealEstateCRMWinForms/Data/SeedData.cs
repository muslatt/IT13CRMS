using RealEstateCRMWinForms.Models;
using Microsoft.EntityFrameworkCore;

namespace RealEstateCRMWinForms.Data
{
    public static class SeedData
    {
        public static void SeedContacts(AppDbContext context)
        {
            Console.WriteLine("SeedContacts: Starting to check for existing contacts...");

            // Check if seed contacts already exist by looking for specific seed data
            var seedContactExists = context.Contacts.Any(c =>
                c.Email == "john.anderson@email.com" ||
                c.Email == "sarah.mitchell@email.com" ||
                c.Email == "michael.chen@email.com");

            var existingCount = context.Contacts.Count();
            Console.WriteLine($"SeedContacts: Found {existingCount} existing contacts, seed contacts exist: {seedContactExists}");

            if (seedContactExists)
            {
                Console.WriteLine("SeedContacts: Seed data already exists, skipping...");
                return; // Seed data already exists
            }

            Console.WriteLine("SeedContacts: No seed contacts found, creating seed data..."); var contacts = new List<Contact>
            {
                new Contact
                {
                    FullName = "John Anderson",
                    Email = "john.anderson@email.com",
                    Phone = "+1-555-0101",
                    Type = "Buyer",
                    Occupation = "Software Engineer",
                    Salary = 95000m,
                    CreatedAt = DateTime.UtcNow.AddDays(-45),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Sarah Mitchell",
                    Email = "sarah.mitchell@email.com",
                    Phone = "+1-555-0102",
                    Type = "Buyer",
                    Occupation = "Marketing Manager",
                    Salary = 75000m,
                    CreatedAt = DateTime.UtcNow.AddDays(-42),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Michael Chen",
                    Email = "michael.chen@email.com",
                    Phone = "+1-555-0103",
                    Type = "Owner",
                    Occupation = "Doctor",
                    Salary = 180000m,
                    CreatedAt = DateTime.UtcNow.AddDays(-40),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Emily Rodriguez",
                    Email = "emily.rodriguez@email.com",
                    Phone = "+1-555-0104",
                    Type = "Renter",
                    Occupation = "Teacher",
                    Salary = 48000m,
                    CreatedAt = DateTime.UtcNow.AddDays(-38),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "David Thompson",
                    Email = "david.thompson@email.com",
                    Phone = "+1-555-0105",
                    Type = "Buyer",
                    Occupation = "Accountant",
                    Salary = 65000m,
                    CreatedAt = DateTime.UtcNow.AddDays(-35),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Jessica Wang",
                    Email = "jessica.wang@email.com",
                    Phone = "+1-555-0106",
                    Type = "Buyer",
                    Occupation = "Lawyer",
                    Salary = 120000m,
                    CreatedAt = DateTime.UtcNow.AddDays(-33),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Robert Johnson",
                    Email = "robert.johnson@email.com",
                    Phone = "+1-555-0107",
                    Type = "Owner",
                    Occupation = "Business Owner",
                    Salary = 150000m,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Lisa Garcia",
                    Email = "lisa.garcia@email.com",
                    Phone = "+1-555-0108",
                    Type = "Renter",
                    Occupation = "Nurse",
                    Salary = 68000m,
                    CreatedAt = DateTime.UtcNow.AddDays(-28),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Christopher Lee",
                    Email = "christopher.lee@email.com",
                    Phone = "+1-555-0109",
                    Type = "Buyer",
                    Occupation = "Engineer",
                    Salary = 88000m,
                    CreatedAt = DateTime.UtcNow.AddDays(-25),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Amanda Brown",
                    Email = "amanda.brown@email.com",
                    Phone = "+1-555-0110",
                    Type = "Buyer",
                    Occupation = "Project Manager",
                    Salary = 82000m,
                    CreatedAt = DateTime.UtcNow.AddDays(-23),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Daniel Williams",
                    Email = "daniel.williams@email.com",
                    Phone = "+1-555-0111",
                    Type = "Owner",
                    Occupation = "Architect",
                    Salary = 92000m,
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Nicole Davis",
                    Email = "nicole.davis@email.com",
                    Phone = "+1-555-0112",
                    Type = "Renter",
                    Occupation = "Graphic Designer",
                    Salary = 55000m,
                    CreatedAt = DateTime.UtcNow.AddDays(-18),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Kevin Martinez",
                    Email = "kevin.martinez@email.com",
                    Phone = "+1-555-0113",
                    Type = "Buyer",
                    Occupation = "Sales Manager",
                    Salary = 78000m,
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Rachel Taylor",
                    Email = "rachel.taylor@email.com",
                    Phone = "+1-555-0114",
                    Type = "Buyer",
                    Occupation = "Financial Analyst",
                    Salary = 70000m,
                    CreatedAt = DateTime.UtcNow.AddDays(-13),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "James Wilson",
                    Email = "james.wilson@email.com",
                    Phone = "+1-555-0115",
                    Type = "Owner",
                    Occupation = "Dentist",
                    Salary = 160000m,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Stephanie Moore",
                    Email = "stephanie.moore@email.com",
                    Phone = "+1-555-0116",
                    Type = "Renter",
                    Occupation = "Social Worker",
                    Salary = 45000m,
                    CreatedAt = DateTime.UtcNow.AddDays(-8),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Brian Jackson",
                    Email = "brian.jackson@email.com",
                    Phone = "+1-555-0117",
                    Type = "Buyer",
                    Occupation = "IT Manager",
                    Salary = 105000m,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Jennifer White",
                    Email = "jennifer.white@email.com",
                    Phone = "+1-555-0118",
                    Type = "Buyer",
                    Occupation = "Pharmacist",
                    Salary = 125000m,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Anthony Harris",
                    Email = "anthony.harris@email.com",
                    Phone = "+1-555-0119",
                    Type = "Owner",
                    Occupation = "Real Estate Investor",
                    Salary = 200000m,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Michelle Clark",
                    Email = "michelle.clark@email.com",
                    Phone = "+1-555-0120",
                    Type = "Renter",
                    Occupation = "Journalist",
                    Salary = 52000m,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Steven Lewis",
                    Email = "steven.lewis@email.com",
                    Phone = "+1-555-0121",
                    Type = "Buyer",
                    Occupation = "Police Officer",
                    Salary = 58000m,
                    CreatedAt = DateTime.UtcNow.AddHours(-20),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Kimberly Robinson",
                    Email = "kimberly.robinson@email.com",
                    Phone = "+1-555-0122",
                    Type = "Buyer",
                    Occupation = "HR Manager",
                    Salary = 72000m,
                    CreatedAt = DateTime.UtcNow.AddHours(-18),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Matthew Walker",
                    Email = "matthew.walker@email.com",
                    Phone = "+1-555-0123",
                    Type = "Owner",
                    Occupation = "Contractor",
                    Salary = 85000m,
                    CreatedAt = DateTime.UtcNow.AddHours(-15),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Laura Hall",
                    Email = "laura.hall@email.com",
                    Phone = "+1-555-0124",
                    Type = "Renter",
                    Occupation = "Chef",
                    Salary = 42000m,
                    CreatedAt = DateTime.UtcNow.AddHours(-12),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Joshua Allen",
                    Email = "joshua.allen@email.com",
                    Phone = "+1-555-0125",
                    Type = "Buyer",
                    Occupation = "Pilot",
                    Salary = 110000m,
                    CreatedAt = DateTime.UtcNow.AddHours(-10),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Elizabeth Young",
                    Email = "elizabeth.young@email.com",
                    Phone = "+1-555-0126",
                    Type = "Buyer",
                    Occupation = "Veterinarian",
                    Salary = 98000m,
                    CreatedAt = DateTime.UtcNow.AddHours(-8),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "William King",
                    Email = "william.king@email.com",
                    Phone = "+1-555-0127",
                    Type = "Owner",
                    Occupation = "Consultant",
                    Salary = 135000m,
                    CreatedAt = DateTime.UtcNow.AddHours(-6),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Mary Wright",
                    Email = "mary.wright@email.com",
                    Phone = "+1-555-0128",
                    Type = "Renter",
                    Occupation = "Librarian",
                    Salary = 38000m,
                    CreatedAt = DateTime.UtcNow.AddHours(-4),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Charles Lopez",
                    Email = "charles.lopez@email.com",
                    Phone = "+1-555-0129",
                    Type = "Buyer",
                    Occupation = "Data Scientist",
                    Salary = 115000m,
                    CreatedAt = DateTime.UtcNow.AddHours(-2),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Barbara Hill",
                    Email = "barbara.hill@email.com",
                    Phone = "+1-555-0130",
                    Type = "Buyer",
                    Occupation = "Physical Therapist",
                    Salary = 78000m,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-30),
                    IsActive = true
                }
            };

            Console.WriteLine($"SeedContacts: Adding {contacts.Count} contacts to database...");
            context.Contacts.AddRange(contacts);

            Console.WriteLine("SeedContacts: Saving changes...");
            context.SaveChanges();

            Console.WriteLine("SeedContacts: Successfully seeded contacts!");
        }

        public static void Initialize(AppDbContext context)
        {
            Console.WriteLine("SeedData: Initializing...");
            context.Database.EnsureCreated();
            Console.WriteLine("SeedData: Database ensured, calling SeedContacts...");
            SeedContacts(context);
            Console.WriteLine("SeedData: Initialization complete!");
        }
    }
}
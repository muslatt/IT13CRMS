using System.Windows.Forms;
using RealEstateCRMWinForms.Views;
using RealEstateCRMWinForms.Data;
using Microsoft.EntityFrameworkCore;

namespace RealEstateCRMWinForms
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Ensure DB is created and up to date
            try
            {
                Console.WriteLine("Program: Creating database context...");
                using var ctx = DbContextHelper.CreateDbContext();
                Console.WriteLine("Program: Running database migration...");
                ctx.Database.Migrate();

                // Seed initial data
                Console.WriteLine("Program: Calling SeedData.Initialize...");
                SeedData.Initialize(ctx);
                Console.WriteLine("Program: SeedData.Initialize completed!");

                // Self-heal missing columns that older databases may lack (EnsureCreated -> Migrate switch cases)
                try
                {
                    ctx.Database.ExecuteSqlRaw(@"IF COL_LENGTH('Users','PendingPasswordEncrypted') IS NULL 
ALTER TABLE [Users] ADD [PendingPasswordEncrypted] NVARCHAR(MAX) NULL;");
                }
                catch { /* ignore */ }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Program: Error during initialization: {ex.Message}");
                // Ignore at startup; UI will still load and surface issues later
            }

            Application.Run(new MainContainerForm());
        }
    }
}

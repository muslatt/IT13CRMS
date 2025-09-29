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
                using var ctx = DbContextHelper.CreateDbContext();
                ctx.Database.Migrate();

                // Self-heal missing columns that older databases may lack (EnsureCreated -> Migrate switch cases)
                try
                {
                    ctx.Database.ExecuteSqlRaw(@"IF COL_LENGTH('Users','PendingPasswordEncrypted') IS NULL 
ALTER TABLE [Users] ADD [PendingPasswordEncrypted] NVARCHAR(MAX) NULL;");
                }
                catch { /* ignore */ }
            }
            catch
            {
                // Ignore at startup; UI will still load and surface issues later
            }

            Application.Run(new MainContainerForm());
        }
    }
}

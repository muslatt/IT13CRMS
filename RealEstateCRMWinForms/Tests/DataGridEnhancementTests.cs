// RealEstateCRMWinForms\Tests\DataGridEnhancementTests.cs
using System;
using System.Linq;
using System.Windows.Forms;
using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.Controls;
using RealEstateCRMWinForms.Utils;

namespace RealEstateCRMWinForms.Tests
{
    /// <summary>
    /// Tests to verify that DataGrid enhancements don't break existing functionality
    /// </summary>
    public static class DataGridEnhancementTests
    {
        /// <summary>
        /// Runs all enhancement tests
        /// </summary>
        /// <returns>True if all tests pass, false otherwise</returns>
        public static bool RunAllTests()
        {
            try
            {
                TestCustomColumnCreation();
                TestModelExtensions();
                TestUIStylesIntegration();
                
                Console.WriteLine("All DataGrid enhancement tests passed!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DataGrid enhancement tests failed: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Tests that custom columns can be created without errors
        /// </summary>
        private static void TestCustomColumnCreation()
        {
            // Test DataGridViewImageTextColumn
            var imageTextColumn = new DataGridViewImageTextColumn
            {
                ImagePropertyName = "AvatarPath",
                TextPropertyName = "FullName",
                HeaderText = "Name",
                ShowInitialsWhenNoImage = true
            };
            AssertNotNull(imageTextColumn);
            AssertEquals("AvatarPath", imageTextColumn.ImagePropertyName);
            AssertEquals("FullName", imageTextColumn.TextPropertyName);
            
            // Test DataGridViewBadgeColumn
            var badgeColumn = new DataGridViewBadgeColumn();
            AssertNotNull(badgeColumn);
            AssertNotNull(badgeColumn.StatusStyles);
            AssertNotNull(badgeColumn.DefaultStyle);
            
            // Test adding custom status style
            badgeColumn.AddStatusStyle("Test", System.Drawing.Color.Red, System.Drawing.Color.White);
            AssertTrue(badgeColumn.StatusStyles.ContainsKey("Test"));
            
            // Test DataGridViewScoreColumn
            var scoreColumn = new DataGridViewScoreColumn();
            AssertNotNull(scoreColumn);
            AssertEquals(80, scoreColumn.HighScoreThreshold);
            AssertEquals(60, scoreColumn.MediumScoreThreshold);
            
            // Test score color logic
            var highColor = scoreColumn.GetScoreColor(85);
            var mediumColor = scoreColumn.GetScoreColor(70);
            var lowColor = scoreColumn.GetScoreColor(45);
            AssertNotNull(highColor);
            AssertNotNull(mediumColor);
            AssertNotNull(lowColor);
        }
        
        /// <summary>
        /// Tests that model extensions work correctly
        /// </summary>
        private static void TestModelExtensions()
        {
            // Test Lead extensions
            var lead = new Lead
            {
                FullName = "John Doe",
                Status = "Qualified",
                Type = "Buyer",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                LastContacted = DateTime.UtcNow.AddDays(-2)
            };
            
            AssertTrue(lead.Score > 0);
            AssertTrue(lead.Score <= 100);
            AssertNotNull(lead.AssignedAgent);
            AssertNotNull(lead.ScoreDescription);
            AssertEquals(5, lead.DaysOld);
            AssertEquals(2, lead.DaysSinceLastContact);
            
            // Test Contact extensions
            var contact = new Contact
            {
                FullName = "Jane Smith",
                Type = "Buyer",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            };
            
            AssertNotNull(contact.AssignedAgent);
            AssertEquals(3, contact.DaysAsContact);
            AssertNotNull(contact.ContactDuration);
            AssertNotNull(contact.Priority);
            AssertEquals("JS", contact.Initials);
        }
        
        /// <summary>
        /// Tests UIStyles integration
        /// </summary>
        private static void TestUIStylesIntegration()
        {
            // Test status colors
            var (newBg, newText) = UIStyles.GetStatusColors("New");
            AssertNotNull(newBg);
            AssertNotNull(newText);
            
            var (contactedBg, contactedText) = UIStyles.GetStatusColors("Contacted");
            AssertNotNull(contactedBg);
            AssertNotNull(contactedText);
            
            var (qualifiedBg, qualifiedText) = UIStyles.GetStatusColors("Qualified");
            AssertNotNull(qualifiedBg);
            AssertNotNull(qualifiedText);
            
            // Test score colors
            var highScoreColor = UIStyles.GetScoreColor(85);
            var mediumScoreColor = UIStyles.GetScoreColor(70);
            var lowScoreColor = UIStyles.GetScoreColor(45);
            AssertNotNull(highScoreColor);
            AssertNotNull(mediumScoreColor);
            AssertNotNull(lowScoreColor);
            
            // Test avatar colors
            var avatarColor1 = UIStyles.GetAvatarBackgroundColor("John Doe");
            var avatarColor2 = UIStyles.GetAvatarBackgroundColor("John Doe");
            AssertEquals(avatarColor1, avatarColor2); // Should be consistent
            
            // Test fonts
            AssertNotNull(UIStyles.DefaultFont);
            AssertNotNull(UIStyles.BoldFont);
            AssertNotNull(UIStyles.BadgeFont);
        }
        
        /// <summary>
        /// Simple assertion helper for equality
        /// </summary>
        private static void AssertEquals<T>(T expected, T actual)
        {
            if (!Equals(expected, actual))
            {
                throw new Exception($"Expected: {expected}, Actual: {actual}");
            }
        }
        
        /// <summary>
        /// Simple assertion helper for non-null values
        /// </summary>
        private static void AssertNotNull(object obj)
        {
            if (obj == null)
            {
                throw new Exception("Expected non-null value");
            }
        }
        
        /// <summary>
        /// Simple assertion helper for boolean values
        /// </summary>
        private static void AssertTrue(bool condition)
        {
            if (!condition)
            {
                throw new Exception("Expected true condition");
            }
        }
    }
}
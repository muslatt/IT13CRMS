// RealEstateCRMWinForms\Tests\TestRunner.cs
using System;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Tests
{
    /// <summary>
    /// Simple test runner for validating DataGrid enhancements
    /// </summary>
    public static class TestRunner
    {
        /// <summary>
        /// Runs all available tests and shows results
        /// </summary>
        public static void RunAllTests()
        {
            var results = new System.Text.StringBuilder();
            results.AppendLine("DataGrid Enhancement Test Results");
            results.AppendLine("=====================================");
            results.AppendLine();
            
            bool allPassed = true;
            
            // Run AvatarGenerator tests
            try
            {
                bool avatarTestsPassed = AvatarGeneratorTests.RunAllTests();
                results.AppendLine($"✓ AvatarGenerator Tests: {(avatarTestsPassed ? "PASSED" : "FAILED")}");
                allPassed &= avatarTestsPassed;
            }
            catch (Exception ex)
            {
                results.AppendLine($"✗ AvatarGenerator Tests: FAILED - {ex.Message}");
                allPassed = false;
            }
            
            // Run DataGrid Enhancement tests
            try
            {
                bool enhancementTestsPassed = DataGridEnhancementTests.RunAllTests();
                results.AppendLine($"✓ DataGrid Enhancement Tests: {(enhancementTestsPassed ? "PASSED" : "FAILED")}");
                allPassed &= enhancementTestsPassed;
            }
            catch (Exception ex)
            {
                results.AppendLine($"✗ DataGrid Enhancement Tests: FAILED - {ex.Message}");
                allPassed = false;
            }
            
            results.AppendLine();
            results.AppendLine($"Overall Result: {(allPassed ? "ALL TESTS PASSED ✓" : "SOME TESTS FAILED ✗")}");
            
            // Show results in a message box
            MessageBox.Show(results.ToString(), "Test Results", 
                MessageBoxButtons.OK, 
                allPassed ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }
        
        /// <summary>
        /// Validates that the enhanced DataGrids can be created without errors
        /// </summary>
        public static bool ValidateDataGridCreation()
        {
            try
            {
                // Test creating a DataGridView with custom columns
                using (var testGrid = new DataGridView())
                {
                    testGrid.AutoGenerateColumns = false;
                    
                    // Add custom columns
                    var imageTextColumn = new RealEstateCRMWinForms.Controls.DataGridViewImageTextColumn
                    {
                        ImagePropertyName = "AvatarPath",
                        TextPropertyName = "FullName",
                        HeaderText = "Name"
                    };
                    testGrid.Columns.Add(imageTextColumn);
                    
                    var badgeColumn = new RealEstateCRMWinForms.Controls.DataGridViewBadgeColumn
                    {
                        DataPropertyName = "Status",
                        HeaderText = "Status"
                    };
                    testGrid.Columns.Add(badgeColumn);
                    
                    var scoreColumn = new RealEstateCRMWinForms.Controls.DataGridViewScoreColumn
                    {
                        DataPropertyName = "Score",
                        HeaderText = "Score"
                    };
                    testGrid.Columns.Add(scoreColumn);
                    
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DataGrid creation validation failed: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Quick validation method that can be called during application startup
        /// </summary>
        public static void QuickValidation()
        {
            try
            {
                // Quick test of core functionality
                var testLead = new RealEstateCRMWinForms.Models.Lead
                {
                    FullName = "Test User",
                    Type = "Buyer"
                };
                
                // Test that computed properties work
                var score = testLead.Score;
                var agent = testLead.AssignedAgent;
                
                // Test avatar generation
                var avatar = RealEstateCRMWinForms.Utils.AvatarGenerator.GenerateInitialsAvatar("Test User", 32);
                avatar?.Dispose();
                
                // Test UI styles
                var statusColors = RealEstateCRMWinForms.Utils.UIStyles.GetStatusColors("New");
                
                System.Diagnostics.Debug.WriteLine("Quick validation passed - DataGrid enhancements are working");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Quick validation failed: {ex.Message}");
                MessageBox.Show($"DataGrid enhancement validation failed: {ex.Message}\n\nThe application may not display properly.", 
                    "Validation Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
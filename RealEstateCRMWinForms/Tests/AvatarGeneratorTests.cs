// RealEstateCRMWinForms\Tests\AvatarGeneratorTests.cs
using System;
using System.Drawing;
using RealEstateCRMWinForms.Utils;

namespace RealEstateCRMWinForms.Tests
{
    /// <summary>
    /// Unit tests for AvatarGenerator functionality
    /// Note: These are basic validation tests. For production, consider using a proper testing framework like NUnit or xUnit
    /// </summary>
    public static class AvatarGeneratorTests
    {
        /// <summary>
        /// Runs all avatar generator tests
        /// </summary>
        /// <returns>True if all tests pass, false otherwise</returns>
        public static bool RunAllTests()
        {
            try
            {
                TestInitialsExtraction();
                TestAvatarGeneration();
                TestColorConsistency();
                TestEdgeCases();
                
                Console.WriteLine("All AvatarGenerator tests passed!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AvatarGenerator tests failed: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Tests initials extraction from various name formats
        /// </summary>
        private static void TestInitialsExtraction()
        {
            // Test normal full names
            AssertEquals("JD", AvatarGenerator.GetInitials("John Doe"));
            AssertEquals("JS", AvatarGenerator.GetInitials("Jane Smith"));
            
            // Test single names
            AssertEquals("JO", AvatarGenerator.GetInitials("John"));
            AssertEquals("A", AvatarGenerator.GetInitials("A"));
            
            // Test multiple names
            AssertEquals("JD", AvatarGenerator.GetInitials("John Michael Doe"));
            AssertEquals("MS", AvatarGenerator.GetInitials("Mary Jane Smith"));
            
            // Test edge cases
            AssertEquals("??", AvatarGenerator.GetInitials(""));
            AssertEquals("??", AvatarGenerator.GetInitials(null));
            AssertEquals("??", AvatarGenerator.GetInitials("   "));
            
            // Test names with extra spaces
            AssertEquals("JD", AvatarGenerator.GetInitials("  John   Doe  "));
        }
        
        /// <summary>
        /// Tests avatar image generation
        /// </summary>
        private static void TestAvatarGeneration()
        {
            // Test avatar generation with different sizes
            var avatar32 = AvatarGenerator.GenerateInitialsAvatar("John Doe", 32);
            AssertNotNull(avatar32);
            AssertEquals(32, avatar32.Width);
            AssertEquals(32, avatar32.Height);
            
            var avatar64 = AvatarGenerator.GenerateInitialsAvatar("Jane Smith", 64);
            AssertNotNull(avatar64);
            AssertEquals(64, avatar64.Width);
            AssertEquals(64, avatar64.Height);
            
            // Test default agent avatar
            var defaultAvatar = AvatarGenerator.GenerateDefaultAgentAvatar(32);
            AssertNotNull(defaultAvatar);
            AssertEquals(32, defaultAvatar.Width);
            AssertEquals(32, defaultAvatar.Height);
            
            // Clean up
            avatar32.Dispose();
            avatar64.Dispose();
            defaultAvatar.Dispose();
        }
        
        /// <summary>
        /// Tests color generation consistency
        /// </summary>
        private static void TestColorConsistency()
        {
            // Same name should always generate same color
            var color1 = UIStyles.GetAvatarBackgroundColor("John Doe");
            var color2 = UIStyles.GetAvatarBackgroundColor("John Doe");
            AssertEquals(color1, color2);
            
            // Different names should potentially generate different colors
            var color3 = UIStyles.GetAvatarBackgroundColor("Jane Smith");
            // Note: We don't assert they're different since hash collisions are possible
            
            // Test empty/null names
            var colorEmpty = UIStyles.GetAvatarBackgroundColor("");
            var colorNull = UIStyles.GetAvatarBackgroundColor(null);
            AssertNotNull(colorEmpty);
            AssertNotNull(colorNull);
        }
        
        /// <summary>
        /// Tests edge cases and error handling
        /// </summary>
        private static void TestEdgeCases()
        {
            // Test LoadOrGenerateAvatar with non-existent file
            var avatar = AvatarGenerator.LoadOrGenerateAvatar("non_existent_file.jpg", "John Doe", 32);
            AssertNotNull(avatar);
            AssertEquals(32, avatar.Width);
            AssertEquals(32, avatar.Height);
            avatar.Dispose();
            
            // Test with null avatar path
            var avatar2 = AvatarGenerator.LoadOrGenerateAvatar(null, "Jane Smith", 32);
            AssertNotNull(avatar2);
            avatar2.Dispose();
            
            // Test with empty name
            var avatar3 = AvatarGenerator.GenerateInitialsAvatar("", 32);
            AssertNotNull(avatar3);
            avatar3.Dispose();
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
    }
}
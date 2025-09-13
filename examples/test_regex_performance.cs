using System;
using System.Diagnostics;
using System.IO;

class Program
{
    static void Main()
    {
        // Create test content that would previously cause regex timeout
        var testContent = @"
            public void TestMethod() 
            {
                GetZero(value) + Increment(One) + GetZero(value) + Increment(One) + GetZero(value) + Increment(One);
                GetZero(value) + Increment(One) + GetZero(value) + Increment(One) + GetZero(value) + Increment(One);
                GetZero(value) + Increment(One) + GetZero(value) + Increment(One) + GetZero(value) + Increment(One);
            }
            
            public void TestMethod2()
            {
                FixSizes(data) { nested { more nested { deeply nested } } };
                FixSizes(data) { nested { more nested { deeply nested } } };
                FixSizes(data) { nested { more nested { deeply nested } } };
            }
        ";
        
        // Write to temp file
        var tempFile = Path.GetTempFileName() + ".cs";
        File.WriteAllText(tempFile, testContent);
        
        Console.WriteLine($"Testing regex performance with file: {tempFile}");
        Console.WriteLine($"Content length: {testContent.Length} characters");
        
        // Test the transformer
        var transformer = new CSharpToCppTranslator.CustomCSharpToCppTransformer();
        
        var stopwatch = Stopwatch.StartNew();
        
        try 
        {
            var result = transformer.Transform(testContent);
            stopwatch.Stop();
            
            Console.WriteLine($"Transform completed successfully in {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine("No regex timeout occurred!");
            
            // Clean up
            File.Delete(tempFile);
        }
        catch (System.Text.RegularExpressions.RegexMatchTimeoutException ex)
        {
            stopwatch.Stop();
            Console.WriteLine($"ERROR: Regex timeout still occurs after {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Exception: {ex.Message}");
            File.Delete(tempFile);
            Environment.Exit(1);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Console.WriteLine($"Other error occurred after {stopwatch.ElapsedMilliseconds}ms: {ex.Message}");
            File.Delete(tempFile);
            Environment.Exit(1);
        }
    }
}
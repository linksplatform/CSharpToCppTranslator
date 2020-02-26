using System;
using Platform.Collections.Arrays;
using Platform.RegularExpressions.Transformer;

namespace CSharpToCppTranslator
{
    public class Program
    {
        static int Main(string[] args)
        {
            //for (int i = 0; i < args.Length; i++)
            //{
            //    Console.WriteLine(args[i]);   
            //}
            //args = new string[] { @"D:\CodeArchive\Links\Ranges\csharp\Platform.Ranges\EnsureExtensions.cs", @"D:\CodeArchive\Links\Ranges\cpp\Platform.Ranges\EnsureExtensions.cpp", "debug" };
            var csharpToCpp = new CustomCSharpToCppTransformer();
            var transformer = IsDebugModeRequested(args) ? new DebugTransformerDecorator(csharpToCpp, ".cpp") : (ITransformer)new CustomCSharpToCppTransformer();
            var cli = new TransformerCLI(transformer);
            var success = cli.Run(args, out string message);
            if (!string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine(message);
            }
            return success ? 0 : -1;
        }

        static private bool IsDebugModeRequested(string[] args) => args.TryGetElement(2, out string thirdArgument) ? string.Equals(thirdArgument, "debug", StringComparison.OrdinalIgnoreCase) : false;
    }
}

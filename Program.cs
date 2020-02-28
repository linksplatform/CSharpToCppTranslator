using System;
using Platform.Collections.Arrays;
using Platform.RegularExpressions.Transformer;

namespace CSharpToCppTranslator
{
    public class Program
    {
        private const string SourceFileExtension = ".cs";
        private const string TargetFileExtension = ".cpp";

        static void Main(string[] args)
        {
            //for (int i = 0; i < args.Length; i++)
            //{
            //    Console.WriteLine(args[i]);   
            //}
            //args = new string[] { @"D:\CodeArchive\Links\Ranges\csharp\Platform.Ranges\EnsureExtensions.cs", @"D:\CodeArchive\Links\Ranges\cpp\Platform.Ranges\EnsureExtensions.cpp", "debug" };
            var csharpToCpp = new CustomCSharpToCppTransformer();
            var transformer = IsDebugModeRequested(args) ? new LoggingFileTransformer(csharpToCpp, SourceFileExtension, TargetFileExtension) : new FileTransformer(csharpToCpp, SourceFileExtension, TargetFileExtension);
            new TransformerCLI(transformer).Run(args);
        }

        static private bool IsDebugModeRequested(string[] args) => args.TryGetElement(2, out string thirdArgument) ? string.Equals(thirdArgument, "debug", StringComparison.OrdinalIgnoreCase) : false;
    }
}

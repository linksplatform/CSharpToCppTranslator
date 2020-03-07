using System;
using System.IO;
using System.Linq;
using Platform.Collections;
using Platform.Collections.Arrays;
using Platform.RegularExpressions.Transformer;

namespace CSharpToCppTranslator
{
    public class Program
    {
        private const string DefaultSourceFileExtension = ".cs";
        private const string DefaultTargetFileExtension = ".h";

        static void Main(string[] args)
        {
            if (args.IsNullOrEmpty())
            {
                var solutionFolder = Path.GetFullPath("../../../../.");
                var csharpFolder = Path.Combine(solutionFolder, "csharp");
                var cppFolder = Path.Combine(solutionFolder, "cpp");
                var folders = Directory.GetDirectories(csharpFolder).OrderBy(x => x.Length);
                var sourceProjectFolder = folders.First();
                var projectFolderRelativePath = Path.GetRelativePath(csharpFolder, sourceProjectFolder);
                var targetProjectFolder = Path.Combine(cppFolder, projectFolderRelativePath);
                args = new string[] { sourceProjectFolder, targetProjectFolder.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar, "debug"};
            }
            var sourceFileExtension = GetSourceFileExtension(args);
            var targetFileExtension = GetTargetFileExtension(args);
            var csharpToCpp = new CustomCSharpToCppTransformer();
            var transformer = IsDebugModeRequested(args) ? new LoggingFileTransformer(csharpToCpp, sourceFileExtension, targetFileExtension) : new FileTransformer(csharpToCpp, sourceFileExtension, targetFileExtension);
            new TransformerCLI(transformer).Run(args);
        }

        static string GetSourceFileExtension(string[] args) => args.TryGetElement(2, out string sourceFileExtension) ? sourceFileExtension : DefaultSourceFileExtension;

        static string GetTargetFileExtension(string[] args) => args.TryGetElement(3, out string targetFileExtension) ? targetFileExtension : DefaultTargetFileExtension;

        static private bool IsDebugModeRequested(string[] args) => args.TryGetElement(4, out string debugArgument) ? string.Equals(debugArgument, "debug", StringComparison.OrdinalIgnoreCase) : false;
    }
}

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
        private const string SourceFileExtension = ".cs";
        private const string TargetFileExtension = ".cpp";

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
            var csharpToCpp = new CustomCSharpToCppTransformer();
            var transformer = IsDebugModeRequested(args) ? new LoggingFileTransformer(csharpToCpp, SourceFileExtension, TargetFileExtension) : new FileTransformer(csharpToCpp, SourceFileExtension, TargetFileExtension);
            new TransformerCLI(transformer).Run(args);
        }

        static private bool IsDebugModeRequested(string[] args) => args.TryGetElement(2, out string thirdArgument) ? string.Equals(thirdArgument, "debug", StringComparison.OrdinalIgnoreCase) : false;
    }
}

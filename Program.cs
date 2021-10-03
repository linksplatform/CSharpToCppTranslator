using System;
using System.IO;
using System.Linq;
using Platform.Collections;
using Platform.Collections.Arrays;
using Platform.RegularExpressions.Transformer;

namespace CSharpToCppTranslator
{
    /// <summary>
    /// <para>
    /// Represents the program.
    /// </para>
    /// <para></para>
    /// </summary>
    public static class Program
    {
        private const string DefaultSourceFileExtension = ".cs";
        private const string DefaultTargetFileExtension = ".h";

        private static void Main(string[] args)
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
                args = new[] { sourceProjectFolder, targetProjectFolder.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar, "debug"};
            }
            var sourceFileExtension = GetSourceFileExtension(args);
            var targetFileExtension = GetTargetFileExtension(args);
            var csharpToCpp = new CustomCSharpToCppTransformer();
            var transformer = IsDebugModeRequested(args) ? new LoggingFileTransformer(csharpToCpp, sourceFileExtension, targetFileExtension) : new FileTransformer(csharpToCpp, sourceFileExtension, targetFileExtension);
            new TransformerCLI(transformer).Run(args);
        }

        private static string GetSourceFileExtension(string[] args) => args.TryGetElement(2, out var sourceFileExtension) ? sourceFileExtension : DefaultSourceFileExtension;

        private static string GetTargetFileExtension(string[] args) => args.TryGetElement(3, out var targetFileExtension) ? targetFileExtension : DefaultTargetFileExtension;

        private static bool IsDebugModeRequested(string[] args) => args.TryGetElement(4, out var debugArgument) && string.Equals(debugArgument, "debug", StringComparison.OrdinalIgnoreCase);
    }
}

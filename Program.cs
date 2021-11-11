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
        /// <summary>
        /// <para>
        /// The default source file extension.
        /// </para>
        /// <para></para>
        /// </summary>
        private const string DefaultSourceFileExtension = ".cs";
        /// <summary>
        /// <para>
        /// The default target file extension.
        /// </para>
        /// <para></para>
        /// </summary>
        private const string DefaultTargetFileExtension = ".h";

        /// <summary>
        /// <para>
        /// Main the args.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="args">
        /// <para>The args.</para>
        /// <para></para>
        /// </param>
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

        /// <summary>
        /// <para>
        /// Gets the source file extension using the specified args.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="args">
        /// <para>The args.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The string</para>
        /// <para></para>
        /// </returns>
        private static string GetSourceFileExtension(string[] args) => args.TryGetElement(2, out var sourceFileExtension) ? sourceFileExtension : DefaultSourceFileExtension;

        /// <summary>
        /// <para>
        /// Gets the target file extension using the specified args.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="args">
        /// <para>The args.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The string</para>
        /// <para></para>
        /// </returns>
        private static string GetTargetFileExtension(string[] args) => args.TryGetElement(3, out var targetFileExtension) ? targetFileExtension : DefaultTargetFileExtension;

        /// <summary>
        /// <para>
        /// Determines whether is debug mode requested.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="args">
        /// <para>The args.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The bool</para>
        /// <para></para>
        /// </returns>
        private static bool IsDebugModeRequested(string[] args) => args.TryGetElement(4, out var debugArgument) && string.Equals(debugArgument, "debug", StringComparison.OrdinalIgnoreCase);
    }
}

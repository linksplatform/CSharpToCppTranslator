using Platform.RegularExpressions.Transformer;
using System.IO;

namespace CSharpToCppTranslator
{
    public class DebugTransformerDecorator : ITransformer
    {
        private readonly Transformer _baseTransformer;
        private readonly string _targetExtension;

        public DebugTransformerDecorator(Transformer baseTransformer, string targetExtension)
        {
            _baseTransformer = baseTransformer;
            _targetExtension = targetExtension;
        }

        public string Transform(string source, IContext context)
        {
            var path = context?.Path ?? "";
            if (!string.IsNullOrWhiteSpace(path))
            {
                var filename = Path.GetFileNameWithoutExtension(path);
                var targetPath = Path.Combine(Path.GetDirectoryName(path), filename);
                _baseTransformer.WriteStepsToFiles(path, targetPath, _targetExtension, skipFilesWithNoChanges: true);
            }
            return _baseTransformer.Transform(source, context);
        }
    }
}

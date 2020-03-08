using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Platform.RegularExpressions.Transformer;
using Platform.RegularExpressions.Transformer.CSharpToCpp;

namespace CSharpToCppTranslator
{
    public class CustomCSharpToCppTransformer : CSharpToCppTransformer
    {
        public static readonly IList<ISubstitutionRule> CustomRules = new List<SubstitutionRule>
        {
            // Just delete it in GenericCollectionMethodsBase.cs
            (new Regex(@"\r?\n[\t ]+[^\r\n]* GetZero(.|\s)+Increment\(One\)(.|\s)+?}"), "", 0),
            // Just delete it in SizedBinaryTreeMethodsBase.cs
            (new Regex(@"\r?\n[\t ]+[^\r\n]* FixSizes(.|\s)+};"), "    };", 0),
            // Just delete it in SizedAndThreadedAVLBalancedTreeMethods.cs
            (new Regex(@"\r?\n[\t ]+[^\r\n]* PrintNode(.|\s)+?}[\t ]*\r?\n"), "", 0),
            // TElement path[MaxPath] = { {0} }; 
            // TElement path[MaxPath]; path[0] = 0;
            (new Regex(@"TElement path\[([_a-zA-Z0-9]+)\] = \{ \{0\} \};"), "TElement path[$1]; path[0] = 0;", 0),
            // UncheckedConverter<TElement, long>.Default.Convert(node)
            // node
            (new Regex(@"UncheckedConverter<[a-zA-Z0-9:_]+, [a-zA-Z0-9:_]+>\.Default\.Convert\((?<argument>((?<parenthesis>\()|(?<-parenthesis>\))|[^()]*)+)\)"), "${argument}", 0),
            // NumericType<TElement>.BytesSize
            // sizeof(TElement)
            (new Regex(@"NumericType<([a-zA-Z0-9]+)>\.BytesSize"), "sizeof($1)", 0),
            // EqualityComparer<TreeElement>.Default.Equals(this->GetElement(node), default)
            // iszero(GetElement(node), sizeof(TreeElement))
            (new Regex(@"EqualityComparer<TreeElement>\.Default\.Equals\(this->GetElement\(node\), default\)"), "iszero(this->GetElement(node), sizeof(TreeElement))", 0),
            // EnsureOnDebugExtensionRoot
            // Platform::Exceptions::ExtensionRoots::EnsureOnDebugExtensionRoot
            (new Regex(@"(?<before>[^:A-Za-z0-9_])(?<!class )(?<type>EnsureAlwaysExtensionRoot|EnsureOnDebugExtensionRoot|ThrowExtensionRoot)"), "${before}Platform::Exceptions::ExtensionRoots::${type}", 0),
            // Ensure.Always.ArgumentMeetsCriteria(
            // Platform::Exceptions::EnsureExtensions::ArgumentMeetsCriteria(Ensure::Always, 
            (new Regex(@"Ensure\.(?<field>Always|OnDebug)\.(?<method>ArgumentMeetsCriteria|ArgumentNotNull)\("), "Platform::Exceptions::EnsureExtensions::${method}(Platform::Exceptions::Ensure::${field}, ", 0),
            // Ensure.Always.ArgumentMeetsCriteria(
            // Platform::Exceptions::EnsureExtensions::ArgumentMeetsCriteria(Ensure::Always, 
            (new Regex(@"Ensure\.(?<field>Always|OnDebug)\.(?<method>ArgumentMeetsCriteria|ArgumentNotNull)<(?<type>[^>;\n]+)>\("), "Platform::Exceptions::EnsureExtensions::${method}<${type}>(Platform::Exceptions::Ensure::${field}, ", 0),
            // Ensure.Always.ArgumentInRange(
            // Platform::Ranges::EnsureExtensions::ArgumentInRange(Ensure::Always, 
            (new Regex(@"Ensure\.(?<field>Always|OnDebug)\.(?<method>MaximumArgumentIsGreaterOrEqualToMinimum|ArgumentInRange)\("), "Platform::Ranges::EnsureExtensions::${method}(Platform::Exceptions::Ensure::${field}, ", 0),
            // Ensure.Always.ArgumentInRange(
            // Platform::Ranges::EnsureExtensions::ArgumentInRange(Ensure::Always, 
            (new Regex(@"Ensure\.(?<field>Always|OnDebug)\.(?<method>MaximumArgumentIsGreaterOrEqualToMinimum|ArgumentInRange)<(?<type>[^>;\n]+)>\("), "Platform::Ranges::EnsureExtensions::${method}<${type}>(Platform::Exceptions::Ensure::${field}, ", 0),
            // IgnoredExceptions.RaiseExceptionIgnoredEvent(
            // IgnoredExceptions::RaiseExceptionIgnoredEvent(
            (new Regex(@"IgnoredExceptions\.RaiseExceptionIgnoredEvent\("), "IgnoredExceptions::RaiseExceptionIgnoredEvent(", 0),
            // sb.append(exception.what()).append(1, '\n'); ... sb.append(Platform::Converters::To<std::string>(exception.StackTrace)).append(1, '\n');
            // sb.append(exception.what()).append(1, '\n');
            (new Regex(@"(?<begin>sb\.append\(exception\.what\(\)\)\.append\(1, '\\n'\);)(.|\n)+sb\.append\(Platform::Converters::To<std::string>\(exception\.StackTrace\)\)\.append\(1, '\\n'\);"), "${begin}", 0),
            // Insert scope borders.
            // const std::exception& ex
            // const std::exception& ex/*~ex~*/
            (new Regex(@"(?<before>\(| )(?<variableDefinition>(const )?(std::)?exception&? (?<variable>[_a-zA-Z0-9]+))(?<after>\W)"), "${before}${variableDefinition}/*~${variable}~*/${after}", 0),
            // Inside the scope of ~!ex!~ replace:
            // ex.Ignore()
            // Platform::Exceptions::ExceptionExtensions::Ignore(ex)
            (new Regex(@"(?<scope>/\*~(?<variable>[_a-zA-Z0-9]+)~\*/)(?<separator>.|\n)(?<before>((?<!/\*~\k<variable>~\*/)(.|\n))*?)\k<variable>\.Ignore\(\)"), "${scope}${separator}${before}Platform::Exceptions::ExceptionExtensions::Ignore(${variable})", 10),
            // Remove scope borders.
            // /*~ex~*/
            // 
            (new Regex(@"/\*~[_a-zA-Z0-9]+~\*/"), "", 0),
            // Zero
            // 0
            (new Regex(@"(\W)(Zero)(\W)"), "${1}0$3", 0),
            // One
            // 1
            (new Regex(@"(\W)(One)(\W)"), "${1}1$3", 0),
            // Two
            // 2
            (new Regex(@"(\W)(Two)(\W)"), "${1}2$3", 0),
            // Comparer.Compare(firstArgument, secondArgument) < 0
            // (firstArgument) < (secondArgument)
            (new Regex(@"(?<separator>\W)Comparer\.Compare\((?<firstArgument>((?<parenthesis>\()|(?<-parenthesis>\))|[^(),]*)+), (?<secondArgument>((?<parenthesis>\()|(?<-parenthesis>\))|[^()]*)+)\) (?<operator>\S{1,2}) 0"), "${separator}(${firstArgument}) ${operator} (${secondArgument})", 0),
            // !this->AreEqual(firstArgument, secondArgument)
            // (firstArgument) != (secondArgument)
            (new Regex(@"(?<separator>\W)!(this->AreEqual|EqualityComparer\.Equals|EqualityComparer<[a-zA-Z0-9]+>\.Default\.Equals)\((?<firstArgument>((?<parenthesis>\()|(?<-parenthesis>\))|[^(),]*)+), (?<secondArgument>((?<parenthesis>\()|(?<-parenthesis>\))|[^()]*)+)\)"), "${separator}(${firstArgument}) != (${secondArgument})", 0),
            // this->AreEqual(firstArgument, secondArgument)
            // (firstArgument) == (secondArgument)
            (new Regex(@"(?<separator>\W)(?<!::)(this->AreEqual|EqualityComparer\.Equals|EqualityComparer<[a-zA-Z0-9]+>\.Default\.Equals)\((?<firstArgument>((?<parenthesis>\()|(?<-parenthesis>\))|[^(),]*)+), (?<secondArgument>((?<parenthesis>\()|(?<-parenthesis>\))|[^()]*)+)\)"), "${separator}(${firstArgument}) == (${secondArgument})", 0),
            // !this->EqualToZero(argument)
            // (argument) != 0
            (new Regex(@"(?<separator>\W)!this->EqualToZero\((?<argument>((?<parenthesis>\()|(?<-parenthesis>\))|[^()]*)+)\)"), "${separator}(${argument}) != 0", 0),
            // this->EqualToZero(argument)
            // (argument) == 0
            (new Regex(@"(?<separator>\W)this->EqualToZero\((?<argument>((?<parenthesis>\()|(?<-parenthesis>\))|[^()]*)+)\)"), "${separator}(${argument}) == 0", 0),
            // this->Add(firstArgument, secondArgument)
            // (firstArgument) + (secondArgument)
            (new Regex(@"(?<separator>\W)(Arithmetic\.Add|this->Add)\((?<firstArgument>((?<parenthesis>\()|(?<-parenthesis>\))|[^(),]*)+), (?<secondArgument>((?<parenthesis>\()|(?<-parenthesis>\))|[^()]*)+)\)"), "${separator}(${firstArgument}) + (${secondArgument})", 0),
            // this->Increment(argument)
            // (argument) + 1
            (new Regex(@"(?<separator>\W)(Arithmetic\.Increment|this->Increment)\((?<argument>((?<parenthesis>\()|(?<-parenthesis>\))|[^()]*)+)\)"), "${separator}(${argument}) + 1", 0),
            // this->Decrement(argument)
            // (argument) - 1;
            (new Regex(@"(?<separator>\W)(Arithmetic\.Decrement|this->Decrement)\((?<argument>((?<parenthesis>\()|(?<-parenthesis>\))|[^()]*)+)\)"), "${separator}(${argument}) - 1", 0),
            // this->GreaterThan(firstArgument, secondArgument)
            // (firstArgument) > (secondArgument)
            (new Regex(@"(?<separator>\W)this->GreaterThan\((?<firstArgument>((?<parenthesis>\()|(?<-parenthesis>\))|[^(),]*)+), (?<secondArgument>((?<parenthesis>\()|(?<-parenthesis>\))|[^()]*)+)\)"), "${separator}(${firstArgument}) > (${secondArgument})", 0),
            // this->GreaterThanZero(argument)
            // (argument) > 0
            (new Regex(@"(?<separator>\W)this->GreaterThanZero\((?<argument>((?<parenthesis>\()|(?<-parenthesis>\))|[^()]*)+)\)"), "${separator}(${argument}) > 0", 0),
            // template <typename TElement> class SizeBalancedTree : public SizeBalancedTreeMethods<TElement>
            // template <typename TElement, std::size_t N> class SizeBalancedTree : public Platform::Collections::Methods::Trees::SizeBalancedTreeMethods<TElement>
            (new Regex(@"template <typename TElement> class ([a-zA-Z0-9]+Tree) : public ([a-zA-Z0-9]+TreeMethods)<TElement>"), "template <typename TElement, std::size_t N> class $1 : public Platform::Collections::Methods::Trees::$2<TElement>", 0),
            // SizeBalancedTree(std::int32_t capacity) { (_elements, _allocated) = (new TreeElement[capacity], 1); }
            // SizeBalancedTree() { _allocated = 1; }
            (new Regex(@"([a-zA-Z0-9]+)\(std::int32_t capacity\) { \(_elements, _allocated\) = \(new TreeElement\[capacity\], 1\); }"), "$1() { _allocated = 1; }", 0),
            // void PrintNodeValue(TElement node, StringBuilder sb) override { sb.Append(node); }
            //
            (new Regex(@"[\r\n]{1,2}\s+[\r\n]{1,2}\s+[^\r\n]*PrintNodeValue\([^\(\r\n]+\) override {[^}\r\n]+}"), "", 0),
            // this->allocate(
            // allocate(
            (new Regex(@"this->(allocate|free|iszero)\("), "$1(", 0),
            // auto sizeBalancedTree = new SizeBalancedTree<uint>(10000);
            // SizeBalancedTree<uint, 10000> sizeBalancedTree;
            (new Regex(@"auto ([a-zA-Z0-9]+) = new ([a-zA-Z0-9]+)<([_a-zA-Z0-9:]+)>\(([0-9]+)\);"), "$2<$3, $4> $1;", 0),
            // &sizeBalancedTree->Root
            // &sizeBalancedTree.Root
            (new Regex(@"&([a-zA-Z0-9]+)->([a-zA-Z0-9]+)"), "&$1.$2", 0),
            // sizeBalancedTree.Count
            // sizeBalancedTree.GetCount()
            (new Regex(@"([a-zA-Z0-9]+)\.Count"), "$1.GetCount()", 0),
            // sizeBalancedTree.Allocate
            // [&]()-> auto { return sizeBalancedTree.Allocate(); }
            (new Regex(@"(\(|, )([a-zA-Z0-9]+)\.(Allocate)"), "$1[&]()-> auto { return $2.$3(); }", 0),
            // sizeBalancedTree.Free
            // [&](std::uint32_t link)-> auto { sizeBalancedTree.Free(link); }
            (new Regex(@"(\(|, )([a-zA-Z0-9]+)\.(Free)"), "$1[&](std::uint32_t link)-> auto { $2.$3(link); }", 0),
            // sizeBalancedTree.TestMultipleCreationsAndDeletions(
            // TestExtensions::TestMultipleCreationsAndDeletions(sizeBalancedTree, 
            (new Regex(@"([a-zA-Z0-9]+)\.(TestMultipleCreationsAndDeletions|TestMultipleRandomCreationsAndDeletions)\("), "TestExtensions::$2<std::uint32_t>($1, ", 0),
            // SizedBinaryTreeMethodsBase
            // Platform::Collections::Methods::Trees::SizedBinaryTreeMethodsBase
            (new Regex(@"\(SizedBinaryTreeMethodsBase<TElement>"), "(Platform::Collections::Methods::Trees::SizedBinaryTreeMethodsBase<TElement>&", 0),
        }.Cast<ISubstitutionRule>().ToList();

        public CustomCSharpToCppTransformer() : base(CustomRules) { }
    }
}

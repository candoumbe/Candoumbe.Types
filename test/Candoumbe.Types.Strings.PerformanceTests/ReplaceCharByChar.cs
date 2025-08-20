using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Candoumbe.Types.Strings;

namespace Candoumbe.Types.PerformanceTests;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
public class CharReplacementBenchmark
{
    private string InputString = "Hello World! This is a test string. Hello World!";
    private const char OldChar = 'o';
    private const char NewChar = '0';
    private StringBuilder _stringBuilder;
    private StringSegmentLinkedList _stringSegments;

    [Params(10, 100, 1000, 10000)]
    public int NodeCount {get; set;}

    [GlobalSetup]
    public void SetUp()
    {
        _stringBuilder = new StringBuilder(InputString);
        _stringSegments = new StringSegmentLinkedList(InputString);
        for (int i = 0; i < NodeCount; i++)
        {
            _stringBuilder.Append(InputString);
            _stringSegments.Append(InputString);
        }

        InputString = _stringBuilder.ToString();

    }

    [Benchmark(Baseline = true)]
    public string ReplaceUsingStringReplace() => InputString.Replace(OldChar, NewChar);

    [Benchmark]
    public string ReplaceUsingStringBuilder()
    {
        _stringBuilder.Replace(OldChar, NewChar);
        return _stringBuilder.ToString();
    }

    [Benchmark]
    public string NoReplacement() => _stringBuilder.ToString();

    [Benchmark]
    public string ReplacementWithStringSegmentLinkedList() => _stringSegments.Replace(OldChar, NewChar).ToStringValue();

}
using System.Buffers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;

namespace Candoumbe.Types.Strings.PerformanceTests;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class StringSegmentLinkedListBenchmarks
{
    private string[] _smallWords;
    private string _oneMbText;

    [Params(0.0, 0.01, 0.10, 0.50)]
    public double HitRate { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        Random rnd = new(42);
        _smallWords = Enumerable.Range(0, 10_000)
            .Select(_ => RandomWord(rnd, 5, 20))
            .ToArray();

        const int length = 1_048_576;
        char[] buffer = ArrayPool<char>.Shared.Rent(length);
        try
        {
            for (int i = 0; i < length; i++)
            {
                buffer[i] = rnd.NextDouble() < HitRate ? 'x' : 'y';
            }
            _oneMbText = new string(buffer, 0, length);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    private static string RandomWord(Random rnd, int min, int max)
    {
        int len = rnd.Next(min, max);
        char[] chars = new char[len];
        for (int i = 0; i < len; i++)
        {
            chars[i] = (char)('a' + rnd.Next(0, 26));
        }
        return new string(chars);
    }


    [Benchmark]
    [BenchmarkCategory( "Append10kSmall")]
    public int Append_10k_small_strings_total_length()
    {
        StringSegmentLinkedList list = new();
        foreach (string s in _smallWords)
        {
            list.Append(s);
        }
        return list.GetTotalLength();
    }

    [Benchmark]
    [BenchmarkCategory("ReplaceStringOn1MB")]
    public int Replace_char_to_char_on_1MB()
    {
        StringSegmentLinkedList list = new(_oneMbText);
        StringSegmentLinkedList replaced = list.Replace('x', 'z');
        return replaced.GetTotalLength();
    }

    [Benchmark]
    [BenchmarkCategory( "ReplaceStringToStringOn1MB")]
    public int Replace_string_to_string_on_1MB()
    {
        StringSegmentLinkedList list = new(_oneMbText);
        StringSegmentLinkedList replaced = list.Replace("xy", "zz");
        return replaced.GetTotalLength();
    }

    [Benchmark]
    [BenchmarkCategory("ToStringValue_Compact")]
    public int ToStringValue_before_after_Compact()
    {
        StringSegmentLinkedList list = new();
        foreach (string s in _smallWords)
        {
            list.Append(s);
        }
        string before = list.ToStringValue();
        list.Compact();
        string after = list.ToStringValue();
        return before.Length + after.Length;
    }
}
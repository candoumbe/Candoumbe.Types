using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Bogus;

namespace Candoumbe.Types.Strings.PerformanceTests;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
public class ConcatVsStringSegmentLinkedList
{
    [Params(10, 100, 1000)]
    public int WordCount { get; set; }

    private Faker _faker;
    private string[] _words;

    [GlobalSetup]
    public void SetUp()
    {
        _faker = new Faker();
        _words = _faker.Lorem.Words(WordCount);
    }


    [Benchmark(Baseline = true)]
    public string Concatenate_with_string_concat()
    {
        string result = _words.Aggregate(string.Empty, string.Concat);

        return result;
    }

    [Benchmark]
    public string Concatenate_with_StringBuilder()
    {
        StringBuilder result =  new ();

        result = _words.Aggregate(result, (current, word) => current.Append(word));

        return result.ToString();
    }


    [Benchmark]
    public string Concatenate_with_StringSegmentLinkedList()
    {
        StringSegmentLinkedList list = new(_words[0]);

        foreach (string word in _words.Skip(1))
        {
            list.Append(word);
        }

        return list.ToStringValue();
    }
}
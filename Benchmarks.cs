using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging.Abstractions;
using NumberChallenge.Runners;
using NumberChallenge.Strategies;

namespace NumberChallenge;

public class Benchmarks {
    public Runner Runner { get; set; } = default!;

    [Params(20)]
    public int Numbers { get; set; }

    [Params(1000)]
    public int Upper { get; set; }

    [GlobalSetup]
    public void Setup()
        => Runner = new Runner(NullLogger<Runner>.Instance,
            new UniformStrategy(
                NullLogger<UniformStrategy>.Instance),
                Numbers,
                Upper,
                new(69420));

    [Benchmark]
    public bool RunSimulation()
        => Runner.Run();
}

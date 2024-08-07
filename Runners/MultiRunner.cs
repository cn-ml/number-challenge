using Microsoft.Extensions.Logging;
using NumberChallenge.Strategies;

namespace NumberChallenge.Runners;

public class MultiRunner : IRunner {
    private readonly List<Runner> _runners;
    private readonly ILogger<MultiRunner> _logger;

    public MultiRunner(ILoggerFactory loggerFactory, int concurrency, IStrategy strategy, int count, int upper) {
        _logger = loggerFactory.CreateLogger<MultiRunner>();
        var runners = new List<Runner>(concurrency);
        for (int i = 0; i < concurrency; i++)
            runners.Add(new(loggerFactory.CreateLogger<Runner>(), strategy, count, upper, Random.Shared));
        _runners = runners;
    }

    public long Runs => _runners.Sum(i => i.Runs) + InitialRuns;
    public long Successes => _runners.Sum(i => i.Successes) + InitialSuccesses;

    public long InitialRuns { get; init; } = 0;
    public long InitialSuccesses { get; init; } = 0;

    public void Start() {
        _logger.LogInformation("Starting {amount} runners...", _runners.Count);
        _runners.ForEach(static i => i.Start());
        _logger.LogInformation("{amount} runners started!", _runners.Count);
    }
    public void Stop() {
        _logger.LogInformation("Stopping {amount} runners...", _runners.Count);
        _runners.ForEach(static i => i.Stop());
        _logger.LogInformation("{amount} runners stopped!", _runners.Count);
    }
}

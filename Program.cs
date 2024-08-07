using Microsoft.Extensions.Logging;
using NumberChallenge.Runners;
using NumberChallenge.Strategies;

var loggerFactory = LoggerFactory.Create(builder => builder.AddSimpleConsole());
var logger = loggerFactory.CreateLogger<Program>();

var strategy = new UniformStrategy(loggerFactory.CreateLogger<UniformStrategy>());
var runner = new MultiRunner(loggerFactory, Environment.ProcessorCount, strategy, 20, 1000);
var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
runner.Start();
var start = DateTime.UtcNow;
var lastRatio = -1L;
while (await timer.WaitForNextTickAsync()) {
    var now = DateTime.UtcNow;
    var spent = now - start;
    var successes = runner.Successes;
    var attempts = runner.Runs;
    var ratio = successes is 0 ? 0 : attempts / successes;
    var attemptsPerSecond = (int)Math.Round(attempts / spent.TotalSeconds);
    var converge = ratio == lastRatio;
    logger.LogInformation("{successes} / {attempts} = 1 in {ratio} {converging} ({speed} per second)", successes, attempts, ratio, converge ? "converged" : "not converged", attemptsPerSecond);
    lastRatio = ratio;
}
runner.Stop();

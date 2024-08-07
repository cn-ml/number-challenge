using Microsoft.Extensions.Logging;
using NumberChallenge.Runners;
using NumberChallenge.Strategies;

const int n = 20;
const int m = 1000;
var fileName = $"save_{n}_{m}.bin";

var loggerFactory = LoggerFactory.Create(builder => builder.AddSimpleConsole());
var logger = loggerFactory.CreateLogger<Program>();

var saveSuccesses = 0L;
var saveRuns = 0L;
var saveTimeSpent = TimeSpan.Zero;

try {
    using var save_file = new BinaryReader(File.OpenRead(fileName));
    saveSuccesses = save_file.ReadInt64();
    saveRuns = save_file.ReadInt64();
    saveTimeSpent = TimeSpan.FromTicks(save_file.ReadInt64());
    logger.LogInformation("Loaded save state: {successes} / {attempts}", saveSuccesses, saveRuns);
} catch (FileNotFoundException) {
} catch (Exception e) {
    logger.LogError(e, "Failed loading save state from {fileName}!", fileName);
    throw;
}

var strategy = new UniformStrategy(loggerFactory.CreateLogger<UniformStrategy>());
var runner = new MultiRunner(loggerFactory, Environment.ProcessorCount, strategy, n, m) {
    InitialSuccesses = saveSuccesses,
    InitialRuns = saveRuns,
};
var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
runner.Start();
var start = DateTime.UtcNow - saveTimeSpent;
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

    try {
        using var save_file = new BinaryWriter(File.OpenWrite(fileName));
        save_file.Write(successes);
        save_file.Write(attempts);
        save_file.Write(spent.Ticks);
    } catch (Exception e) {
        logger.LogError(e, "Failed saving state to {fileName}!", fileName);
    }
}
runner.Stop();

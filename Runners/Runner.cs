using Microsoft.Extensions.Logging;
using NumberChallenge.Strategies;

namespace NumberChallenge.Runners;

public class Runner(ILogger<Runner> logger, IStrategy strategy, int count, int upper, Random random) : IRunner {
    private readonly int?[] _nums = new int?[count];
    private long _successes = 0;
    private long _runs = 0;
    public long Runs => _runs;
    public long Successes => _successes;
    private readonly object _runSync = new();
    private readonly object _threadSync = new();

    private CancellableTask? _task;

    public void Start() {
        lock (_threadSync)
            _task = _task is null
                ? CancellableTask.Run(RunLoop)
                : throw new InvalidOperationException("Task is already running!");
        logger.LogInformation("Starting runner...");
    }

    public void Stop() {
        CancellableTask task;
        lock (_threadSync) {
            task = _task ?? throw new InvalidOperationException("Task is not running!");
            _task = null;
        }
        logger.LogInformation("Stopping runner...");
        task.Cancel();
    }

    private void RunLoop(CancellationToken cancellationToken) {
        logger.LogInformation("Run loop started!");
        lock (_runSync)
            while (!cancellationToken.IsCancellationRequested)
                RunCounted();
        logger.LogInformation("Run loop cancelled!");
    }

    public bool Run() {
        if (!Monitor.TryEnter(_runSync))
            throw new InvalidOperationException("Runner cannot be used concurrently!");
        logger.LogInformation("Running once!");
        try {
            return RunCounted();
        } finally {
            Monitor.Exit(_runSync);
        }
    }

    private bool RunCounted() {
        var result = RunCore();
        Interlocked.Increment(ref _runs);
        if (result)
            Interlocked.Increment(ref _successes);
        return result;
    }

    private bool RunCore() {
        Span<int?> nums = _nums;
        nums.Clear();
        for (var i = 0; i < count; i++) {
            var number = random.Next(0, upper);
            if (strategy.GetSlot(nums, number, upper) is not { } slot)
                return false;
            if (nums[slot].HasValue)
                throw new InvalidProgramException("Strategy selected a spot that was already picked!");
            nums[slot] = number;
        }
        return true;
    }
}

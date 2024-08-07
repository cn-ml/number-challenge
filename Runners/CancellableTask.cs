
namespace NumberChallenge.Runners;

public class CancellableTask(Task task, CancellationTokenSource cts) {
    public static CancellableTask Run(Action<CancellationToken> action) {
        var cts = new CancellationTokenSource();
        return new(Task.Run(() => action(cts.Token)), cts);
    }

    public TaskStatus Status => task.Status;

    public void Cancel() => cts.Cancel();
}

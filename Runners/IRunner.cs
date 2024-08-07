namespace NumberChallenge.Runners;

public interface IRunner {
    long Runs { get; }
    long Successes { get; }

    void Start();
    void Stop();
}

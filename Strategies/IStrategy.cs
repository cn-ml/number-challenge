namespace NumberChallenge.Strategies;

public interface IStrategy {
    int? GetSlot(ReadOnlySpan<int?> nums, int number, int upper);
}

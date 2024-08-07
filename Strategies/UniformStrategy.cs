using Microsoft.Extensions.Logging;

namespace NumberChallenge.Strategies;

public class UniformStrategy(
    ILogger<UniformStrategy> logger)
    : IStrategy {
    public int? GetSlot(ReadOnlySpan<int?> nums, int number, int upper) {
        var left = -1;
        var leftVal = 0;
        int rightVal;
        for (var right = 0; left < nums.Length; left = right++, leftVal = rightVal) {
            if (leftVal > number)
                return null;
            while (right < nums.Length && !nums[right].HasValue)
                right++;
            rightVal = right < nums.Length ? nums[right]!.Value : upper;
            if (rightVal < number)
                continue;
            left++; // was exclusive lower
            if (right - left is not (>= 1 and var spots))
                continue;
            var spot = ((rightVal - leftVal) is not 0 and var interval)
                ? (number - leftVal) * spots / (interval + 1) + left
                : left;
            return spot;
        }
        return null;
    }
}

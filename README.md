# Number Challenge

Calculates the probability of succeeding the number challenge.

## Details

Given an empty list of N numbers, you have to repeatedly insert a randomly generated number in the interval [0, M) into that list such that it stays sorted. You may not move the numbers you have already placed. If all spots in the list are filled, you have succeeded. If a number is selected which does not fit into your ordering, you have failed.

## Benchmarks

For the parameters N = 20 and M = 1000 I can reach about 1.8 million game simulations per second on my laptop (i7 8650U). The probability seems to converge at 1 : 8300.

# RangePermute

Memory efficient psuedo-randomly permuted enumerator for C#/.NET to enumerate gigantic ranges.

Say you want to generate a random permutation of numbers in range [0-2^50), without duplicates. Naive approach to get decent pseudo-random permutations would need prohibitively large amount of memory.
This library can generate such enumerations using constant memory, and is suitable for Linq-style consumption.

With UInt64, currently we can support 2^64. Can easily be modified to support range up-to 2^128. Internals are 128-bit already.

# Relevant reading: 
- Luby, Michael, and Charles Rackoff. "How to Construct Pseudorandom Permutations from Pseudorandom Functions." SIAM Journal on Computing, vol. 17, no. 2, 1988, pp. 373–386.
- Black, John, and Phillip Rogaway. "Ciphers with Arbitrary Finite Domains." The Cryptographers Track at the Rsa Conference, 2002. [http://web.cs.ucdavis.edu/~rogaway/papers/subset.pdf]

# Credits
Pretty much a C# port of https://pypi.python.org/pypi/shuffled/ .
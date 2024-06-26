﻿## IProvider
IProvider contains all the interfaces necessary to provide Actions to Jester.

Jester doesn't take Actions directly, instead he takes Entries.
These Entries contain metadata relating to how valuable they are, how many actions they are, and how they should enhance themselves if given the chance.

It is advised that each unique entry provided by a provider has a weight between 1 and 2.
For character-specific entries, it is advised to be between 4 and 8.

For upgrades, it is advised the total weight of upgrades it offers is between 1 and 2.

## IStrategy
IStrategy contains all the interfaces necessary to provide Strategies to Jester.
It requires IProvider to function!

At the moment there are three kinds of Strategy:
- Full: A strategy that is fully self-contained. Books' shard-costed cards, Dave cards, just anything that simply must be done alone to generate reasonably.
- Outer: Something that applies a mutation to a simple strategy. Costs like discard, making the card Exhaust, and even nothing are existing Outer strategies.
- Inner: A simple strategy. It doesn't typically try to pull anything.

## IUtil
IUtil contains various utilities that require nothing else.
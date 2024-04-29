// using System.Diagnostics;
// using Jester.Api;
// using Jester.Generator;
// using Jester.Generator.Provider;
// using Jester.Generator.Provider.Common;
//
// namespace Jester;
//
// public class JesterCLI
// {
//     private static bool generated = true;
//     
//     public static void Main()
//     {
//         if (generated) return;
//         
//         generated = true;
//         
//         var cards = new List<IJesterResult>();
//
//         var watch = Stopwatch.StartNew();
//         
//         for (var i = 0; i < 10000; i++)
//             cards.Add(JesterGenerator.GenerateCard(
//                 new JesterRequest
//                 {
//                     Seed = i,
//                     State = new State(),
//                     BasePoints = 60 + new Random(i).Next(0, 24),
//                     CardData = new CardData
//                     {
//                         cost = 3
//                     },
//                     Whitelist = new HashSet<string>
//                     {
//                         "attack",
//                         "shot"
//                     }
//                 }));
//         
//         watch.Stop();
//
//         var firstEntries = new Dictionary<string, int>();
//         var secondEntries = new Dictionary<string, int>();
//         var thirdEntryCount = 0;
//
//         var rawDamage = 0;
//         var pierceDamage = 0;
//
//         foreach (var card in cards)
//         {
//             var entries = card.Entries;
//
//             if (JesterGenerator.DebugMode)
//             {
//                 entries.RemoveAt(entries.Count - 1);
//                 entries.RemoveAt(0);
//             }
//
//             var first = entries[0].ToString() ?? "None";
//             string second;
//             if (entries.Count == 1)
//                 second = "None";
//             else
//                 second = entries[1].ToString() ?? "None";
//
//             if (firstEntries.TryGetValue(first, out var f))
//                 firstEntries[first] = f + 1;
//             else
//                 firstEntries[first] = 1;
//
//             if (secondEntries.TryGetValue(second, out var s))
//                 secondEntries[second] = s + 1;
//             else
//                 secondEntries[second] = 1;
//             
//             if (entries.Count >= 3)
//                 thirdEntryCount++;
//             
//             foreach (var entry in entries)
//             {
//                 if (entry is AttackProvider.AttackEntry e)
//                 {
//                     if (e.Piercing)
//                         pierceDamage += e.Damage;
//                     else
//                         rawDamage += e.Damage;
//                 }
//             }
//         }
//
//         var firstData = firstEntries.Keys
//             .Select(k => $"{k}: {firstEntries[k]}")
//             .Aggregate((a, b) => a + ", " + b);
//         
//         var secondData = secondEntries.Keys
//             .Select(k => $"{k}: {secondEntries[k]}")
//             .Aggregate((a, b) => a + ", " + b);
//         
//         Console.WriteLine(watch.ElapsedMilliseconds);
//         Console.WriteLine(firstData);
//         Console.WriteLine(secondData);
//         Console.WriteLine(thirdEntryCount);
//         Console.WriteLine(rawDamage);
//         Console.WriteLine(pierceDamage);
//     }
// }
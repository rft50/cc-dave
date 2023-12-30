using System.Diagnostics;
using Jester.Api;
using Jester.Generator;

namespace Jester;

public class JesterCLI
{
    private static bool generated = true;
    
    public static void Main()
    {
        if (generated) return;
        
        generated = true;
        
        var cards = new List<IJesterResult>();

        var watch = Stopwatch.StartNew();
        
        for (var i = 0; i < 10000; i++)
            cards.Add(JesterGenerator.GenerateCard(
                new JesterRequest
                {
                    Seed = i,
                    FirstAction = "attack",
                    State = new State(),
                    BasePoints = 20 + new Random(i).Next(0, 8),
                    CardData = new CardData
                    {
                        cost = 1
                    }
                }));
        
        watch.Stop();

        var firstEntries = new Dictionary<string, int>();
        var secondEntries = new Dictionary<string, int>();
        var thirdEntryCount = 0;

        foreach (var card in cards)
        {
            var entries = card.Entries;

            if (JesterGenerator.DebugMode)
            {
                entries.RemoveAt(entries.Count - 1);
                entries.RemoveAt(0);
            }

            var first = entries[0].ToString() ?? "None";
            string second;
            if (entries.Count == 1)
                second = "None";
            else
                second = entries[1].ToString() ?? "None";

            if (firstEntries.TryGetValue(first, out var f))
                firstEntries[first] = f + 1;
            else
                firstEntries[first] = 1;

            if (secondEntries.TryGetValue(second, out var s))
                secondEntries[second] = s + 1;
            else
                secondEntries[second] = 1;
            
            if (entries.Count >= 3)
                thirdEntryCount++;
        }

        var firstData = firstEntries.Keys
            .Select(k => $"{k}: {firstEntries[k]}")
            .Aggregate((a, b) => a + ", " + b);
        
        var secondData = secondEntries.Keys
            .Select(k => $"{k}: {secondEntries[k]}")
            .Aggregate((a, b) => a + ", " + b);
        
        Console.WriteLine(watch.ElapsedMilliseconds);
        Console.WriteLine(firstData);
        Console.WriteLine(secondData);
        Console.WriteLine(thirdEntryCount);
    }
}
using Jester.Artifacts;

namespace Jester.Actions;

public class FinalActAction : CardAction
{
    public Upgrade Upgrade;
    
    public override Route? BeginWithRoute(G g, State s, Combat c)
    {
        var cards = c.hand;

        if (cards.Count == 0) return null;
        
        var cardBrowse = new ArbitraryCardBrowse
        {
            mode = CardBrowse.Mode.Browse,
            browseAction = new FinalActSubAction
            {
                Upgrade = Upgrade
            },
            Cards = cards
        };

        timer = 0;

        return cardBrowse.GetCardList(g).Count != 0 ? cardBrowse : null;
    }

    public override List<Tooltip> GetTooltips(State s) => new()
    {
        new TTGlossary(ModManifest.OpeningFatigue.GlobalName)
    };
}

public class FinalActSubAction : CardAction
{
    public Upgrade Upgrade;
    
    public override void Begin(G g, State s, Combat c)
    {
        var card = selectedCard!;
        card.singleUseOverride = true;
        
        c.QueueImmediate(new ArbitraryCardPlayAction
        {
            Card = card,
            DoCardCheck = false
        });

        if (Upgrade == Upgrade.A)
        {
            c.QueueImmediate(new ArbitraryCardPlayAction
            {
                Card = card,
                DoCardCheck = false
            });
            c.QueueImmediate(new ArbitraryCardPlayAction
            {
                Card = card,
                DoCardCheck = false
            });
        }
        else if (Upgrade == Upgrade.B)
        {
            var ceremony = GetClosingCeremony(s, c);
            ceremony.Count++;
            ceremony.Pulse();
        }
    }

    public static ClosingCeremony GetClosingCeremony(State s, Combat c)
    {
        if (s.EnumerateAllArtifacts().Find(a => a is ClosingCeremony) is ClosingCeremony inInventory) return inInventory;

        var ceremony = new ClosingCeremony();
        c.QueueImmediate(new AAddArtifact
        {
            artifact = ceremony,
            artifactPulse = ceremony.Key()
        });

        return ceremony;
    }
}
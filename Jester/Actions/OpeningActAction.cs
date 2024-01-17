using Jester.Artifacts;

namespace Jester.Actions;

public class OpeningActAction : CardAction
{
    public bool Discounted;
    
    public override Route? BeginWithRoute(G g, State s, Combat c)
    {
        var script = GetOpeningScript(s, c);

        var scriptIds = script.CardData.Select(d => d.Item1).ToList();

        var cards = c.hand
            .Where(ca => !scriptIds.Contains(ca.uuid))
            .Where(ca => ca.singleUseOverride == false || ca.singleUseOverride == null && !ca.GetData(s).singleUse)
            .ToList();

        if (cards.Count == 0) return null;
        
        var cardBrowse = new ArbitraryCardBrowse
        {
            mode = CardBrowse.Mode.Browse,
            browseAction = new OpeningActSubAction
            {
                Discounted = Discounted
            },
            Cards = cards
        };

        timer = 0;

        return cardBrowse.GetCardList(g).Count != 0 ? cardBrowse : null;
    }

    public override List<Tooltip> GetTooltips(State s) => new()
    {
        new TTGlossary("status." + ModManifest.OpeningFatigue.GlobalName)
    };

    public static OpeningScript GetOpeningScript(State s, Combat c)
    {
        if (s.EnumerateAllArtifacts().Find(a => a is OpeningScript) is OpeningScript inInventory) return inInventory;

        var script = new OpeningScript();
        c.QueueImmediate(new AAddArtifact
        {
            artifact = script,
            artifactPulse = script.Key()
        });

        return script;
    }
}

public class OpeningActSubAction : CardAction
{
    public bool Discounted;
    
    public override void Begin(G g, State s, Combat c)
    {
        var script = OpeningActAction.GetOpeningScript(s, c);
        
        script.RegisterCard(selectedCard!, Discounted);

        artifactPulse = script.Key();
    }
}
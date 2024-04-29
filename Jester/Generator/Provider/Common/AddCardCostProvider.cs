using System.ComponentModel.DataAnnotations;
using Jester.Api;

namespace Jester.Generator.Provider.Common;

using IJesterRequest = IJesterApi.IJesterRequest;
using IEntry = IJesterApi.IEntry;
using IProvider = IJesterApi.IProvider;

public class AddCardCostProvider : IProvider
{
    public IEnumerable<(double, IEntry)> GetEntries(IJesterRequest request)
    {
        if (!request.Whitelist.Contains("cost")) return new List<(double, IEntry)>();
        
        var limit = request.CardData.cost >= 4 ? 2 : 1;

        return Enumerable.Range(1, limit)
            .SelectMany(
                i => new List<(double, IEntry)>
                {
                    (1.0/limit, new AddCardCostEntry
                    {
                        Card = new TrashFumes(),
                        Amount = i,
                        CostPer = -10
                    }),
                    (1.0/limit, new AddCardCostEntry
                    {
                        Card = new ColorlessTrash(),
                        Amount = i,
                        CostPer = -20
                    })
                }
            );
    }

    private class AddCardCostEntry : IEntry
    {
        [Required] public Card Card = null!;
        [Required] public int Amount;
        [Required] public int CostPer;

        public IReadOnlySet<string> Tags =>
            new HashSet<string>
            {
                "cost",
                "addCard"
            };


        public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new AAddCard
            {
                card = Card.CopyWithNewId(),
                amount = Amount,
                destination = CardDestination.Deck,
                insertRandomly = true
            }
        };

        public int GetCost() => Amount * CostPer;
        
        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            if (Amount <= 1) return new List<(double, IEntry)>();
            return new List<(double, IEntry)>
            {
                (1, new AddCardCostEntry
                {
                    Card = Card,
                    Amount = Amount - 1,
                    CostPer = CostPer
                })
            };
        }

        public void AfterSelection(IJesterRequest request)
        {
        }
    }
}
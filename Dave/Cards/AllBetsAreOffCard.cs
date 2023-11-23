namespace Dave.Cards
{
    // 2-cost, X=Red+Black Rigging, shoot for X, clear Red+Black Rigging, exhaust
    // A: don't clear Rigging
    // B: don't Exhaust
    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class AllBetsAreOffCard : Card
    {
        private static Spr card_sprite = Spr.cards_GoatDrone;

        public override List<CardAction> GetActions(State s, Combat c)
        {
            var redRigging = (Status)(ModManifest.red_rigging.Id ?? throw new Exception("missing status"));
            var blackRigging = (Status)(ModManifest.black_rigging.Id ?? throw new Exception("missing status"));

            var rigAmount = s.ship.Get(redRigging) + s.ship.Get(blackRigging);
            
            var actions = new List<CardAction>
            {
                new AVariableHint
                {
                    status = redRigging,
                    secondStatus = blackRigging
                },
                new AAttack { damage = this.GetDmg(s, rigAmount), xHint = 1 }
            };

            if (upgrade != Upgrade.A)
            {
                actions.Add(new AStatus
                {
                    status = redRigging,
                    statusAmount = 0,
                    targetPlayer = true,
                    mode = AStatusMode.Set
                });
                actions.Add(new AStatus
                {
                    status = blackRigging,
                    statusAmount = 0,
                    targetPlayer = true,
                    mode = AStatusMode.Set
                });
            }

            return actions;
        }

        public override CardData GetData(State state) => new()
        {
            cost = 2,
            art = card_sprite,
            exhaust = upgrade != Upgrade.B
        };
    }
}
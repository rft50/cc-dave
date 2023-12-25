namespace Jester.Generator.Provider;

public class StatusProvider : IProvider
{
    private const int CostOne = 40;
    private const int CostTwo = 60;
    private const int CostThree = 80;
    private const int CostFour = 100;
    
    private static List<StatusStruct> _stats = new()
    {
        new StatusStruct
        {
            Status = Enum.Parse<Status>("maxShield"),
            Tags = new HashSet<string>
            {
                "status",
                "defensive"
            },
            Cost = 20,
            Stackable = false
        },
        new StatusStruct
        {
            Status = Enum.Parse<Status>("payback"),
            Tags = new HashSet<string>
            {
                "status",
                "mustExhaust",
                "offensive"
            },
            Cost = CostFour,
            Stackable = false
        },
        new StatusStruct
        {
            Status = Enum.Parse<Status>("stunSource"),
            Tags = new HashSet<string>
            {
                "status",
                "mustExhaust",
                "defensive"
            },
            Cost = CostFour,
            Stackable = false
        },
        new StatusStruct
        {
            Status = Enum.Parse<Status>("ace"),
            Tags = new HashSet<string>
            {
                "status",
                "mustExhaust",
                "utility"
            },
            Cost = CostThree,
            Stackable = false
        },
        new StatusStruct
        {
            Status = Enum.Parse<Status>("hermes"),
            Tags = new HashSet<string>
            {
                "status",
                "utility"
            },
            Cost = 15,
            Stackable = false
        },
        new StatusStruct
        {
            Status = Enum.Parse<Status>("drawNextTurn"),
            Tags = new HashSet<string>
            {
                "status",
                "utility",
                "drawNext"
            },
            Cost = 20,
            Stackable = false
        },
        new StatusStruct
        {
            Status = Enum.Parse<Status>("energyNextTurn"),
            Tags = new HashSet<string>
            {
                "status",
                "utility",
                "energyNext"
            },
            Cost = 30,
            Stackable = false
        },
        new StatusStruct
        {
            Status = Enum.Parse<Status>("strafe"),
            Tags = new HashSet<string>
            {
                "status",
                "mustExhaust",
                "offensive"
            },
            Cost = CostFour,
            Stackable = false
        },
        new StatusStruct
        {
            Status = Enum.Parse<Status>("endlessMagazine"),
            Tags = new HashSet<string>
            {
                "status",
                "mustExhaust",
                "offensive"
            },
            Cost = CostTwo,
            Stackable = false
        },
        new StatusStruct
        {
            Status = Enum.Parse<Status>("powerdrive"),
            Tags = new HashSet<string>
            {
                "status",
                "mustExhaust",
                "offensive"
            },
            Cost = CostTwo,
            Stackable = false
        },
        new StatusStruct
        {
            Status = Enum.Parse<Status>("tableFlip"),
            Tags = new HashSet<string>
            {
                "status",
                "mustExhaust",
                "utility"
            },
            Cost = CostOne,
            Stackable = false
        },
        new StatusStruct
        {
            Status = Enum.Parse<Status>("cleanExhaust"),
            Tags = new HashSet<string>
            {
                "status",
                "mustExhaust",
                "utility"
            },
            Cost = CostFour,
            Stackable = false
        }
    };

    private readonly List<StatusEntry> _entries = new();
    
    public StatusProvider()
    {
        foreach (var stat in _stats)
        {
            for (var i = 1; i <= 3; i++)
            {
                _entries.Add(new StatusEntry(stat, i));
                
                if (!stat.Stackable)
                    break;
            }
        }
    }
    
    public List<IEntry> GetEntries(JesterRequest request)
    {
        var alreadyPresent = request.Entries
            .Where(e => e is StatusEntry)
            .Select(e => (e as StatusEntry)!.Data.Status);
        var isExhaust = request.Whitelist.Contains("mustExhaust") || request.CardData.exhaust;
        
        var minCost = request.MinCost;
        var maxCost = request.MaxCost;

        return _entries.Where(e => Util.InRange(minCost, e.GetCost(), maxCost))
            .Where(e => !alreadyPresent.Contains(e.Data.Status))
            .Where(e => isExhaust || !e.Tags.Contains("mustExhaust"))
            .ToList<IEntry>();
    }

    public struct StatusStruct
    {
        public Status Status;
        public HashSet<string> Tags;
        public int Cost;
        public bool Stackable;
    }

    public class StatusEntry : IEntry
    {
        public StatusStruct Data { get; }
        public int Amount { get; }

        public StatusEntry(StatusStruct data, int amount)
        {
            Data = data;
            Amount = amount;
        }

        public HashSet<string> Tags => Data.Tags;
        public int GetActionCount() => 1;

        public List<CardAction> GetActions(State s, Combat c) => new()
        {
            new AStatus
            {
                status = Data.Status,
                statusAmount = Amount,
                targetPlayer = true
            }
        };

        public int GetCost() => Amount * Data.Cost;

        public IEntry? GetUpgradeA(JesterRequest request, out int cost)
        {
            if (!Data.Stackable)
            {
                cost = 0;
                return null;
            }

            cost = Data.Cost;
            return new StatusEntry(Data, Amount + 1);
        }

        public IEntry? GetUpgradeB(JesterRequest request, out int cost)
        {
            cost = 0;
            return null;
        }

        public void AfterSelection(JesterRequest request)
        {
        }
    }
}
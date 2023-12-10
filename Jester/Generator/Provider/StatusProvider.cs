namespace Jester.Generator.Provider;

public class StatusProvider : IProvider
{
    private static List<StatusStruct> _stats = new()
    {
        new StatusStruct
        {
            Status = Enum.Parse<Status>("maxShield"),
            Tags = new HashSet<string>(),
            Cost = 20,
            Stackable = false
        },
        new StatusStruct
        {
            Status = Enum.Parse<Status>("payback"),
            Tags = new HashSet<string>
            {
                "mustExhaust"
            },
            Cost = 50,
            Stackable = false
        },
        new StatusStruct
        {
            Status = Enum.Parse<Status>("stunSource"),
            Tags = new HashSet<string>
            {
                "mustExhaust"
            },
            Cost = 80,
            Stackable = false
        },
        new StatusStruct
        {
            Status = Enum.Parse<Status>("ace"),
            Tags = new HashSet<string>
            {
                "mustExhaust"
            },
            Cost = 50,
            Stackable = false
        },
        new StatusStruct
        {
            Status = Enum.Parse<Status>("hermes"),
            Tags = new HashSet<string>(),
            Cost = 15,
            Stackable = false
        },
        new StatusStruct
        {
            Status = Enum.Parse<Status>("drawNextTurn"),
            Tags = new HashSet<string>(),
            Cost = 20,
            Stackable = false
        },
        new StatusStruct
        {
            Status = Enum.Parse<Status>("energyNextTurn"),
            Tags = new HashSet<string>(),
            Cost = 30,
            Stackable = false
        },
        new StatusStruct
        {
            Status = Enum.Parse<Status>("strafe"),
            Tags = new HashSet<string>
            {
                "mustExhaust"
            },
            Cost = 50,
            Stackable = false
        },
        new StatusStruct
        {
            Status = Enum.Parse<Status>("endlessMagazine"),
            Tags = new HashSet<string>
            {
                "mustExhaust"
            },
            Cost = 50,
            Stackable = false
        },
        new StatusStruct
        {
            Status = Enum.Parse<Status>("powerdrive"),
            Tags = new HashSet<string>
            {
                "mustExhaust"
            },
            Cost = 50,
            Stackable = false
        },
        new StatusStruct
        {
            Status = Enum.Parse<Status>("tableFlip"),
            Tags = new HashSet<string>
            {
                "mustExhaust"
            },
            Cost = 30,
            Stackable = false
        },
        new StatusStruct
        {
            Status = Enum.Parse<Status>("cleanExhaust"),
            Tags = new HashSet<string>
            {
                "mustExhaust"
            },
            Cost = 80,
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
                _entries.Add(new StatusEntry(this, stat, i));
                
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
        
        var minCost = request.MinCost;
        var maxCost = request.MaxCost;

        return _entries.Where(e => Util.InRange(minCost, e.GetCost(), maxCost))
            .Where(e => !alreadyPresent.Contains(e.Data.Status))
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

        public StatusEntry(IProvider provider, StatusStruct data, int amount)
        {
            Provider = provider;
            Data = data;
            Amount = amount;
        }

        public HashSet<string> Tags => Data.Tags;
        public IProvider Provider { get; }
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
            return new StatusEntry(Provider, Data, Amount + 1);
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
using System.ComponentModel.DataAnnotations;
using Jester.Api;

namespace Jester.Generator.Provider.Common;

using IJesterRequest = IJesterApi.IJesterRequest;
using IEntry = IJesterApi.IEntry;
using IProvider = IJesterApi.IProvider;

public class StatusProvider : IProvider
{
    private const int CostOne = 40;
    private const int CostTwo = 60;
    private const int CostThree = 80;
    private const int CostFour = 100;
    
    private static readonly List<StatusStruct> Stats = new()
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
            Stackable = true
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
            Cost = 10,
            Stackable = true
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
            Cost = 15,
            Stackable = true
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
            Cost = CostThree,
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
        foreach (var stat in Stats)
        {
            for (var i = 1; i <= 3; i++)
            {
                _entries.Add(new StatusEntry
                {
                    Data = stat,
                    Amount = i
                });
                
                if (!stat.Stackable)
                    break;
            }
        }
    }
    
    public IEnumerable<(double, IEntry)> GetEntries(IJesterRequest request)
    {
        var alreadyPresent = request.Entries
            .Where(e => e is StatusEntry)
            .Select(e => (e as StatusEntry)!.Data.Status);
        var isExhaust = ModManifest.JesterApi.HasCardFlag("exhaust", request);

        return _entries.Where(e => !alreadyPresent.Contains(e.Data.Status))
            .Where(e => isExhaust || !e.Tags.Contains("mustExhaust"))
            .Select(e => (0.5, e as IEntry));
    }

    public struct StatusStruct
    {
        public Status Status;
        public HashSet<string> Tags;
        public int Cost;
        public bool Stackable;
    }

    private class StatusEntry : IEntry
    {
        [Required] public StatusStruct Data { get; init; }
        [Required] public int Amount { get; init; }

        public IReadOnlySet<string> Tags => Data.Tags;

        public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new AStatus
            {
                status = Data.Status,
                statusAmount = Amount,
                targetPlayer = true
            }
        };

        public int GetCost() => Amount * Data.Cost;

        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            if (!Data.Stackable) return new List<(double, IEntry)>();
            return new List<(double, IEntry)>
            {
                (1, new StatusEntry
                {
                    Data = Data,
                    Amount = Amount + 1
                })
            };
        }

        public void AfterSelection(IJesterRequest request)
        {
        }
    }
}
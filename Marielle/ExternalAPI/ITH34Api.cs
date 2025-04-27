using Nickel;

namespace Marielle.ExternalAPI;

public interface ITH34Api
{
	IDeckEntry TH34_Deck { get; }
    IStatusEntry MinusChargeStatus { get; }
    IStatusEntry PlusChargeStatus { get; }
    IStatusEntry RefractoryStatus { get; }
    IStatusEntry OptimizeStatus { get; }
    IStatusEntry OptimizeBStatus { get; }
}
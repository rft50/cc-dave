using Nickel;

namespace Marielle;

public interface IMarielleApi
{
    public IDeckEntry MarielleDeck();
    
    public IStatusEntry Curse();
    
    public IStatusEntry Enflamed();

    public ICardTraitEntry Fleeting();
}
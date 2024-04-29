namespace Jester.External;

public interface IMoreDifficultiesApi
{
	void RegisterAltStarters(Deck deck, StarterDeck starterDeck);
    bool HasAltStarters(Deck deck);
	public StarterDeck? GetAltStarters(Deck deck);
    bool AreAltStartersEnabled(State state, Deck deck);
}

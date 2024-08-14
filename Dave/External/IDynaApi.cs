using Nickel;

namespace Dave.External;

public interface IDynaApi
{
	IDeckEntry DynaDeck { get; }

	void RegisterHook(IDynaHook hook, double priority);
}

public interface IDynaHook
{
	void OnBlastwaveTrigger(State state, Combat combat, Ship ship, int worldX, bool hitMidrow) { }
	void OnBlastwaveHit(State state, Combat combat, Ship ship, int originWorldX, int waveWorldX, bool hitMidrow) { }
	int ModifyBlastwaveDamage(Card? card, State state, bool targetPlayer, int blastwaveIndex) => 0;

	void OnChargeFired(State state, Combat combat, Ship targetShip, int worldX) { }
	void OnChargeSticked(State state, Combat combat, Ship ship, int worldX) { }
	void OnChargeTrigger(State state, Combat combat, Ship ship, int worldX) { }
}
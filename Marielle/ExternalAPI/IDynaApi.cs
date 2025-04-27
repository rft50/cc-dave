using Nickel;

namespace Marielle.ExternalAPI;

public interface IDynaApi
{
	IDeckEntry DynaDeck { get; }

	IStatusEntry TempNitroStatus { get; }
	IStatusEntry NitroStatus { get; }
	IStatusEntry BastionStatus { get; }

	PDamMod FluxDamageModifier { get; }

	int GetBlastwaveDamage(Card? card, State state, int baseDamage, bool targetPlayer = false, int blastwaveIndex = 0);

	bool IsBlastwave(AAttack attack);
	bool IsStunwave(AAttack attack);
	int? GetBlastwaveDamage(AAttack attack);
	int GetBlastwaveRange(AAttack attack);
	AAttack SetBlastwave(AAttack attack, int? damage, int range = 1, bool isStunwave = false);
}
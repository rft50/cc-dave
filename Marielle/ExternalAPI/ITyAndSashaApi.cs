using System;
using System.Collections.Generic;
using Nickel;

#nullable enable
namespace Marielle.ExternalAPI;

public interface ITyAndSashaApi
{
	int GetXBonus(Card card, List<CardAction> actions, State s, Combat c);

	int CountWildsInHand(State s, Combat c);

	void RegisterHook(IHook hook, double priority = 0);
	void UnregisterHook(IHook hook);
	

	Deck TyDeck { get; }
	Status PredationStatus { get; }
	Status XFactorStatus { get; }
	Status ExtremeMeasuresStatus { get; }

	ICardTraitEntry WildTrait { get; }

	public interface IHook
	{
		// Returning true means you skip future hooks
		bool ApplyXBonus(IApplyXBonusArgs args) => false;

		public interface IApplyXBonusArgs
		{
			State State { get; }
			CardAction Action { get; }
			int Bonus { get; }
		}

		int AffectX(IAffectXArgs args) => 0;

		public interface IAffectXArgs
		{
			State State { get; }
			Combat Combat { get; }
			Card Card { get; }
			List<CardAction> Actions { get; }
			int Amount { get; set; }
		}
	}
}

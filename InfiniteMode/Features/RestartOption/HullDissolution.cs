using System;
using System.Collections.Generic;

namespace InfiniteMode.Features.RestartOption;

public class HullDissolution : IRestartOption
{
    public bool Selectable(State s) => s.ship.hullMax >= 9;

    public float GetWeight(State s) => 1;

    public List<CardAction> GetSingleActions(State s) =>
    [
        new AHullMax
        {
            amount = -4,
            targetPlayer = true
        },
        new AHurt
        {
            hurtAmount = GetSingleDamage(s),
            targetPlayer = true
        }.WithDescription(ModEntry.Instance.Localizations.Localize(["restart", "option", "hull", "line"], new {Hull = GetSingleDamage(s)}))
    ];

    public List<CardAction> GetDoubleActions(State s) =>
    [
        new AHullMax
        {
            amount = -8,
            targetPlayer = true
        },
        new AHurt
        {
            hurtAmount = GetDoubleDamage(s),
            targetPlayer = true
        }.WithDescription(ModEntry.Instance.Localizations.Localize(["restart", "option", "hull", "line"], new {Hull = GetDoubleDamage(s)}))
    ];

    public string GetSingleDescription(State s) =>
        ModEntry.Instance.Localizations.Localize(["restart", "option", "hull", "desc"], new {MH = 4, Hull = GetSingleDamage(s)});

    public string GetDoubleDescription(State s) =>
        ModEntry.Instance.Localizations.Localize(["restart", "option", "hull", "desc"], new {MH = 8, Hull = GetDoubleDamage(s)});

    private int GetSingleDamage(State s) => Math.Min(s.ship.hullMax - 4, s.ship.hull) / 3;
    private int GetDoubleDamage(State s) => Math.Min(s.ship.hullMax - 8, s.ship.hull) * 2 / 3;
}
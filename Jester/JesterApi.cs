﻿using Jester.Api;
using Jester.Generator;
using Jester.Generator.Strategy;

namespace Jester;

public class JesterApi : IJesterApi
{
    public void RegisterStrategy(IStrategy strategy) => JesterGenerator.Strategies.Add(strategy);

    public IJesterRequest NewJesterRequest() => new JesterRequest();

    public IJesterResult NewJesterResult() => new JesterResult();

    public IList<IEntry> GetOptionsFromProviders(IJesterRequest request, IEnumerable<IProvider> providers) => StrategyUtil.GetOptionsFromProviders(request, providers);

    public IList<IEntry> GetOptionsFromProvidersFiltered(IJesterRequest request, IEnumerable<IProvider> providers) =>
        StrategyUtil.GetOptionsFromProvidersFiltered(request, providers);

    public int PerformUpgradeA(IJesterRequest request, IList<IEntry> entries, ref int pts) =>
        StrategyUtil.PerformUpgradeA(request, entries, ref pts);

    public IJesterResult CallInnerStrategy(IJesterRequest request, IList<IProvider> providers, int maxActions) =>
        JesterGenerator.CallInnerStrategy(request, providers, maxActions);

    public void RegisterProvider(IProvider provider) => JesterGenerator.Providers.Add(provider);
    
    private readonly JesterUtil _util = new();
    public IJesterApi.IJesterUtil GetJesterUtil() => _util;
    
    private class JesterUtil : IJesterApi.IJesterUtil
    {
        public bool InRange(int min, int val, int max) => Util.InRange(min, val, max);

        public bool ContainsAll<T>(IEnumerable<T> source, IEnumerable<T> mustContain) =>
            Util.ContainsAll(source, mustContain);

        public T GetRandom<T>(IList<T> source, Random rng) => Util.GetRandom(source, rng);

        public IList<int> GetDeployOptions(ISet<int> occupied, int offset = 0, int skip = 0) =>
            Util.GetDeployOptions(occupied, offset, skip);

        public void Shuffle<T>(IList<T> list, Random rng) => Util.Shuffle(list, rng);
    }

    private readonly Dictionary<string, Func<IJesterRequest, bool>> _cardFlagCalculators = new();

    public bool HasCardFlag(string flag, IJesterRequest request)
    {
        return _cardFlagCalculators.TryGetValue(flag, out var calc) && calc.Invoke(request);
    }

    public void RegisterCardFlag(string flag, Func<IJesterRequest, bool> calculator)
    {
        _cardFlagCalculators[flag] = calculator;
    }

    private readonly Dictionary<string, List<Deck>> _characterFlags = new();

    public bool HasCharacterFlag(string flag, State s)
    {
        return _characterFlags.TryGetValue(flag, out var list) && s.characters.Any(c => list.Contains(c.deckType.GetValueOrDefault()));
    }

    public void RegisterCharacterFlag(string flag, Deck deck)
    {
        if (!_characterFlags.ContainsKey(flag))
            _characterFlags[flag] = new List<Deck>();
        _characterFlags[flag].Add(deck);
    }
}


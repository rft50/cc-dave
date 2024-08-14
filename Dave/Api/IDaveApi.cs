namespace Dave.Api;

public interface IDaveApi
{
    public interface IRollHook
    {
        void OnRoll(State state, Combat combat, bool isRed, bool isBlack, bool isRoll);
    }

    public interface IRollModifier
    {
        // red, black; if null, proceed to next hook
        // rigging is processed at priority 0, positive prio to proc before that, negative prio to proc after
        (bool, bool)? ModifyRoll(State state, Combat combat);
    }
}
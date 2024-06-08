namespace Dave.Api;

public interface IRollHook
{
    void OnRoll(State state, Combat combat, bool isRed, bool isBlack, bool isRoll);
}
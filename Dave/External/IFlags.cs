﻿namespace Dave.External;

public partial interface IJesterApi
{
    public bool HasCardFlag(string flag, IJesterRequest request);

    public void RegisterCardFlag(string flag, Func<IJesterRequest, bool> calculator);

    public bool HasCharacterFlag(string flag);

    public void RegisterCharacterFlag(string flag, Deck deck);
}
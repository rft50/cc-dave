using Jester.External;
using Newtonsoft.Json;
using Nickel;
using Shockah.Shared;

namespace Jester;

internal sealed class Settings
{
	[JsonProperty]
	public ProfileSettings Global = new();

	[JsonIgnore]
	public ProfileBasedValue<IModSettingsApi.ProfileMode, ProfileSettings> ProfileBased;

	public Settings()
	{
		ProfileBased = ProfileBasedValue.Create(
			() => ModManifest.Helper.ModData.GetModDataOrDefault(GExt.Instance?.state ?? DB.fakeState, "ActiveProfile", IModSettingsApi.ProfileMode.Slot),
			profile => ModManifest.Helper.ModData.SetModData(GExt.Instance?.state ?? DB.fakeState, "ActiveProfile", profile),
			profile => profile switch
			{
				IModSettingsApi.ProfileMode.Global => Global,
				IModSettingsApi.ProfileMode.Slot => ModManifest.Helper.ModData.ObtainModData<ProfileSettings>(GExt.Instance?.state ?? DB.fakeState, "ProfileSettings"),
				_ => throw new ArgumentOutOfRangeException(nameof(profile), profile, null)
			},
			(profile, data) =>
			{
				switch (profile)
				{
					case IModSettingsApi.ProfileMode.Global:
						Global = data;
						break;
					case IModSettingsApi.ProfileMode.Slot:
						ModManifest.Helper.ModData.SetModData(GExt.Instance?.state ?? DB.fakeState, "ProfileSettings", data);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		);
	}
}

internal sealed class ProfileSettings
{
	public bool InsaneMode = false;
	public int ActionCap = 5;
}
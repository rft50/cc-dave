using Nanoray.PluginManager;
using Nickel;

namespace InfiniteMode;
internal interface IRegisterable
{
    static abstract void Register(IPluginPackage<IModManifest> package, IModHelper helper);
}
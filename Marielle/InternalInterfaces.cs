using Nanoray.PluginManager;
using Nickel;

namespace Marielle;
internal interface IRegisterable
{
    static abstract void Register(IPluginPackage<IModManifest> package, IModHelper helper);
}
﻿namespace CobaltCoreModding.Definitions.ModManifests
{
    /// <summary>
    /// Mods contain manifests, which the mod loader uses to have their data setup.
    /// this is the parent containing base defentions of manifest such as global name.
    /// </summary>
    public interface IManifest
    {
        /// <summary>
        /// The unique modifier of this manifest. must be unique within and across all assemblies for mods.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Will be set by the mod loader to help a manifest find its physical ressources.
        /// </summary>
        public DirectoryInfo? ModRootFolder { get; set; }
    }
}
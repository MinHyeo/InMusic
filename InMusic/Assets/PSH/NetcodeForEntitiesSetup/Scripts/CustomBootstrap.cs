using System;
using Unity.NetCode;

namespace Unity.Multiplayer.Center.NetcodeForEntitiesSetup
{
    /// <summary>
    /// Custom bootstrap for the NetCode example. You can modify this to fit your needs.
    /// </summary> 
    [UnityEngine.Scripting.Preserve]
    public class NetCodeBootstrap : ClientServerBootstrap
    {
        public override bool Initialize(string defaultWorldName)
        {
            // If the user added an OverrideDefaultNetcodeBootstrap MonoBehaviour to their active scene,
            // or toggle Bootstrapping project-wide via NetCodeConfig, that is discovered here, and respected.
            if (IsBootstrappingEnabledForScene())
            {
                AutoConnectPort = 7979;
                CreateDefaultClientServerWorlds();
                return true;
            }

            // For UI scenes, we can simply wait until the user clicks on a button to connect.
            AutoConnectPort = 0;
            return false;
        }

        /// <summary>
        /// Automatically discovers whether or not there is an <see cref="OverrideAutomaticNetcodeBootstrap" /> present
        /// in the active scene, and if there is, uses its value to clobber the default.
        /// Otherwise, falls back to the <see cref="NetCodeConfig.Global"/> value (if present).
        /// Otherwise, falls back on the project default (of FALSE).
        /// </summary>
        public static bool IsBootstrappingEnabledForScene()
        {
            // This sample disables Netcode for Entities automatic game-scene bootstrapping by default,
            // so that your non-N4E scenes do not attempt to create netcode worlds.
            const NetCodeConfig.AutomaticBootstrapSetting projectDefault = NetCodeConfig.AutomaticBootstrapSetting.DisableAutomaticBootstrap;
            
            // Find in-scene overrides:
            var automaticNetcodeBootstrap = DiscoverAutomaticNetcodeBootstrap(logNonErrors: true);

            // And any NetcodeConfig overrides:
            var automaticBootstrapSettingValue = automaticNetcodeBootstrap
                ? automaticNetcodeBootstrap.ForceAutomaticBootstrapInScene
                : (NetCodeConfig.Global ? NetCodeConfig.Global.EnableClientServerBootstrap : projectDefault);
            
            return automaticBootstrapSettingValue == NetCodeConfig.AutomaticBootstrapSetting.EnableAutomaticBootstrap;
        }
    }
}
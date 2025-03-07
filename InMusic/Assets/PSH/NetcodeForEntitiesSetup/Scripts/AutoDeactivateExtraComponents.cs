using UnityEngine;

namespace Unity.Multiplayer.Center.NetcodeForEntitiesSetup
{
    public class AutoDeactivateExtraComponents : MonoBehaviour
    {
        void Awake()
        {
            // Deactivate extra AudioListeners
            var audioListeners = Object.FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
            for (var i = 1; i < audioListeners.Length; i++)
            {
                audioListeners[i].enabled = false;
            }

            // Deactivate extra Directional Lights
            var directionalLights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
            var directionalLightCount = 0;
            foreach (var light in directionalLights)
            {
                if (light.type == LightType.Directional)
                {
                    directionalLightCount++;
                    if (directionalLightCount > 1)
                    {
                        light.enabled = false;
                    }
                }
            }
        }
    }
}

using Peg.MessageDispatcher;
using UnityEngine;


namespace Peg.Graphics
{
    /// <summary>
    /// Handles incoming messages to set the scene's global lighting and fog.
    /// </summary>
    public class SceneLighting : MonoBehaviour
    {
        public void OnEnable()
        {
            GlobalMessagePump.Instance.AddListener<SceneLightingCmd>(HandleMsg);
        }

        public void OnDisable()
        {
            GlobalMessagePump.Instance.RemoveListener<SceneLightingCmd>(HandleMsg);
        }

        void HandleMsg(SceneLightingCmd cmd)
        {
            if ( ((int)cmd.Flags & (int)SceneLightingCmd.StateFlags.AmbientColor) != 0 )
                RenderSettings.ambientLight = cmd.AmbientColor;
            if (((int)cmd.Flags & (int)SceneLightingCmd.StateFlags.FogColor) != 0)
                RenderSettings.fogColor = cmd.FogColor;
            if (((int)cmd.Flags & (int)SceneLightingCmd.StateFlags.FogEnabled) != 0)
                RenderSettings.fog = cmd.FogEnabled;
            if (((int)cmd.Flags & (int)SceneLightingCmd.StateFlags.FogMode) != 0)
                RenderSettings.fogMode = cmd.FogMode;
            if (((int)cmd.Flags & (int)SceneLightingCmd.StateFlags.AmbientIntensity) != 0)
                RenderSettings.ambientIntensity = cmd.AmbientIntensity;
            if (((int)cmd.Flags & (int)SceneLightingCmd.StateFlags.FogDensity) != 0)
                RenderSettings.fogDensity = cmd.FogDensity;
        }
    }


    /// <summary>
    /// Command that can be used to control global lighting and fog settings for the scene.
    /// </summary>
    public class SceneLightingCmd : IMessageCommand
    {
        public Color AmbientColor { get; private set; }
        public float AmbientIntensity { get; private set; }
        public Color FogColor { get; private set; }
        public bool FogEnabled { get; private set; }
        public FogMode FogMode { get; private set; }
        public float FogDensity { get; private set; }

        public StateFlags Flags { get; private set; }

        public enum StateFlags
        {
            AmbientColor        = 1,
            FogColor            = 1 << 2,
            FogEnabled          = 1 << 3,
            FogMode             = 1 << 4,
            AmbientIntensity    = 1 << 5,
            FogDensity          = 1 << 6,

            All = AmbientColor | FogColor | FogEnabled | FogMode | AmbientIntensity,
            Ambient = AmbientColor | AmbientIntensity,
            Colors = AmbientColor | FogColor,
            Fog = FogEnabled | FogColor | FogMode | FogDensity,
        }


        public SceneLightingCmd(Color ambientLight, float ambientIntensity, bool enableFog, Color fogColor, FogMode fogMode, float fogDensity)
        {
            Flags = StateFlags.All;
            AmbientColor = ambientLight;
            AmbientIntensity = ambientIntensity;
            FogEnabled = enableFog;
            FogColor = fogColor;
            FogMode = fogMode;
            FogDensity = fogDensity;
        }

        public SceneLightingCmd(Color ambientLight, float ambientIntensity)
        {
            Flags = StateFlags.Ambient;
            AmbientColor = ambientLight;
            AmbientIntensity = ambientIntensity;
        }

        public SceneLightingCmd(Color ambientLight, Color fogColor)
        {
            Flags = StateFlags.Colors;
            AmbientColor = ambientLight;
            FogColor = fogColor;
        }

        public SceneLightingCmd(Color ambientLight)
        {
            Flags = StateFlags.AmbientColor;
            AmbientColor = ambientLight;
        }

        public SceneLightingCmd(bool enableFog, Color fogColor)
        {
            Flags = StateFlags.FogColor | StateFlags.FogEnabled;
            FogEnabled = enableFog;
            FogColor = fogColor;
        }

        public SceneLightingCmd(bool enableFog, Color fogColor, FogMode fogMode, float fogDensity)
        {
            Flags = StateFlags.Fog;
            FogEnabled = enableFog;
            FogColor = fogColor;
            FogMode = fogMode;
            FogDensity = fogDensity;
        }
    }
}

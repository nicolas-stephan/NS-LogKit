using UnityEngine;

namespace NS.LogKit {
    internal static class LogSettingsLoader {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemInit() => LogKitSettings.LoadAndApply();
    }
}
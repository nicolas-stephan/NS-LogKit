using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NS.LogKit {
    [CreateAssetMenu(menuName = "NS/LogKit Settings", fileName = "LogKitSettings")]
    public sealed class LogKitSettings : ScriptableObject {
        const string ResourcesPath = "LogKitSettings";
        internal const string SettingsPath = "Assets/Resources/LogKitSettings.asset";

        [Header("Global"), Tooltip("Master switch — disables ALL logs when false."), SerializeField]
        private bool globalEnabled = true;

        [Header("Channels"), SerializeField] private List<ChannelConfig> channels = new();

        public bool GlobalEnabled => globalEnabled;
        public IReadOnlyList<ChannelConfig> Channels => channels;

        public static void LoadAndApply() {
            var settings = Resources.Load<LogKitSettings>(ResourcesPath);
            if (settings == null) return;

            settings.Apply();
        }

        private void Apply() {
            if (!globalEnabled) {
                LogChannelRegistry.SetAllEnabled(false);
                return;
            }

            foreach (var cfg in channels) {
                if (string.IsNullOrWhiteSpace(cfg.Name)) continue;

                var channel = LogChannelRegistry.GetOrCreate(cfg.Name);
                channel.IsEnabled = cfg.IsEnabled;
                channel.MinLevel = cfg.MinLevel;
            }
        }

        [Serializable]
        public sealed class ChannelConfig {
            [Tooltip("Channel name as passed to PackageLogger.For(...)"), SerializeField]
            private string name = string.Empty;

            [Tooltip("Whether this channel emits any logs at all"), SerializeField]
            private bool enabled = true;

            [Tooltip("Minimum level that will be logged"), SerializeField]
            private LogLevel minLevel = LogLevel.Info;

            public ChannelConfig() { }

            public ChannelConfig(string name, bool enabled = true, LogLevel minLevel = LogLevel.Info) : this() {
                this.name = name;
                this.enabled = enabled;
                this.minLevel = minLevel;
            }

            public string Name => name;
            public bool IsEnabled => enabled;
            public LogLevel MinLevel => minLevel;
        }

#if UNITY_EDITOR
        public void SyncWithRegistry() {
            var all = LogChannelRegistry.GetAll();
            channels.Clear();
            foreach (var channel in all)
                channels.Add(new ChannelConfig(channel.Name, channel.IsEnabled, channel.MinLevel));
            EditorUtility.SetDirty(this);
        }

        private bool HasEntry(string inName) {
            foreach (var c in channels)
                if (c.Name == inName)
                    return true;

            return false;
        }
#endif
    }
}
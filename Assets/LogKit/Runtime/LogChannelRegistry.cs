using System;
using System.Collections.Generic;

namespace NS.LogKit {
    public static class LogChannelRegistry {
        private static readonly Dictionary<string, LogChannel> Channels = new(StringComparer.Ordinal);
        private static readonly object Lock = new();

        public static event Action<LogChannel>? OnChannelRegistered;

        public static LogChannel GetOrCreate(string name) {
            lock (Lock) {
                if (Channels.TryGetValue(name, out var channel))
                    return channel;

                channel = new LogChannel(name);
                Channels[name] = channel;
                OnChannelRegistered?.Invoke(channel);

                return channel;
            }
        }

        public static IReadOnlyList<LogChannel> GetAll() {
            lock (Lock)
                return new List<LogChannel>(Channels.Values);
        }

        public static void SetAllEnabled(bool enabled) {
            lock (Lock)
                foreach (var channel in Channels.Values)
                    channel.IsEnabled = enabled;
        }

        public static void SetGroupEnabled(string prefix, bool enabled) {
            if (string.IsNullOrEmpty(prefix)) return;

            lock (Lock)
                foreach (var kvp in Channels)
                    if (kvp.Key.StartsWith(prefix, StringComparison.Ordinal))
                        kvp.Value.IsEnabled = enabled;
        }

        public static void SetGroupMinLevel(string prefix, LogLevel level) {
            if (string.IsNullOrEmpty(prefix)) return;

            lock (Lock)
                foreach (var kvp in Channels)
                    if (kvp.Key.StartsWith(prefix, StringComparison.Ordinal))
                        kvp.Value.MinLevel = level;
        }
    }
}
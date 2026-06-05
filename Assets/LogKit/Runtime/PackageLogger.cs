using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace NS.LogKit {
    /// <summary>
    ///     A zero-overhead, channel-based logger for Unity packages.
    ///     All methods are stripped from non-development builds via [Conditional].
    ///     Usage:
    ///     private static readonly PackageLogger Log = PackageLogger.For("MyPackage.Physics");
    ///     Log.Info("Simulation started");
    ///     Log.Warn("High iteration count: {0}", iterCount);
    /// </summary>
    public sealed class PackageLogger {
        private const string ConditionalLog = "ENABLE_LOGS";
        private const string ConditionalVerbose = "ENABLE_VERBOSE_LOG";

        private readonly string _channelName;
        private readonly string _prefix;

        private LogChannel _channel;

        private PackageLogger(string channelName) {
            _channelName = channelName;
            _prefix = $"[{channelName}] ";
            _channel = LogChannelRegistry.GetOrCreate(channelName);
        }


        public static PackageLogger For(string channelName) => string.IsNullOrWhiteSpace(channelName)
            ? throw new ArgumentException("Channel name cannot be null or empty.", nameof(channelName))
            : new PackageLogger(channelName);

        private bool IsEnabled(LogLevel level) {
            _channel = LogChannelRegistry.GetOrCreate(_channelName);
            return _channel.IsEnabled && _channel.MinLevel <= level;
        }

        private string BuildMessage(string message) => string.Concat(_prefix, message);

        #region Log API

        [Conditional(ConditionalLog)]
        public void Info(string message, Object? context = null) {
            if (!IsEnabled(LogLevel.Info)) return;

            Debug.Log(BuildMessage(message), context);
        }

        [Conditional(ConditionalLog)]
        public void Info(string format, params object[] args) {
            if (!IsEnabled(LogLevel.Info)) return;

            Debug.Log(BuildMessage(string.Format(format, args)));
        }

        [Conditional(ConditionalLog)]
        public void Warn(string message, Object? context = null) {
            if (!IsEnabled(LogLevel.Warning)) return;

            Debug.LogWarning(BuildMessage(message), context);
        }

        [Conditional(ConditionalLog)]
        public void Warn(string format, params object[] args) {
            if (!IsEnabled(LogLevel.Warning)) return;

            Debug.LogWarning(BuildMessage(string.Format(format, args)));
        }

        [Conditional(ConditionalLog)]
        public void Error(string message, Object? context = null) {
            if (!IsEnabled(LogLevel.Error)) return;

            Debug.LogError(BuildMessage(message), context);
        }

        [Conditional(ConditionalLog)]
        public void Error(string format, params object[] args) {
            if (!IsEnabled(LogLevel.Error)) return;

            Debug.LogError(BuildMessage(string.Format(format, args)));
        }

        [Conditional(ConditionalLog)]
        public void Exception(Exception ex, Object? context = null) {
            if (!IsEnabled(LogLevel.Error)) return;

            Debug.LogException(ex, context);
        }

        [Conditional(ConditionalLog), Conditional(ConditionalVerbose)]
        public void Verbose(string message, Object? context = null) {
            if (!IsEnabled(LogLevel.Verbose)) return;

            Debug.Log(BuildMessage($"[VERBOSE] {message}"), context);
        }

        [Conditional(ConditionalLog), Conditional(ConditionalVerbose)]
        public void Verbose(string format, params object[] args) {
            if (!IsEnabled(LogLevel.Verbose)) return;

            Debug.Log(BuildMessage($"[VERBOSE] {string.Format(format, args)}"));
        }

        #endregion
    }
}
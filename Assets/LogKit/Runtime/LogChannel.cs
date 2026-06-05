namespace NS.LogKit {
    public enum LogLevel {
        Verbose = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
        Off = 99
    }

    public sealed class LogChannel {
        internal LogChannel(string name) {
            Name = name;
        }

        public string Name { get; }
        public bool IsEnabled { get; set; } = true;
        public LogLevel MinLevel { get; set; } = LogLevel.Info;

        public void Disable() {
            IsEnabled = false;
        }

        public void Enable(LogLevel minLevel = LogLevel.Info) {
            IsEnabled = true;
            MinLevel = minLevel;
        }
    }
}
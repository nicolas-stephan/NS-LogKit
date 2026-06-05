using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace NS.LogKit.Editor {
    public sealed class LogKitSettingsWindow : EditorWindow {
        private Vector2 _scroll;

        private bool _showDefineSymbols;

        private void OnEnable() {
            LogChannelRegistry.OnChannelRegistered += OnChannelRegistered;
        }

        private void OnDisable() {
            LogChannelRegistry.OnChannelRegistered -= OnChannelRegistered;
        }

        private void OnGUI() {
            DrawHeader();
            DrawDefineSymbolsSection();
            EditorGUILayout.Space(4);
            DrawChannelList();
            DrawFooter();
        }

        [MenuItem("NS/Tools/LogKit Settings")]
        public static void Open() => GetWindow<LogKitSettingsWindow>("LogKit Settings").Show();

        private void DrawHeader() {
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Package Logs", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Enable or disable individual log channels per package.",
                EditorStyles.miniLabel);
            EditorGUILayout.Space(4);
            DrawHorizontalLine();
        }

        private void DrawDefineSymbolsSection() {
            _showDefineSymbols = EditorGUILayout.Foldout(_showDefineSymbols, "Build Symbols");
            if (!_showDefineSymbols) return;

            EditorGUILayout.HelpBox(
                "Add or remove scripting define symbols to control logs at compile time.\n\n" +
                "• ENABLE_LOGS  — enables all package log calls (Info, Warn, Error).\n" +
                "  Remove this to strip ALL log calls from the build — zero overhead.\n\n" +
                "• ENABLE_VERBOSE_LOG     — additionally enables Verbose() calls.\n",
                MessageType.Info);

            using (new EditorGUILayout.HorizontalScope()) {
                var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
                var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);

                var defines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);

                DrawDefineToggle(ref defines, "ENABLE_LOGS",
                    "Enable Logs", namedBuildTarget);
                DrawDefineToggle(ref defines, "ENABLE_VERBOSE_LOG",
                    "Enable Verbose", namedBuildTarget);
            }
        }

        private static void DrawDefineToggle(ref string defines, string symbol,
            string label, NamedBuildTarget target) {
            var hasSym = defines.Contains(symbol);
            var toggle = EditorGUILayout.ToggleLeft(label, hasSym, GUILayout.Width(160));
            if (toggle == hasSym) return;

            defines = toggle
                ? defines + ";" + symbol
                : defines.Replace(symbol, "").Replace(";;", ";").Trim(';');

            PlayerSettings.SetScriptingDefineSymbols(target, defines);
        }

        private void DrawChannelList() {
            EditorGUILayout.LabelField("Channels", EditorStyles.boldLabel);
            EditorGUILayout.Space(2);

            var channels = LogChannelRegistry.GetAll();
            if (channels.Count == 0) {
                EditorGUILayout.HelpBox(
                    "No channels registered yet. Channels appear here the first time " +
                    "PackageLogger.For(\"...\") is called at runtime.",
                    MessageType.Info);
                return;
            }

            using (new EditorGUILayout.HorizontalScope()) {
                if (GUILayout.Button("Enable All", GUILayout.Width(90)))
                    LogChannelRegistry.SetAllEnabled(true);
                if (GUILayout.Button("Disable All", GUILayout.Width(90)))
                    LogChannelRegistry.SetAllEnabled(false);
            }

            EditorGUILayout.Space(2);

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
                EditorGUILayout.LabelField("Channel", EditorStyles.miniLabel, GUILayout.MinWidth(200));
                EditorGUILayout.LabelField("Enabled", EditorStyles.miniLabel, GUILayout.Width(60));
                EditorGUILayout.LabelField("Min Level", EditorStyles.miniLabel, GUILayout.Width(100));
            }

            foreach (var channel in channels)
                using (new EditorGUILayout.HorizontalScope()) {
                    EditorGUILayout.LabelField(channel.Name, GUILayout.MinWidth(200));

                    var wasEnabled = channel.IsEnabled;
                    channel.IsEnabled = EditorGUILayout.Toggle(channel.IsEnabled, GUILayout.Width(60));
                    if (channel.IsEnabled != wasEnabled) Repaint();

                    using (new EditorGUI.DisabledScope(!channel.IsEnabled)) {
                        channel.MinLevel = (LogLevel)EditorGUILayout.EnumPopup(
                            channel.MinLevel, GUILayout.Width(100));
                    }
                }

            EditorGUILayout.EndScrollView();
        }

        private void DrawFooter() {
            DrawHorizontalLine();
            EditorGUILayout.Space(4);

            using (new EditorGUILayout.HorizontalScope()) {
                if (GUILayout.Button("Save to Settings Asset"))
                    SaveToSettingsAsset();

                if (GUILayout.Button("Refresh Channels"))
                    Repaint();
            }

            EditorGUILayout.Space(4);
            EditorGUILayout.HelpBox(
                "Changes here apply immediately to the running Editor session.\n" +
                "Click \"Save to Settings Asset\" to persist across restarts.",
                MessageType.None);
        }

        private static void SaveToSettingsAsset() {
            var settings = AssetDatabase.LoadAssetAtPath<LogKitSettings>(LogKitSettings.SettingsPath);
            if (settings == null) {
                settings = CreateInstance<LogKitSettings>();
                Directory.CreateDirectory("Assets/Resources");
                AssetDatabase.CreateAsset(settings, LogKitSettings.SettingsPath);
            }

            Undo.RecordObject(settings, "Save LogKit Settings");

            settings.SyncWithRegistry();

            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssetIfDirty(settings);
            AssetDatabase.Refresh();

            EditorGUIUtility.PingObject(settings);
            Debug.Log("[LogKitSettings] Saved to " + LogKitSettings.SettingsPath);
        }

        private static void DrawHorizontalLine() {
            var rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f));
        }

        private void OnChannelRegistered(LogChannel _) => Repaint();
    }
}
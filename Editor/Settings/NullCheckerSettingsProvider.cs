using UnityEditor;
using UnityEngine;
using System.IO;

namespace NullCheckerEditor
{
    public class NullCheckerSettingsProvider : SettingsProvider
    {
        public const string k_MyCustomSettingsPath = "Assets/NullChecker/Editor/MyCustomSettings.asset";
        private SerializedObject _nullCheckerSettings;

        class Styles
        {
            public static GUIContent lineSize = new GUIContent("Line size in pixels");
            public static GUIContent lineSpacing = new GUIContent("Line spacing in pixels");
            public static GUIContent okColor = new GUIContent("Field color when filled");
            public static GUIContent errorColor = new GUIContent("Field color when null");
            public static GUIContent defaultWarning = new GUIContent("Warning message diplayed when null");
            public static GUIContent baseAssembly = new GUIContent("Default assembly");
        }

        public NullCheckerSettingsProvider(string path, SettingsScope scope = SettingsScope.User) : base(path, scope) 
        {
            _nullCheckerSettings = NullCheckerSettings.GetSerializedSettings();
        }

        public static bool IsSettingsAvailable()
        {
            return File.Exists(k_MyCustomSettingsPath);
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.PropertyField(_nullCheckerSettings.FindProperty("_linePixelSize"), Styles.lineSize);
            EditorGUILayout.PropertyField(_nullCheckerSettings.FindProperty("_linePixelSpacing"), Styles.lineSpacing);
            EditorGUILayout.PropertyField(_nullCheckerSettings.FindProperty("_okColor"), Styles.okColor);
            EditorGUILayout.PropertyField(_nullCheckerSettings.FindProperty("_errorColor"), Styles.errorColor);
            EditorGUILayout.PropertyField(_nullCheckerSettings.FindProperty("_defaultWarning"), Styles.defaultWarning);
            EditorGUILayout.PropertyField(_nullCheckerSettings.FindProperty("_baseAssembly"), Styles.baseAssembly);
        }

        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            if (IsSettingsAvailable())
            {
                var provider = new NullCheckerSettingsProvider("Project/NullCheckerProvider", SettingsScope.Project);

                provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
                return provider;
            }

            return null;
        }
    }
}
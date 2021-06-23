using System.IO;
using UnityEditor;
using UnityEngine;

namespace NullCheckerEditor
{
    public class NullCheckerSettings : ScriptableObject
    {
        #region Exposed
        public const string SETTINGS_ASSETS_DIRECTORY = "NullChecker/Settings";
        public const string SETTINGS_ASSETS_NAME = "NullCheckerSettings.asset";

        [SerializeField]
        private float _linePixelSize = 18f;

        [SerializeField]
        private float _linePixelSpacing = 2f;
        
        [SerializeField]
        private Color _okColor = new Color(0f, 79f/255, 5f/255);

        [SerializeField]
        private Color _errorColor = new Color(79f/255, 0f, 0f);

        [SerializeField]
        private string _baseAssembly = "Assembly-CSharp";

        [SerializeField]
        private string _defaultWarning = "Value is Null. Need to FIX before play !";

        public float LinePixelSize => _linePixelSize;
        public float LinePixelSpacing => _linePixelSpacing;
        public Color OkColor => _okColor;
        public Color ErrorColor => _errorColor;
        public string BaseAssembly => _baseAssembly; 
        public string DefaultWarning => _defaultWarning; 
        
        #endregion

        internal static NullCheckerSettings GetOrCreateSettings()
        {
            var settingsPath = $"{SETTINGS_ASSETS_DIRECTORY}/{SETTINGS_ASSETS_NAME}";

            var settings = (NullCheckerSettings)EditorGUIUtility.Load(settingsPath);//AssetDatabase.LoadAssetAtPath<NullCheckerSettings>(k_MyCustomSettingsPath);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<NullCheckerSettings>();
                
                Directory.CreateDirectory($"{Application.dataPath}/Editor Default Resources/{SETTINGS_ASSETS_DIRECTORY}");
                AssetDatabase.Refresh();
                AssetDatabase.CreateAsset(settings, $"Assets/Editor Default Resources/{settingsPath}");
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }
}
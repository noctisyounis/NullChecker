using UnityEditor;
using UnityEngine;

namespace NullCheckerEditor
{
    public class NullCheckerSettings : ScriptableObject
    {
        #region Exposed
        public const string k_MyCustomSettingsPath = "Assets/NullChecker/Editor/MyCustomSettings.asset";

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

        public float LinePixelSize{ get => _linePixelSize; }
        public float LinePixelSpacing{ get => _linePixelSpacing; }
        public Color OkColor{ get => _okColor; }
        public Color ErrorColor{ get => _errorColor; }
        public string BaseAssembly{ get => _baseAssembly; }
        public string DefaultWarning{ get => _defaultWarning; }
        
        #endregion

        internal static NullCheckerSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<NullCheckerSettings>(k_MyCustomSettingsPath);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<NullCheckerSettings>();
                AssetDatabase.CreateAsset(settings, k_MyCustomSettingsPath);
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
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NullCheckerEditor
{
    public class NullCheckerSettings : ScriptableObject
    {
        #region Exposed

        [SerializeField]
        private float _linePixelSize = 18f;
        [SerializeField]
        private float _linePixelSpacing = 2f;
        [SerializeField]
        private Color _validColor = new Color(0f, 79f/255, 5f/255);
        [SerializeField]
        private Color _errorColor = new Color(79f/255, 0f, 0f);
        [SerializeField]
        private string _baseAssembly = "Assembly-CSharp";
        [SerializeField]
        private string _defaultWarning = "Value is Null. Need to FIX before play !";
        [SerializeField]
        private string _settingPathOverride;
        
        #endregion


        #region Public Properties

        public static NullCheckerSettings Instance {get; private set;}
        public float LinePixelSize => _linePixelSize;
        public float LinePixelSpacing => _linePixelSpacing;
        public Color ValidColor => _validColor;
        public Color ErrorColor => _errorColor;
        public string BaseAssembly => _baseAssembly;
        public string DefaultWarning => _defaultWarning;

        #endregion


        #region Constants

        public const string SETTINGS_ASSETS_DIRECTORY = "NullChecker/Settings";
        public const string SETTINGS_ASSETS_NAME = "NullCheckerSettings.asset";

        #endregion

        
        #region Main

        internal static NullCheckerSettings GetOrCreateSettings()
        {
            if (Instance != null) return Instance;

            var settingsPath = _pathOverride ?? $"Assets/{SETTINGS_ASSETS_DIRECTORY}/{SETTINGS_ASSETS_NAME}";
            var settings = AssetDatabase.LoadAssetAtPath<NullCheckerSettings>(settingsPath);

            if(settings != null) return Instance = settings;

            return Instance = CreateSettingsAt(settingsPath);
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        public void CheckPath()
        {
            if(_pathOverride.Equals(_settingPathOverride)) return;

            MoveSettings(_settingPathOverride);
        }

        #endregion


        #region Plumbery

        private void MoveSettings(string newPath)
        {
            if (!newPath.Contains("Assets"))
            {
                Instance._settingPathOverride = _pathOverride;
                Debug.LogWarning("Path need to begin with 'Assets/'");
                return;
            }

            AssetDatabase.DeleteAsset(_pathOverride);
            var parentDir = GetParentPath(_pathOverride);
            ClearFoldersOnPath(parentDir);
            Instance = CreateSettingsAt(newPath);
        }

        private void ClearFoldersOnPath(string path)
        {
            path = path.Replace('\\', '/');
            if(!Directory.Exists(path)) return;
            if(Instance._settingPathOverride.StartsWith(path)) return;
            if(Directory.GetDirectories(path).Length > 0 || Directory.GetFiles(path).Length > 0) return;

            FileUtil.DeleteFileOrDirectory(path);
            FileUtil.DeleteFileOrDirectory($"{path}.meta");
            var parentDir = GetParentPath(path);
            ClearFoldersOnPath(parentDir);
        }
        
        #endregion


        #region Utils

        private static NullCheckerSettings CreateSettingsAt(string relativePath)
        {
            var settings = ScriptableObject.CreateInstance<NullCheckerSettings>();
            settings._settingPathOverride = relativePath;
            var truncatedPath = relativePath.Replace("Assets/", "");
            var fullPath = $"{Application.dataPath}/{truncatedPath}";
            _pathOverride = relativePath;
            
            if(!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            }

            AssetDatabase.Refresh();
            AssetDatabase.CreateAsset(settings, _pathOverride);
            AssetDatabase.SaveAssets();
            Debug.Log($"Created new settings at '{_pathOverride}'");

            return settings;
        }

        private string GetParentPath(string path)
        {
            return Path.GetDirectoryName(path);
        }

        #endregion
    

        #region Private Fields
            
        private static string _pathOverride;

        #endregion
    }
}
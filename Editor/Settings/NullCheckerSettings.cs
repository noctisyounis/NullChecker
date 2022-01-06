using System.IO;
using UnityEditor;
using UnityEngine;

namespace NullChecker.Editor
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
        private string _defaultWarning = "Value is Null. Need to FIX before play !";
        [SerializeField]
        private string _settingPathOverride;
        
        #endregion


        #region Unity API

        private void OnEnable() 
        {
            if(Instance != null)
            {
                //Debug.Log($"path override has been found at {Instance._settingPathOverride}");
                _pathOverride = Instance._settingPathOverride;
            }

            else
            {
                _pathOverride = _settingPathOverride;
            }
        }
             
        #endregion


        #region Public Properties

        public static NullCheckerSettings Instance {get; private set;}
        public float LinePixelSize => _linePixelSize;
        public float LinePixelSpacing => _linePixelSpacing;
        public Color ValidColor => _validColor;
        public Color ErrorColor => _errorColor;        
        public string DefaultWarning => _defaultWarning;

        #endregion


        #region Constants

        public const string SETTINGS_ASSETS_DIRECTORY = "NullChecker\\Settings";
        public const string SETTINGS_ASSETS_NAME = "NullCheckerSettings.asset";

        #endregion


        #region Properties

        public static SerializedObject SerializedSettings => new SerializedObject(GetOrCreateSettings());
             
        #endregion

        
        #region Main

        internal static NullCheckerSettings GetOrCreateSettings()
        {
            /* Debug.Log($"This is instance static => {Instance}"); */
            if (Instance != null) return Instance;
            /* Debug.Log($"This is path override static => {_pathOverride}"); */
            var settingsPath = _pathOverride;
            /* Debug.Log($"No setting instance referenced, try to find it at <color=cyan>'{settingsPath}'</color>"); */
            var settings = AssetDatabase.LoadAssetAtPath<NullCheckerSettings>($"{settingsPath}/{SETTINGS_ASSETS_NAME}");

            if(settings != null)
            {
                //Debug.Log($"Settings found at <color=cyan>'{settingsPath}'</color>");
                return Instance = settings;
            }

            //Debug.Log($"No setting found at path override, try to find it at default path");
            settingsPath = $"Assets\\{SETTINGS_ASSETS_DIRECTORY}";
            settings = AssetDatabase.LoadAssetAtPath<NullCheckerSettings>($"{settingsPath}\\{SETTINGS_ASSETS_NAME}");

            if(settings != null)
            {
                Debug.Log($"Settings found at <color=cyan>'{settingsPath}'</color>");
                return Instance = settings;
            }

            Debug.Log($"No setting found in project, creating a new one at <color=cyan>'{settingsPath}'</color>");

            return Instance = CreateSettingsAt(settingsPath);
        }

        public void CheckPath()
        {
            if(_settingPathOverride.Length < 1) 
            {
                ResetSettingPath();
                return;
            }

            Instance._settingPathOverride = _settingPathOverride.Replace('/', '\\');
            if(_settingPathOverride[_settingPathOverride.Length - 1].Equals('\\'))
            {

                Debug.LogWarning($"Invalid synthax, the setting path can't end with '/' or '\\'");
                Instance._settingPathOverride = _settingPathOverride.Remove(_settingPathOverride.Length - 1, 1);
            }
            
            if(_pathOverride.Equals(_settingPathOverride)) return;
            if (!_settingPathOverride.Contains("Assets"))
            {
                ResetSettingPath();
                Debug.LogWarning("Path need to begin with 'Assets/'");
                return;
            }
            
            if(_settingPathOverride.Contains("."))
            {
                ResetSettingPath();
                Debug.LogWarning("Path must be a folder, '.' or extension are not allowed.");
                return;
            }

            MoveSettings(_settingPathOverride);
        }

        private void ResetSettingPath()
        {
            Instance._settingPathOverride = _pathOverride;
        }

        #endregion


        #region Plumbery

        private void MoveSettings(string newPath)
        {
            Debug.Log($"initialize settings movement to <color=cyan>'{newPath}'</color>");
            Debug.Log($"Delete previous settings at <color=cyan>'{_pathOverride}'</color>");
            AssetDatabase.DeleteAsset(_pathOverride);
            var parentDir = GetParentPath(_pathOverride);
            ClearFoldersOnPath(parentDir);
            //Debug.Log($"Creating new settings at <color=cyan>'{newPath}'</color>");
            Instance = CreateSettingsAt(newPath);
        }

        private void ClearFoldersOnPath(string path)
        {
            if(!Directory.Exists(path)) {
                /* Debug.Log($"<color=cyan>'{path}'</color> doesn't exists, stop cleaning path"); */
                return;
            }

            var isDirectoryNotEmpty = Directory.GetDirectories(path).Length > 0 || Directory.GetFiles(path).Length > 0;
            if(isDirectoryNotEmpty) 
            {
                /* Debug.Log($"<color=cyan>'{path}'</color> contains {Directory.GetDirectories(path).Length} directories and {Directory.GetFiles(path).Length} files, stop deleting"); */
                return;
            }
            
            var isPartOfDestinationPath = Instance._settingPathOverride.StartsWith(path);
            //Debug.Log($"path <color=cyan>{path}</color> > < instance <color=cyan>{Instance._settingPathOverride}</color>");
            isPartOfDestinationPath = isPartOfDestinationPath && Instance._settingPathOverride[path.Length - 1].Equals('\\');
            if(isPartOfDestinationPath) 
            {
                Debug.Log($"<color=cyan>'{path}'</color> is part of the destination path, end of cleaning"); 
                return;
            }

            Debug.Log($"Deleting folder at <color=cyan>'{path}'</color>");
            FileUtil.DeleteFileOrDirectory(path);
            FileUtil.DeleteFileOrDirectory($"{path}.meta");
            var parentDir = GetParentPath(path);
            ClearFoldersOnPath(parentDir);
        }
        
        #endregion


        #region Utils

        private static NullCheckerSettings CreateSettingsAt(string relativePath)
        {
            /* Debug.Log($"Creation of a new setting instance due not existing settings or modification of the path"); */
            NullCheckerSettings settings = Instance?.CopySettings();
            if(!settings) settings = ScriptableObject.CreateInstance<NullCheckerSettings>();
            
            settings._settingPathOverride = relativePath;
            /* Debug.Log($"Update of _pathOverride: previous was <color=cyan>'{_pathOverride}'</color>"); */
            _pathOverride = relativePath;
            /* Debug.Log($"Update of _pathOverride: new is <color=cyan>'{_pathOverride}'</color>"); */
            
            if(!Directory.Exists(_pathOverride))
            {
                /* Debug.Log($"folder at <color=cyan>'{_pathOverride}'</color> not found, creating folder at location"); */
                Directory.CreateDirectory(_pathOverride);
            }

            //AssetDatabase.Refresh();
            AssetDatabase.CreateAsset(settings, $"{_pathOverride}\\{SETTINGS_ASSETS_NAME}");
            AssetDatabase.SaveAssets();
            //LEAVE THIS ONE PLZ
            Debug.Log($"Created new settings at <color=cyan>'{_pathOverride}\\{SETTINGS_ASSETS_NAME}'</color>", NullCheckerSettings.Instance);

            return settings;
        }

        private string GetParentPath(string path)
        {
            return Path.GetDirectoryName(path);
        }

        private NullCheckerSettings CopySettings()
        {
            var settings = ScriptableObject.CreateInstance<NullCheckerSettings>();
            settings._defaultWarning = Instance._defaultWarning;
            settings._errorColor = Instance._errorColor;
            settings._linePixelSize = Instance._linePixelSize;
            settings._linePixelSpacing = Instance._linePixelSpacing;
            settings._settingPathOverride = Instance._settingPathOverride;
            settings._validColor = Instance._validColor;

            return settings;
        }

        #endregion
    

        #region Private Fields
            
        private static string _pathOverride;

        #endregion
    }
}
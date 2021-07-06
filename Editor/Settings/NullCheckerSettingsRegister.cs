using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NullCheckerEditor
{
    public static class NullCheckerSettingsRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateNullCheckerSettingsProvider()
        {
            var provider = new SettingsProvider("Project/NullCheckSettings", SettingsScope.Project)
            {
                label = "Null Checker",

                guiHandler = (searchContext) =>
                {
                    
                    var settings = NullCheckerSettings.GetSerializedSettings();

                    EditorGUILayout.PropertyField(settings.FindProperty("_linePixelSize"), new GUIContent("Line size in pixels"));
                    EditorGUILayout.PropertyField(settings.FindProperty("_linePixelSpacing"), new GUIContent("Line spacing in pixels"));
                    EditorGUILayout.PropertyField(settings.FindProperty("_validColor"), new GUIContent("Field color when filled"));
                    EditorGUILayout.PropertyField(settings.FindProperty("_errorColor"), new GUIContent("Field color when null"));
                    EditorGUILayout.PropertyField(settings.FindProperty("_defaultWarning"), new GUIContent("Warning message diplayed when null"));
                    EditorGUILayout.PropertyField(settings.FindProperty("_baseAssembly"), new GUIContent("Default assembly"));
                    EditorGUILayout.PropertyField(settings.FindProperty("_settingPathOverride"), new GUIContent("Setting path"));                    

                    settings.ApplyModifiedProperties();
                    NullCheckerSettings.Instance.CheckPath();
                },

                keywords = new HashSet<string>(new[]    {
                                                            "Line", 
                                                            "Size", "size", 
                                                            "Spacing", "spacing", 
                                                            "Color", "color", 
                                                            "Warning", 
                                                            "Assembly", "assembly"
                                                        })
            };

            return provider;
        }
    }
}
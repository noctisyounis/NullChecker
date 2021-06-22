using System.Reflection;
using System;
using UnityEditor;
using UnityEngine;

using Component = UnityEngine.Component;
using static UnityEngine.Debug;

namespace NullCheckerEditor
{
    [CustomPropertyDrawer(typeof(UnityEngine.Object), true)]
    public class ObjectDrawer : PropertyDrawer
    {
        #region Unity API

        public ObjectDrawer() : base()
        {
            PopulateAssemblyNames();
            _warningText = DEFAULT_WARNING;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
        {
            this._property = property;
            DeterminePropertyType();

            EditorGUI.BeginProperty(position, label, _property);
            GUILayout.Space(-17f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            EditorGUILayout.PropertyField(_property, label);
            
            if(!_property.objectReferenceValue)
            {
                GUILayout.Space(5.0f);
                Color defaultColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                DrawWarningLabel();
                GUI.backgroundColor = defaultColor;
                if(_type != null)
                {
                    if(_type.Equals(typeof(GameObject)))
                    {
                        DrawFixGameObjectButton();
                    }
                    else if(_type.IsSubclassOf(typeof(Component)))
                    {
                        DrawFixComponentButton();
                    }
                }
                
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        #endregion


        #region Main

        private void DrawWarningLabel()
        {
            EditorGUILayout.LabelField(_warningText);
        }

        private void DrawFixGameObjectButton()
        {
            if(GUILayout.Button("FIX game object"))
            {
                FindValueToFixGameObject();
            }
        }

        private void DrawFixComponentButton()
        {
            if(GUILayout.Button("FIX component"))
            {
                FindValueToFixComponent();

                ResetWarningText();
                _warningText += " (No usable component found)";
            }
        }

        #endregion


        #region Utils

        private void ResetWarningText()
        {
            _warningText = DEFAULT_WARNING;
        }

        private void DeterminePropertyType()
        {
            _owner = (MonoBehaviour) _property.serializedObject.targetObject;
            var stringType = _property.type;
            var cleanedStringType = stringType
                                    .Replace("PPtr<$", "")
                                    .Replace(">", "");

            _type = FindTypeInAssemblies(cleanedStringType);
        }

        private void FindValueToFixGameObject()
        {
            _property.objectReferenceValue = _owner.gameObject;
        }

        private void FindValueToFixComponent()
        {
            _property.objectReferenceValue = (UnityEngine.Object)Convert.ChangeType(_owner.GetComponent(_type), _type);
        }

        private void PopulateAssemblyNames()
        {
            _assemblyNames = new string[_assemblies.Length];
            for (int i = 0; i < _assemblies.Length; i++)
            {
                _assemblyNames[i] = _assemblies[i].FullName.Split(',')[0];
            }
        }

        private Type FindTypeInAssemblies(string type)
        {
            Type result = null;
            foreach (var assembly in _assemblyNames)
            {
                var path = $"{assembly}.{type}, {assembly}";

                try
                {
                    result = Type.GetType(path, true);
                    return result;
                }catch(Exception e)
                {
                    
                }
            }

            result = Type.GetType($"{type}, {BASE_ASSEMBLY}");

            return result;
        }

        #endregion


        #region Private
        private SerializedProperty _property;
        private MonoBehaviour _owner;
        private Type _type;
        private string _warningText;

        private const string BASE_ASSEMBLY = "Assembly-CSharp";
        private const string DEFAULT_WARNING = "Value is Null. Need to FIX before play !";
        private static string[] _assemblyNames;
        private static Assembly[] _assemblies = AppDomain.CurrentDomain.GetAssemblies();

        #endregion
    }
}
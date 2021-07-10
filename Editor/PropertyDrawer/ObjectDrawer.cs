using System.Collections.Generic;
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
            InitializeFromSettings();
            if(_typeNames == null)
            {
                PopulateTypes();
            }

            _warningText = _defaultWarning;
        }

        public override float GetPropertyHeight (SerializedProperty property, GUIContent label) 
        {
            var correctorShort = _linePixelSize / EditorGUI.GetPropertyHeight (property);
            var correctorLong =  (_linePixelSpacing + _linePixelSize) / EditorGUI.GetPropertyHeight (property) * 3;

            var coefficient = MustBeShort() ? correctorShort : correctorLong;

            return EditorGUI.GetPropertyHeight (property) * coefficient;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
        {
            this._property = property;

            if(_types == null || _types.Count == 0)
            {
                DeterminePropertyPossibleTypes();
                PopulateComponentTypesFrom(_types);
            }

            EditorGUI.BeginProperty(position, label, _property);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var fieldRect = new Rect(position.x, position.y, position.width, _linePixelSize);
            var warningRect = new Rect(position.x, position.y + (_linePixelSize + _linePixelSpacing), position.width, _linePixelSize);
            var buttonRect = new Rect(position.x, warningRect.y + (_linePixelSize + _linePixelSpacing), position.width, _linePixelSize);

            EditorGUI.DrawRect(position, MustBeShort() ? _okColor : _errorColor);

            EditorGUI.PropertyField(fieldRect, _property, label);
            
            if(!_property.objectReferenceValue)
            {
                Color defaultColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                
                DrawWarningLabel(warningRect);

                GUI.backgroundColor = defaultColor;

                if(_types.Count > 0 && _owner != null)
                {
                    if(_types.Contains(typeof(GameObject)))
                    {
                        DrawFixGameObjectButton(buttonRect);
                    }
                    else if(_componentTypes.Count > 0)
                    {
                        DrawFixComponentButton(buttonRect);
                    }
                }
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        #endregion


        #region Main

        private void DrawWarningLabel(Rect rect)
        {
            EditorGUI.LabelField(rect, _warningText);
        }

        private void DrawFixGameObjectButton(Rect rect)
        {
            if(GUI.Button(rect, "FIX game object"))
            {
                FindValueToFixGameObject();
            }
        }

        private void DrawFixComponentButton(Rect rect)
        {
            if(GUI.Button(rect, "FIX component"))
            {
                FindValueToFixComponent();

                ResetWarningText();
                _warningText += " (Not found)";
            }
        }

        #endregion


        #region Utils

        private void InitializeFromSettings()
        {
            _settings = NullCheckerSettings.GetOrCreateSettings();

            _linePixelSize = _settings.LinePixelSize;
            _linePixelSpacing = _settings.LinePixelSpacing;
            _okColor = _settings.ValidColor;
            _errorColor = _settings.ErrorColor;
            _baseAssembly = _settings.BaseAssembly;
            _defaultWarning = _settings.DefaultWarning;
        }

        private bool MustBeShort()
        {
            if(_property == null) return true;
            if(!_property.objectReferenceValue) return false;

            return true;
        }

        private void ResetWarningText()
        {
            _warningText = _defaultWarning;
        }

        private void DeterminePropertyPossibleTypes()
        {
            if(_property.serializedObject.targetObject is MonoBehaviour)
            {
                _owner = (MonoBehaviour) _property.serializedObject.targetObject;
            }
            var stringType = _property.type;
            var cleanedStringType = stringType
                                    .Replace("PPtr<$", "")
                                    .Replace(">", "");

            _types = FindTypeInAssemblies(cleanedStringType);
        }

        private void PopulateComponentTypesFrom(List<Type> types)
        {
            _componentTypes = new List<Type>();

            foreach (var item in _types)
            {
                if(item.IsSubclassOf(typeof(Component)))
                {
                    _componentTypes.Add(item);
                }
            }
        }

        private void FindValueToFixGameObject()
        {
            _property.objectReferenceValue = _owner.gameObject;
        }

        // private void FindValueToFixComponent()
        // {
        //     foreach (var type in _types)
        //     {
        //         var try
        //         if(_property.objectReferenceValue == null)
        //         {
        //             _property.objectReferenceValue = (UnityEngine.Object)Convert.ChangeType(_owner.GetComponent(type), type);
        //         }
        //         else if()
        //         {

        //         }
        //     }
        // }

        private void FindValueToFixComponent()
        {
            var otherPossibleComponents = new List<UnityEngine.Object>();
            foreach (var type in _componentTypes)
            {
                var method = typeof(Component).GetMethod("GetComponent", new Type[]{}).MakeGenericMethod(type);
                var component = (UnityEngine.Object)method.Invoke(_owner, new object[]{});

                if(_property.objectReferenceValue == null)
                {
                    _property.objectReferenceValue = component;
                }
                else
                {
                    otherPossibleComponents.Add(component);
                }
            }

            if(otherPossibleComponents.Count == 0) return;

            DebugOtherPossibleComponents(otherPossibleComponents);
        }

        private void DebugOtherPossibleComponents(List<UnityEngine.Object> others)
        {
            var text = "Other Components have been found but not sets : \n";

            foreach (var item in others)
            {
                text += $"\t-\t{item.name}\n";
            }

            Debug.LogWarning(text);
        }

        private void PopulateTypes()
        {
            _typeNames = new Dictionary<TypeInfo, Type>();

            for (int i = 0; i < _assemblies.Length; i++)
            {
                foreach (var typeInfo in _assemblies[i].DefinedTypes)
                {
                    _typeNames.Add(typeInfo, typeInfo.AsType());
                }
            }
        }

        private List<Type> FindTypeInAssemblies(string type)
        {
            List<Type> result = new List<Type>();

            foreach (var item in _typeNames)
            {
                if(type.Equals(item.Key.Name))
                {
                    result.Add(item.Value);
                }
            }

            return result;
        }

        #endregion


        #region Private
        private NullCheckerSettings _settings;

        private float _linePixelSize = 18f;
        private float _linePixelSpacing = 2f;
        private Color _okColor = new Color(0f, 79f/255, 5f/255);
        private Color _errorColor = new Color(79f/255, 0f, 0f);
        private string _baseAssembly = "Assembly-CSharp";
        private string _defaultWarning = "Value is Null. Need to FIX before play !";

        private SerializedProperty _property;
        private MonoBehaviour _owner;
        private List<Type> _types;
        private List<Type> _componentTypes;
        private string _warningText;

        private static Dictionary<TypeInfo, Type> _typeNames;
        private static Assembly[] _assemblies = AppDomain.CurrentDomain.GetAssemblies();

        #endregion
    }
}
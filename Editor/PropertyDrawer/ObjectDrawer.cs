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

            if(_type == null)
            {
                DeterminePropertyType();
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

                if(_type != null && _owner != null)
                {
                    if(_type.Equals(typeof(GameObject)))
                    {
                        DrawFixGameObjectButton(buttonRect);
                    }
                    else if(_type.IsSubclassOf(typeof(Component)))
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

        private void DeterminePropertyType()
        {
            if(_property.serializedObject.targetObject is MonoBehaviour)
            {
                _owner = (MonoBehaviour) _property.serializedObject.targetObject;
            }

            var targetObjectType = _property.serializedObject.targetObject.GetType();
            var property = targetObjectType.GetProperty(_property.name, 
                                                        BindingFlags.Instance | 
                                                        BindingFlags.NonPublic | 
                                                        BindingFlags.Public);
            if(property != null)
            {
               _type = property.PropertyType;
            }
            
            else
            {
                var field = targetObjectType.GetField(_property.name, 
                                                    BindingFlags.Instance | 
                                                    BindingFlags.NonPublic | 
                                                    BindingFlags.Public);
                _type = field.FieldType;
            }
        }

        private void FindValueToFixGameObject()
        {
            _property.objectReferenceValue = _owner.gameObject;
        }

        private void FindValueToFixComponent()
        {
            var method = typeof(Component).GetMethod("GetComponent", new Type[]{}).MakeGenericMethod(_type);

            _property.objectReferenceValue = (UnityEngine.Object)method.Invoke(_owner, new object[]{});
        }

        #endregion


        #region Private
        private NullCheckerSettings _settings;

        private float _linePixelSize = 18f;
        private float _linePixelSpacing = 2f;
        private Color _okColor = new Color(0f, 79f/255, 5f/255);
        private Color _errorColor = new Color(79f/255, 0f, 0f);
        private string _defaultWarning = "Value is Null. Need to FIX before play !";

        private SerializedProperty _property;
        private MonoBehaviour _owner;
        private Type _type;
        private string _warningText;

        #endregion
    }
}
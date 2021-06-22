using System.Reflection;
using System;
using UnityEditor;
using UnityEngine;

using Component = UnityEngine.Component;
using static UnityEngine.Debug;

[CustomPropertyDrawer(typeof(UnityEngine.Object), true)]
public class ObjectDrawer : PropertyDrawer
{
    #region Unity API

    public ObjectDrawer() : base()
    {
        PopulateAssemblyNames();
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
        var warningString = "Value is Null. Need to FIX before play !";
        EditorGUILayout.LabelField(warningString);
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
        }
    }

    #endregion


    #region Utils

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
            var path = (assembly.Length > 0 && !assembly.Equals(BASE_ASSEMBLY)) ? $"{assembly}.{type}" : type;

            try
            {
                LogWarning(assembly);
                result = Type.GetType($"{path}, {assembly}", true);
                return result;
            }catch(Exception e)
            {
                LogError(assembly);
            }
        }

        return result;
    }

    #endregion


    #region Private
    private SerializedProperty _property;
    private MonoBehaviour _owner;
    private Type _type;

    private const string BASE_ASSEMBLY = "Assembly-CSharp";
    private static string[] _assemblyNames;
    private static Assembly[] _assemblies = AppDomain.CurrentDomain.GetAssemblies();

    #endregion
}
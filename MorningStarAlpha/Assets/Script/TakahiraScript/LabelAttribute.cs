using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LabelAttribute : PropertyAttribute
{
    public readonly string Label;

    public LabelAttribute(string label)
    {
        Label = label;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(LabelAttribute))]
public class LabelAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var newLabel = attribute as LabelAttribute;
        EditorGUI.PropertyField(position, property, new GUIContent(newLabel.Label), true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }
}
#endif




// Enum�̗v�f�ɕt����Attribute
// PropertyAttribute�ł͂Ȃ�System.Attribute���p������
[System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Enum, AllowMultiple = false)]
public class EnumCName : System.Attribute
{
    public string DisplayName { get; private set; }

    public EnumCName(string displayName)
    {
        DisplayName = displayName;
    }
}

// Enum���̂ɕt����Attribute
// �����PropertyDrawer�Ŏg���̂�PropertyAttribute���p������
[System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Enum, AllowMultiple = false)]
public class EnumPName : PropertyAttribute
{
    public System.Type Type { get; private set; }
    public readonly string Label = null;

    public EnumPName(System.Type selfType)
    {
        Type = selfType;
    }

    public EnumPName(string label, System.Type selfType)
    {
        Label = label;
        Type = selfType;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(EnumPName))]
public class EnumAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = attribute as EnumPName;

        // FieldInfo����e�v�f��Attribute���擾�����O�𓾂�
        var names = new List<string>();
        foreach (var fi in attr.Type.GetFields())
        {
            if (fi.IsSpecialName)
            {
                // SpecialName�͔�΂�
                continue;
            }
            var elementAttribute = fi.GetCustomAttributes(typeof(EnumCName), false).FirstOrDefault() as EnumCName;
            names.Add(elementAttribute == null ? fi.Name : elementAttribute.DisplayName);
        }

        // �e�v�f�̒l��Enum.GetValues()�Ŏ擾����
        var values = System.Enum.GetValues(attr.Type).Cast<int>();

        // �`��
        if (attr.Label != null)
            property.intValue = EditorGUI.IntPopup(position, attr.Label, property.intValue, names.ToArray(), values.ToArray());
        else
            property.intValue = EditorGUI.IntPopup(position, property.displayName, property.intValue, names.ToArray(), values.ToArray());
    }
}
#endif
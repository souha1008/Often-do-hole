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




// Enumの要素に付けるAttribute
// PropertyAttributeではなくSystem.Attributeを継承する
[System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Enum, AllowMultiple = false)]
public class EnumCName : System.Attribute
{
    public string DisplayName { get; private set; }

    public EnumCName(string displayName)
    {
        DisplayName = displayName;
    }
}

// Enum自体に付けるAttribute
// これはPropertyDrawerで使うのでPropertyAttributeを継承する
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

        // FieldInfoから各要素のAttributeを取得し名前を得る
        var names = new List<string>();
        foreach (var fi in attr.Type.GetFields())
        {
            if (fi.IsSpecialName)
            {
                // SpecialNameは飛ばす
                continue;
            }
            var elementAttribute = fi.GetCustomAttributes(typeof(EnumCName), false).FirstOrDefault() as EnumCName;
            names.Add(elementAttribute == null ? fi.Name : elementAttribute.DisplayName);
        }

        // 各要素の値はEnum.GetValues()で取得する
        var values = System.Enum.GetValues(attr.Type).Cast<int>();

        // 描画
        if (attr.Label != null)
            property.intValue = EditorGUI.IntPopup(position, attr.Label, property.intValue, names.ToArray(), values.ToArray());
        else
            property.intValue = EditorGUI.IntPopup(position, property.displayName, property.intValue, names.ToArray(), values.ToArray());
    }
}
#endif
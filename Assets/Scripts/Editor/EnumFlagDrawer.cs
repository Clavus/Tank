using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumFlagAttribute))]
public class EnumFlagsAttributeDrawer : PropertyDrawer
{
    // These values are automatically adjusted based on the width of the enum names and the width of the inspector window
    private int buttonsPerLine = 1;
    private int numLines = 1;

    public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
    {
        int buttonsIntValue = 0;
        int enumLength = prop.enumNames.Length;
        bool[] buttonPressed = new bool[enumLength];
        float buttonWidth = (rect.width - EditorGUIUtility.labelWidth) / buttonsPerLine;
        float buttonHeight = rect.height / numLines;
        EditorGUI.LabelField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height), label);
        EditorGUI.BeginChangeCheck();

        for (int i = 0; i < enumLength; i++)
        {
            // Check if the button is/was pressed 
            if ((prop.intValue & (1 << i)) == 1 << i)
            {
                buttonPressed[i] = true;
            }

            Rect buttonPos = new Rect(rect.x + EditorGUIUtility.labelWidth + buttonWidth * (i % buttonsPerLine), rect.y + (float) Math.Floor((float)i / buttonsPerLine) * buttonHeight, buttonWidth, buttonHeight);
            buttonPressed[i] = GUI.Toggle(buttonPos, buttonPressed[i], prop.enumNames[i], "Button");

            if (buttonPressed[i])
                buttonsIntValue += 1 << i;
        }

        if (EditorGUI.EndChangeCheck())
        {
            prop.intValue = buttonsIntValue;
        }
    }

    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
    {
        float buttonSpaceWidth = EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth;
        float buttonMargin = 16;
        int enumLength = prop.enumNames.Length;
        float maxSize = 0;

        Vector2 size;
        for (int i = 0; i < enumLength; i++)
        {
            size = GUI.skin.label.CalcSize(new GUIContent(prop.enumNames[i]));
            size.x = size.x + buttonMargin;
            if (size.x > maxSize)
                maxSize = size.x;
        }
        buttonsPerLine = (int) Math.Floor(Mathf.Max(1f,buttonSpaceWidth / maxSize));
        numLines = (int) Math.Ceiling((float) enumLength / buttonsPerLine);

        return base.GetPropertyHeight(prop, label) * numLines;
    }
}
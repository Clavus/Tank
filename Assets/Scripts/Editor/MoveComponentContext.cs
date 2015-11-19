using UnityEditor;
using UnityEngine;

public class MoveComponentContext
{
    private enum Destination
    {
        Top,
        Bottom
    };

    private const string kComponentArrayName = "m_Component";
    private const int kFirstComponentIndex = 1;

    [MenuItem("CONTEXT/Component/Move to Top")]
    private static void Top(MenuCommand command)
    {
        Move((Component) command.context, Destination.Top);
    }

    [MenuItem("CONTEXT/Component/Move to Bottom")]
    private static void Bottom(MenuCommand command)
    {
        Move((Component) command.context, Destination.Bottom);
    }

    private static void Move(Component target, Destination destination)
    {
        //SerializedObject component = new SerializedObject(target);
        SerializedObject gameObject = new SerializedObject(target.gameObject);
        SerializedProperty componentArray = gameObject.FindProperty(kComponentArrayName);
        int size = componentArray.arraySize;

        for (int index = kFirstComponentIndex; index < size; ++index)
        {
            SerializedProperty iterator = componentArray.GetArrayElementAtIndex(index);
            iterator.Next(true);
            iterator.Next(true);

            if (iterator.objectReferenceValue == target)
            {
                componentArray.MoveArrayElement(index, destination == Destination.Top ? kFirstComponentIndex : size - 1);
                gameObject.ApplyModifiedProperties();

                break;
            }
        }
    }
}
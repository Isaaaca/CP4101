using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SequenceEvent))]
public class ScriptedEventEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float defaultHeight = base.GetPropertyHeight(property,label);
        //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        SerializedProperty eventTypeProperty = property.FindPropertyRelative("eventType");
        SerializedProperty[] displayedProperty = new SerializedProperty[3];
        switch (eventTypeProperty.enumValueIndex)
        {
            case (int)SequenceEvent.EventType.Dialogue:
                displayedProperty[0] = property.FindPropertyRelative("dialogue");
                break;
            case (int)SequenceEvent.EventType.Fade:
                displayedProperty[0] = property.FindPropertyRelative("opacity");
                displayedProperty[1] = property.FindPropertyRelative("duration");
                break;
            case (int)SequenceEvent.EventType.Switchable:
                displayedProperty[0] = property.FindPropertyRelative("target");
                break;
            case (int)SequenceEvent.EventType.CameraPanTo:
                displayedProperty[0] = property.FindPropertyRelative("target");
                break;
            case (int)SequenceEvent.EventType.CameraJumpTo:
                displayedProperty[0] = property.FindPropertyRelative("target");
                break;
            case (int)SequenceEvent.EventType.Teleport:
                displayedProperty[0] = property.FindPropertyRelative("position");
                break;
            case (int)SequenceEvent.EventType.TeleportRelative:
                displayedProperty[0] = property.FindPropertyRelative("position");
                break;
            case (int)SequenceEvent.EventType.Turn:
                displayedProperty[0] = property.FindPropertyRelative("target");
                break;
            case (int)SequenceEvent.EventType.Pause:
                displayedProperty[0] = property.FindPropertyRelative("duration");
                break;


        }


        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.

        EditorGUI.BeginProperty(position, label, property);
        // Draw label
        Rect enumPosition = new Rect(position.x, position.y, position.width, defaultHeight);
        EditorGUI.PropertyField(enumPosition, eventTypeProperty);

        Rect dispRect = position;
        for (int i = 0; i<displayedProperty.Length&& displayedProperty[i]!= null; i++)
        {
            dispRect = GetNextLineRect(dispRect, defaultHeight);
            EditorGUI.PropertyField(dispRect, displayedProperty[i]);
        }
        EditorGUI.EndProperty();

    }

    private Rect GetNextLineRect(Rect position, float height)
    {
        return new Rect(position.x, position.y + height, position.width, height);
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty eventTypeProperty = property.FindPropertyRelative("eventType");
        float height = base.GetPropertyHeight(property, label);
        switch (eventTypeProperty.enumValueIndex)
        {
            case (int)SequenceEvent.EventType.Dialogue:
                height *= 2;
                break;
            case (int)SequenceEvent.EventType.Switchable:
                height *= 2;
                break;
            case (int)SequenceEvent.EventType.CameraPanTo:
                height *= 2;
                break;
            case (int)SequenceEvent.EventType.CameraJumpTo:
                height *= 2;
                break;
            case (int)SequenceEvent.EventType.Teleport:
                height *= 2;
                break;
            case (int)SequenceEvent.EventType.TeleportRelative:
                height *= 2;
                break;
            case (int)SequenceEvent.EventType.Turn:
                height *= 2;
                break;
            case (int)SequenceEvent.EventType.Pause:
                height *= 2;
                break;
            case (int)SequenceEvent.EventType.Fade:
                height *= 3;
                break;
        }
        return height + 10;
    }
}

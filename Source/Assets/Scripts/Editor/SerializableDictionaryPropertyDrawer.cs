using UnityEditor;

[CustomPropertyDrawer(typeof(GameEventDictionary))]
[CustomPropertyDrawer(typeof(SequenceDictionary))]
[CustomPropertyDrawer(typeof(FeedbackDictionary))]
[CustomPropertyDrawer(typeof(AudioDictionary))]
public class AnySerializableDictionaryPropertyDrawer :
SerializableDictionaryPropertyDrawer
{ }
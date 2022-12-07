using UnityEngine;
using UnityEditor;

using NijiDive.Map.Chunks;

namespace NijiDive.Editors
{
    [CustomEditor(typeof(ChunkEditor))]
    public class ChunkEditorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var chunkEditor = (ChunkEditor)target;
            var exclude = new string[] { nameof(chunkEditor.newChunkFileName) };

            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, exclude);
            if (GUILayout.Button("Load Chunk")) chunkEditor.LoadChunk();
            if (GUILayout.Button($"Overwrite {nameof(chunkEditor.ToLoad)}")) chunkEditor.OverwriteChunk();

            EditorGUILayout.PropertyField(serializedObject.FindProperty(exclude[0]));
            if (GUILayout.Button("Save New Chunk")) chunkEditor.SaveChunk();

            GUILayout.Space(10f);
            if (GUILayout.Button("Clear Editor")) chunkEditor.ClearEditor();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using Kit.Extend;
using Kit.Inspector;

namespace CF.CameraBot
{
	public partial class CameraBotEditor
	{
		private PresetReorderableList m_Preset;
		private void InitReorderableList()
		{
			m_Preset = new PresetReorderableList(serializedObject, "PresetList");
		}

		private void DrawCameraPresetList()
		{
			m_Preset.DoLayoutList();
		}

		private void DestoryReorderableList()
		{
			m_Preset.Dispose();
		}
	}

	/// <summary>Display camera list</summary>
	/// <see cref="http://pastebin.com/WhfRgcdC"/>
	public class PresetReorderableList : ReorderableListTemplate
	{
		protected CameraBot target;
		public PresetReorderableList(SerializedObject serializedObject, string propertyName, bool dragable = true, bool displayHeader = true, bool displayAddButton = true, bool displayRemoveButton = true)
			: base(serializedObject, propertyName, dragable, displayHeader, displayAddButton, displayRemoveButton)
		{
			target = serializedObject.targetObject as CameraBot;
		}

		protected override void OnSelect(ReorderableList list, SerializedProperty selectedElement)
		{
			if (selectedElement.objectReferenceValue != null)
			{
				Component obj = (selectedElement.objectReferenceValue as Component);
				EditorGUIUtility.PingObject(obj.gameObject);
			}
		}

		protected override void OnAdd(ReorderableList list, SerializedProperty newElement)
		{
			string tag = "Create Camera" + target.GetInstanceID();

			Undo.RecordObject(target, tag);
			GameObject obj = new GameObject(string.Empty, typeof(Preset));
			Undo.RegisterCreatedObjectUndo(obj, tag);
			Undo.RecordObject(obj, tag);

			obj.name = "Camera Preset " + obj.GetHashCode();
			obj.transform.SetParent(target.transform);
			obj.transform.SetAsLastSibling();
			Preset preset = obj.GetComponent<Preset>();
			Undo.RecordObject(preset, tag);
			preset.m_DebugColor = Color.white.RandomRange(Color.black.SetAlpha(.5f), Color.white);
			newElement.objectReferenceValue = preset;
			preset.m_Host = target;
			preset.OnValidate();
			Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
		}

		protected override void OnRemove(ReorderableList list, SerializedProperty deleteElement)
		{
			SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(list.index);
			if (element.objectReferenceValue != null)
			{
				Preset item = (element.objectReferenceValue as Preset);
				string label = item.name;
				if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete :\n\r[ " + label + " ] ?", "Yes", "No"))
				{
					string tag = "Remove Camera" + target.GetInstanceID();
					Undo.RecordObjects(new Object[] { item.gameObject, target }, tag);
					target.PresetList.RemoveAt(list.index);
					Undo.DestroyObjectImmediate(item.gameObject);
					Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
				}
			}
			else
			{
				ReorderableList.defaultBehaviours.DoRemoveButton(list);
			}
		}

		protected override void OnReorder(ReorderableList list, int fromIndex, int toIndex)
		{
			// hack : since OnReorder() are faster then OnValidate() we need the way to re-arrange later;
			EditorApplication.update += LaterEditorUpdate;
		}
		
		private void LaterEditorUpdate()
		{
			EditorApplication.update -= LaterEditorUpdate;
			for (int i = 0; i < property.arraySize; i++)
			{
				Component obj = (property.GetArrayElementAtIndex(i).objectReferenceValue as Component);
				if (obj != null)
					obj.transform.SetSiblingIndex(i);
			}
		}
	}
}
#endif
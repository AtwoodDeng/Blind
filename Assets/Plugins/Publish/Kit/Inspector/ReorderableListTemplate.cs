#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Kit.Inspector
{
	public class ReorderableListTemplate : IDisposable
	{
		#region variable
		protected SerializedObject serializedObject;
		protected SerializedProperty property;
		protected string propertyName;

		List<float> elementHeights;
		ReorderableList orderList;
		int selectedIndex;

		Texture2D backgroundImage;
		#endregion
		
		#region System
		/// <summary>Overrider this method for child class.</summary>
		/// <example>
		/// public Your_Class_Here(SerializedObject serializedObject, string propertyName, bool dragable = true, bool displayHeader = true, bool displayAddButton = true, bool displayRemoveButton = true)
		///		: base(serializedObject, propertyName, dragable, displayHeader, displayAddButton, displayRemoveButton) { }
		/// </example>
		/// <param name="serializedObject"></param>
		/// <param name="propertyName"></param>
		/// <param name="dragable"></param>
		/// <param name="displayHeader"></param>
		/// <param name="displayAddButton"></param>
		/// <param name="displayRemoveButton"></param>
		public ReorderableListTemplate(SerializedObject serializedObject, string propertyName,
			bool dragable = true, bool displayHeader = true, bool displayAddButton = true, bool displayRemoveButton = true)
		{
			Init(serializedObject, propertyName, dragable, displayHeader, displayAddButton, displayRemoveButton);
		}
		
		protected virtual void Init(SerializedObject serializedObject, string propertyName,
			bool dragable = true, bool displayHeader = true, bool displayAddButton = true, bool displayRemoveButton = true)
		{
			this.propertyName = propertyName;
			this.serializedObject = serializedObject;
			this.property = serializedObject.FindProperty(this.propertyName);
			elementHeights = new List<float>(property.arraySize);
			SetHightLightBackgroundImage();

			orderList = new ReorderableList(serializedObject, property, dragable, displayHeader, displayAddButton, displayRemoveButton);
			orderList.onAddCallback += OnAdd;
			orderList.onSelectCallback += OnSelect;
			orderList.onRemoveCallback += OnRemove;
			orderList.onReorderCallback += OnReorder;
			orderList.drawHeaderCallback += OnDrawHeader;
			orderList.drawElementCallback += OnDrawElement;
			orderList.drawElementBackgroundCallback += OnDrawElementBackground;
			orderList.elementHeightCallback += OnCalculateItemHeight;
		}
		#endregion

		#region API
		public virtual void SetHightLightBackgroundImage()
		{
			backgroundImage = new Texture2D(3, 1);
			backgroundImage.SetPixel(0, 0, new Color(0f, .8f, .7f));
			backgroundImage.hideFlags = HideFlags.DontSave;
			backgroundImage.wrapMode = TextureWrapMode.Clamp;
			backgroundImage.Apply();
		}

		public void DoLayoutList()
		{
			orderList.DoLayoutList();
		}

		public void DoList(Rect rect)
		{
			orderList.DoList(rect);
		}
		#endregion

		#region listener
		protected virtual void OnDrawHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, property.displayName);
		}
		
		private void OnAdd(ReorderableList list)
		{
			int index = list.serializedProperty.arraySize;
			list.serializedProperty.arraySize++;
			list.index = index;
			SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
			OnAdd(list, element);
		}

		private void OnRemove(ReorderableList list)
		{
			SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(list.index);
			OnRemove(list, element);
		}

		private void OnReorder(ReorderableList list)
		{
			OnReorder(list, selectedIndex, list.index);
			selectedIndex = list.index;
		}

		private void OnSelect(ReorderableList list)
		{
			selectedIndex = list.index;
			SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(list.index);
			OnSelect(list, element);
		}

		private void OnDrawElement(Rect rect, int index, bool active, bool focused)
		{
			if (property == null || property.arraySize <= index || index < 0)
				return;

			SerializedProperty element = property.GetArrayElementAtIndex(index);

			float height = EditorGUI.GetPropertyHeight(element) + EditorGUIUtility.standardVerticalSpacing;
			RenewElementHeight(index, height);
			rect.height = height;
			rect.width -= 40;
			rect.x += 20;
			
			OnDrawElement(rect, index, active, focused, element);
		}
		private void OnDrawElementBackground(Rect rect, int index, bool active, bool focused)
		{
			if (property == null || property.arraySize <= index || index < 0)
				return;

			SerializedProperty element = property.GetArrayElementAtIndex(index);
			float height = elementHeights[index];
			rect.height = height;
			rect.width -= 4;
			rect.x += 2;

			OnDrawElementBackground(rect, index, active, focused, element, height);
		}
		#endregion

		#region Template
		protected virtual void OnAdd(ReorderableList list, SerializedProperty newElement) { }
		protected virtual void OnSelect(ReorderableList list, SerializedProperty selectedElement) { }
		protected virtual void OnRemove(ReorderableList list, SerializedProperty deleteElement)
		{
			if (EditorUtility.DisplayDialog(
				"Warning !",
				"Are you sure you want to delete:\n\r[ " + deleteElement.displayName + " ] ?",
				"Yes", "No"))
			{
				ReorderableList.defaultBehaviours.DoRemoveButton(list);
			}
		}
		protected virtual void OnReorder(ReorderableList list, int fromIndex, int toIndex){ }
		protected virtual void OnDrawElement(Rect rect, int index, bool active, bool focused, SerializedProperty element)
		{
			EditorGUI.PropertyField(rect, element, true);
		}
		protected virtual void OnDrawElementBackground(Rect rect, int index, bool active, bool focused, SerializedProperty element, float height)
		{
			if(active && backgroundImage != null)
				EditorGUI.DrawTextureTransparent(rect, backgroundImage, ScaleMode.ScaleAndCrop);
		}
		#endregion

		#region height hotfix
		private void RenewElementHeight(int index, float height)
		{
			try
			{
				elementHeights[index] = height;
			}
			catch
			{
			}
			finally
			{
				ElementListOverflowFix();
			}
		}
		private float OnCalculateItemHeight(int index)
		{
			float height = 0f;
			try
			{
				if(height != elementHeights[index])
				{
					height = elementHeights[index];
					EditorUtility.SetDirty(serializedObject.targetObject);
				}
			}
			catch
			{
			}
			finally
			{
				ElementListOverflowFix();
			}
			return height;
		}
		private void ElementListOverflowFix()
		{
			if (property.arraySize != elementHeights.Count)
			{
				float[] floats = elementHeights.ToArray();
				Array.Resize(ref floats, property.arraySize);
				elementHeights = floats.ToList();
				EditorUtility.SetDirty(serializedObject.targetObject);
			}
		}
		#endregion

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					orderList.onAddCallback -= OnAdd;
					orderList.onSelectCallback -= OnSelect;
					orderList.onRemoveCallback -= OnRemove;
					orderList.drawHeaderCallback -= OnDrawHeader;
					orderList.drawElementCallback -= OnDrawElement;
					orderList.drawElementBackgroundCallback -= OnDrawElementBackground;
					orderList.elementHeightCallback -= OnCalculateItemHeight;
					backgroundImage = null;
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~ReorderableListExtend() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion
	}
}
#endif
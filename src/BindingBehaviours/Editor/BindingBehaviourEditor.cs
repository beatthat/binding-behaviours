using UnityEngine;
using UnityEditor;
using BeatThat.App;

namespace BeatThat
{
	[CustomEditor(typeof(BindingBehaviour), true)]
	public class BindingBehaviourEditor : UnityEditor.Editor 
	{
		private bool showProperties { get; set; }
		private bool showTransitionOptions { get; set; }

		override public void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			AddBindingsFoldout();
			this.serializedObject.ApplyModifiedProperties();
		}

		protected void AddBindingsFoldout()
		{
			if(!Application.isPlaying) {
				return;
			}

			this.showProperties = EditorGUILayout.Foldout(this.showProperties, "Bindings");
			if(this.showProperties) {
				EditorGUI.indentLevel++;

				if(Application.isPlaying) {
					EditorGUILayout.LabelField("Is Bound: " + (this.target as BindingBehaviour).isBound);
				}

				if((this.target as BindingBehaviour).isBound) {
					AddAttachedBindingsFoldout();
				}

				EditorGUI.indentLevel--;
			}
		}

		private bool showAttachedBindings { get; set; }
		protected void AddAttachedBindingsFoldout()
		{
			this.showAttachedBindings = EditorGUILayout.Foldout(this.showAttachedBindings, "Attached Bindings");
			if(this.showAttachedBindings) {
				EditorGUI.indentLevel++;

				PresentAttachedBindings(this);

				EditorGUI.indentLevel--;
			}
		}

		public static void PresentAttachedBindings(UnityEditor.Editor editor)
		{
			var ctl = editor.target as BindingBehaviour;
			if(!ctl.isBound) {
				return;
			}

			using(var attached = ListPool<Binding>.Get()) {
				ctl.GetAttachedBindings(attached);

				foreach(var ab in attached) {
					EditorGUILayout.LabelField(ab.ToString());
				}
			}
		}
	}
}

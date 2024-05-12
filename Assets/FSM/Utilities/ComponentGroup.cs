#if UNITY_EDITOR
namespace FSM.Utilities
{
    using System.Collections.Generic;
    using UnityEngine;

    public class ComponentGroup : MonoBehaviour
    {
        public string groupName;
        public bool visible, edit;
        public List<Component> comps = new List<Component>();

        public void AddComponentToGroup(Component aComp)
        {
            comps.Add(aComp);
        }
    }
}
#endif
#if UNITY_EDITOR
namespace B83.EditorTools
{
    using FSM.Utilities;
    using UnityEngine;
    using UnityEditor;
    [CustomEditor(typeof(ComponentGroup))]
    public class ComponentGroupEditor : Editor
    {
        ComponentGroup group;
        private void OnEnable()
        {
            group = (ComponentGroup)target;
            for (int i = group.comps.Count - 1; i >= 0; i--)
            {
                if (group.comps[i] == null)
                    group.comps.RemoveAt(i);
            }
        }
        void ChangeVisibility(bool aVisible)
        {
            for(int i = group.comps.Count-1; i>=0; i--)
            {
                var c = group.comps[i];
                if (c == null)
                {
                    group.comps.RemoveAt(i);
                    continue;
                }
                if (aVisible)
                {
                    c.hideFlags &= ~HideFlags.HideInInspector;
                    // required if the object was deselected in between
                    Editor.CreateEditor(c);
                }
                else
                    c.hideFlags |= HideFlags.HideInInspector;
            }
            group.visible = aVisible;
        }
 
        public override void OnInspectorGUI()
        {
            var oldColor = GUI.color;
            var oldEnabled = GUI.enabled;
            if (group.edit)
            {  
                var components = group.gameObject.GetComponents<Component>();
                GUILayout.BeginHorizontal();
                group.groupName = GUILayout.TextField(group.groupName);
                if (GUILayout.Button("done",GUILayout.Width(40)))
                    group.edit = false;
                GUILayout.EndHorizontal();
                foreach (var comp in components)
                {
                    string name = comp.GetType().Name;
                    if (comp is ComponentGroup g)
                        name = "ComponentGroup("+g.groupName+")";
                    bool isInList = group.comps.Contains(comp);
                    GUI.color = isInList ? Color.green : oldColor;
                    GUI.enabled = comp != group;
                    if ((comp.hideFlags & HideFlags.HideInInspector) != 0)
                        name += "(hidden)";
                    if (GUILayout.Toggle(isInList, name) != isInList)
                    {
                        if (isInList)
                            group.comps.Remove(comp);
                        else
                            group.comps.Add(comp);
                    }
                }
                GUI.enabled = oldEnabled;
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUI.color = group.visible ? Color.green : Color.red;
                if (GUILayout.Button(group.groupName))
                    ChangeVisibility(!group.visible);
                GUI.color = Color.red;
                if (GUILayout.Button("hide", GUILayout.Width(40)))
                    ChangeVisibility(false);
                GUI.color = Color.green;
                if (GUILayout.Button("show", GUILayout.Width(40)))
                    ChangeVisibility(true);
                GUI.color = oldColor;
                if (GUILayout.Button("edit", GUILayout.Width(40)))
                {
                    ChangeVisibility(true);
                    group.edit = true;
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}
#endif

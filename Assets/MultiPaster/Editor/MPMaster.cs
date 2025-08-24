using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MultiPaster
{
    public static class MPMaster
    {
        
        public static void MultiPaste(List<Component> components, List<GameObject> targetObjs)
        {
            
            Undo.RecordObjects(targetObjs.ToArray(), "Multi Paste");

            foreach (var item in components)
            {
                if (item != null)
                {
                    System.Type componentType = item.GetType();
                    UnityEditorInternal.ComponentUtility.CopyComponent(item);

                    foreach (var objs in targetObjs)
                    {
                        if (objs != null)
                        {
                           
                            Undo.AddComponent(objs, componentType);
                            

                            Component[] targetComponents = objs.GetComponents(componentType);

                            UnityEditorInternal.ComponentUtility.PasteComponentValues(targetComponents[targetComponents.Length - 1]);

                            EditorUtility.SetDirty(objs);
                        }
                    }
                }
            }
            
            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());

        }
    }
}

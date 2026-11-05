using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Menu: BroncoVerse > VR Keyboard > Add Keyboard To Text Fields In Open Scene
//
// Hooks the XR Interaction Toolkit "Spatial Keyboard" sample up to every
// TMP input field in the open scene, so selecting a field (controller ray or
// poke) pops up the floating keyboard and typed keys go into that field.
//
// It finds the sample's scripts by name, so this file still compiles before
// the sample is imported. Import it first:
//   Window > Package Manager > XR Interaction Toolkit > Samples > Spatial Keyboard > Import
public static class VRKeyboardSetup
{
    const string DisplayTypeName = "XRKeyboardDisplay";
    const string GlobalKeyboardTypeName = "GlobalNonNativeKeyboard";
    const string MenuPath = "BroncoVerse/VR Keyboard/Add Keyboard To Text Fields In Open Scene";

    [MenuItem(MenuPath)]
    public static void AddKeyboardToOpenScene()
    {
        Type displayType = FindType(DisplayTypeName);
        Type globalType = FindType(GlobalKeyboardTypeName);
        if (displayType == null || globalType == null)
        {
            EditorUtility.DisplayDialog("Spatial Keyboard sample not found",
                "Import it first:\n\nWindow > Package Manager > XR Interaction Toolkit > " +
                "Samples > Spatial Keyboard > Import\n\nThen run this menu item again.", "OK");
            return;
        }

        int added = 0;

        // 1. One shared keyboard per scene (the sample's GlobalNonNativeKeyboard prefab).
        if (UnityEngine.Object.FindFirstObjectByType(globalType, FindObjectsInactive.Include) == null)
        {
            GameObject prefab = FindPrefabWithComponent(globalType);
            if (prefab == null)
            {
                Debug.LogError("[VRKeyboardSetup] Could not find the GlobalNonNativeKeyboard prefab in the imported sample.");
                return;
            }
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            Undo.RegisterCreatedObjectUndo(instance, "Add VR keyboard");
            Debug.Log("[VRKeyboardSetup] Added " + instance.name + " to the scene.", instance);
        }

        // 2. A keyboard display on every TMP input field, pointing at its own field.
        var fields = UnityEngine.Object.FindObjectsByType<TMP_InputField>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (TMP_InputField field in fields)
        {
            if (EditorUtility.IsPersistent(field) || field.GetComponent(displayType) != null)
                continue;

            Component display = Undo.AddComponent(field.gameObject, displayType);
            var so = new SerializedObject(display);
            SetObject(so, "m_InputField", field);
            SetBool(so, "m_UpdateOnKeyPress", true);
            SetBool(so, "m_MonitorInputFieldCharacterLimit", true);
            so.ApplyModifiedProperties();
            added++;
            Debug.Log("[VRKeyboardSetup] Keyboard hooked to " + GetPath(field.transform), field);
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorUtility.DisplayDialog("VR keyboard",
            $"Hooked the keyboard to {added} new text field(s).\n\nSave the scene (Ctrl+S) to keep the change.", "OK");
    }

    static Type FindType(string name) =>
        TypeCache.GetTypesDerivedFrom<MonoBehaviour>().FirstOrDefault(t => t.Name == name);

    static GameObject FindPrefabWithComponent(Type type)
    {
        foreach (string guid in AssetDatabase.FindAssets("t:Prefab Keyboard"))
        {
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
            if (go != null && go.GetComponent(type) != null)
                return go;
        }
        return null;
    }

    static void SetObject(SerializedObject so, string prop, UnityEngine.Object value)
    {
        var p = so.FindProperty(prop);
        if (p != null) p.objectReferenceValue = value;
        else Debug.LogWarning($"[VRKeyboardSetup] {so.targetObject.GetType().Name} has no field {prop}; set it by hand.");
    }

    static void SetBool(SerializedObject so, string prop, bool value)
    {
        var p = so.FindProperty(prop);
        if (p != null) p.boolValue = value;
    }

    static string GetPath(Transform t) => t.parent == null ? t.name : GetPath(t.parent) + "/" + t.name;
}

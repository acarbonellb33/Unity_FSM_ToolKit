#if UNITY_EDITOR
namespace FSM.Utilities
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    /// <summary>
    /// Class for loading unique ID's to every object everytime the hierarchy changes.
    /// </summary>
    [InitializeOnLoad]
    public static class HierarchyChecker
    {
        static HierarchyChecker()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }
        
        private static void OnHierarchyChanged()
        {
            GameObject[] rootGameObjects =
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

            List<string> generators = new List<string>();
            
            foreach (GameObject rootGameObject in rootGameObjects)
            {
                EnsureIDGeneratorRecursive(rootGameObject, generators);
            }
        }
        private static void EnsureIDGeneratorRecursive(GameObject gameObject, List<string> generators)
        {
            if (gameObject.GetComponent<IDGenerator>() == null)
            {
                // Add IDGenerator component if not present
                IDGenerator generator = gameObject.AddComponent<IDGenerator>();
                generator.GetUniqueID();
            }
            if(generators.Contains(gameObject.GetComponent<IDGenerator>().GetUniqueID()))
            {
                gameObject.GetComponent<IDGenerator>().GenerateNewID();
            }
            
            generators.Add(gameObject.GetComponent<IDGenerator>().GetUniqueID());
            
            // Recursively check child GameObjects
            foreach (Transform child in gameObject.transform)
            {
                EnsureIDGeneratorRecursive(child.gameObject, generators);
            }
        }
    }
}
#endif
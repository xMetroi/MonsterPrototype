using UnityEditor;
using UnityEngine;

public class PrefabVariantGenerator : MonoBehaviour
{
    [MenuItem("Assets/Create/Monsters/Attacks/New Throwable Prefab")]
    public static void CreateProjectilePrefabVariant()
    {
        // Ruta del prefab original (modifica esta ruta según sea necesario)
        string originalPrefabPath = "Assets/Prefabs/Attacks/Base/BaseProjectile.prefab";
        GameObject originalPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(originalPrefabPath);

        if (originalPrefab == null)
        {
            Debug.LogError("No se pudo encontrar el prefab en la ruta especificada: " + originalPrefabPath);
            return;
        }

        // Crear la instancia del prefab original
        GameObject prefabInstance = PrefabUtility.InstantiatePrefab(originalPrefab) as GameObject;

        // Establecer la carpeta predeterminada
        string defaultFolder = "Assets/Prefabs/Attacks/Projectiles";
        string defaultFileName = originalPrefab.name + "_Variant.prefab";
        string newPath = EditorUtility.SaveFilePanelInProject("Guardar Projectile Prefab", defaultFileName, "prefab", "Por favor, selecciona una ubicación para guardar el prefab variante.", defaultFolder);

        if (string.IsNullOrEmpty(newPath))
        {
            // Si el usuario cancela el guardado, destruir la instancia y salir
            DestroyImmediate(prefabInstance);
            return;
        }

        // Guardar la instancia como un nuevo prefab variante
        PrefabUtility.SaveAsPrefabAsset(prefabInstance, newPath);

        // Destruir la instancia temporal
        DestroyImmediate(prefabInstance);

        Debug.Log("Projectile Prefab variant created and saved at: " + newPath);
    }

    [MenuItem("Assets/Create/Monsters/Attacks/New Attack Prefab")]
    public static void CreateAttackPrefabVariant()
    {
        // Ruta del prefab original (modifica esta ruta según sea necesario)
        string originalPrefabPath = "Assets/Prefabs/Attacks/Base/BaseMelee.prefab";
        GameObject originalPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(originalPrefabPath);

        if (originalPrefab == null)
        {
            Debug.LogError("No se pudo encontrar el prefab en la ruta especificada: " + originalPrefabPath);
            return;
        }

        // Crear la instancia del prefab original
        GameObject prefabInstance = PrefabUtility.InstantiatePrefab(originalPrefab) as GameObject;

        // Establecer la carpeta predeterminada
        string defaultFolder = "Assets/Prefabs/Attacks/Melee";
        string defaultFileName = originalPrefab.name + "_Variant.prefab";
        string newPath = EditorUtility.SaveFilePanelInProject("Guardar Melee Prefab", defaultFileName, "prefab", "Por favor, selecciona una ubicación para guardar el prefab variante.", defaultFolder);

        if (string.IsNullOrEmpty(newPath))
        {
            // Si el usuario cancela el guardado, destruir la instancia y salir
            DestroyImmediate(prefabInstance);
            return;
        }

        // Guardar la instancia como un nuevo prefab variante
        PrefabUtility.SaveAsPrefabAsset(prefabInstance, newPath);

        // Destruir la instancia temporal
        DestroyImmediate(prefabInstance);

        Debug.Log("Melee Prefab variant created and saved at: " + newPath);
    }
}

using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Monsters/Stats/New Attack")]
public class Attack : ScriptableObject
{
    /// <summary>
    /// Enum of the attack type
    /// </summary>
    public enum AttackType
    {
        Melee,
        Throwable,
        Transformation
    }

    [Header("Attack Info")]
    public int attackId;
    public string attackName;
    public string attackDescription;
    public AttackType attackType;

    [Header("Attack Properties")]
    public float attackDamage;
    public float attackCooldown;
    public float attackKnockback;

    //SOLO SALDRA EN EL INSPECTOR SI EL ATAQUE ES DE TIPO MELEE
    #region Melee Variables
    [Header("Melee Properties")]
    public float meleeRange;
    public GameObject meleePrefab;
    
    #endregion

    //SOLO SALDRA EN EL INSPECTOR SI EL ATAQUE ES DE TIPO THROWABLE
    #region Throwable Variables
    [Header("Throwable Properties")]
    public float throwableSpeed;
    public GameObject throwablePrefab;
    #endregion

    //SOLO SALDRA EN EL INSPECTOR SI EL ATAQUE ES DE TIPO TRANSFORMATION
    #region Transformation Variables
    [Header("Transformation Properties")]
    public float transformationDamageMultiplier;
    public float transformationResistanceMultiplier;
    public float transformationSpeedMultiplier;
    public float transformationDuration;
    public GameObject prefabTransformation;
    #endregion

    /// <summary>
    /// Cada que se añada una variable al scriptable object añade
    /// tambien la variable en esta parte para que salga en el inspector
    /// </summary>
#if UNITY_EDITOR
    [CustomEditor(typeof(Attack))]
    public class attackEditor : Editor
    {
        SerializedProperty attackTypeProp;

        private void OnEnable()
        {
            attackTypeProp = serializedObject.FindProperty("attackType");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Mostrar el campo de Enum para la categoría
            EditorGUILayout.PropertyField(attackTypeProp);

            // Mostrar propiedades generales
            EditorGUILayout.PropertyField(serializedObject.FindProperty("attackId"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("attackName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("attackDescription"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("attackDamage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("attackCooldown"));

            // Mostrar propiedades específicas según la categoría
            switch ((AttackType)attackTypeProp.enumValueIndex)
            {
                case AttackType.Melee:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("meleeRange"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("meleePrefab"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("attackKnockback"));
                    break;

                case AttackType.Throwable:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("throwableSpeed"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("throwablePrefab"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("attackKnockback"));
                    break;

                case AttackType.Transformation:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("transformationDamageMultiplier"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("transformationResistanceMultiplier"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("transformationSpeedMultiplier"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("transformationDuration"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("prefabTransformation"));
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}

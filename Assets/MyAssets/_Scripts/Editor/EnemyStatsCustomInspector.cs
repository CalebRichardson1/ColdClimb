using UnityEngine;
using UnityEditor;
using ColdClimb.StateMachines;
using UnityEditor.Rendering;

namespace ColdClimb.DevTools{
    [CustomEditor(typeof(EnemyStats))]
    public class EnemyStatsCustomInspector : Editor{
#region Serialized Properties
        // Lore
        private SerializedProperty enemyName;
        private SerializedProperty enemyDescription;
        private SerializedProperty enemyType;
        
        // Base Stats
        private SerializedProperty health;
        private SerializedProperty attackDamage;
        private SerializedProperty attackSpeed;
        private SerializedProperty minAttackRange;
        private SerializedProperty maxAttackRange;
        private SerializedProperty walkingMovementSpeed;
        private SerializedProperty runningMovementSpeed;

        // Detection Stats
        private SerializedProperty targetRegistrationTime;
        private SerializedProperty maxSpotRange;
        private SerializedProperty immediateChaseRange;
        private SerializedProperty softChaseRange;
        private SerializedProperty minSearchTime;
        private SerializedProperty maxSearchTime;

        // Enemy Definitions
        private SerializedProperty patrolType;
        private SerializedProperty goalType;
        private SerializedProperty chaseType;
        private SerializedProperty attackType;
#endregion

        private bool loreGroup, baseStatGroup, detectionStatsGroup, definitionGroup; 

        private void OnEnable() {
            // Lore
            enemyName = serializedObject.FindProperty("enemyName");
            enemyDescription = serializedObject.FindProperty("enemyDescription");
            enemyType = serializedObject.FindProperty("enemyType");

            // Base Stats
            health = serializedObject.FindProperty("health");
            attackDamage = serializedObject.FindProperty("attackDamage");
            attackSpeed = serializedObject.FindProperty("attackSpeed");
            minAttackRange = serializedObject.FindProperty("minAttackRange");
            maxAttackRange = serializedObject.FindProperty("maxAttackRange");
            walkingMovementSpeed = serializedObject.FindProperty("walkingMovementSpeed");
            runningMovementSpeed = serializedObject.FindProperty("runningMovementSpeed");

            // Detection Stats
            targetRegistrationTime = serializedObject.FindProperty("targetRegistrationTime");
            maxSpotRange = serializedObject.FindProperty("maxSpotRange");
            immediateChaseRange = serializedObject.FindProperty("immediateChaseRange");
            softChaseRange = serializedObject.FindProperty("softChaseRange");
            minSearchTime = serializedObject.FindProperty("minSearchTime");
            maxSearchTime = serializedObject.FindProperty("maxSearchTime");

            // Enemy Definitions
            patrolType = serializedObject.FindProperty("patrolType");
            goalType = serializedObject.FindProperty("goalType");
            chaseType = serializedObject.FindProperty("chaseType");
            attackType = serializedObject.FindProperty("attackType");          
        }


        public override void OnInspectorGUI(){
            serializedObject.Update();
            EnemyStats enemyStats = (EnemyStats)target;

            // Lore
            loreGroup = EditorGUILayout.BeginFoldoutHeaderGroup(loreGroup, "Enemy Lore", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13});
            if(loreGroup){
                EditorGUILayout.Space();
                enemyName.stringValue = EditorGUILayout.TextField(new GUIContent("Name", "Name of the enemy."), enemyStats.enemyName, GUILayout.ExpandWidth(true));
                GUILayout.Label("Description", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Normal, fontSize = 12}, GUILayout.ExpandWidth(true));
                enemyDescription.stringValue = EditorGUILayout.TextArea(enemyStats.enemyDescription, GUILayout.ExpandWidth(true), GUILayout.Height(100));
                enemyType.enumValueIndex = (int)(EnemyType)EditorGUILayout.EnumPopup(new GUIContent("Enemy Type", "The type of enemy."), enemyStats.enemyType);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            // Base Stats
            baseStatGroup = EditorGUILayout.BeginFoldoutHeaderGroup(baseStatGroup, "Base Stats", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13});
            if(baseStatGroup){
                EditorGUILayout.Space();
                health.intValue = EditorGUILayout.IntSlider(new GUIContent("Health", "Base max health of the enemy, can be restored or damaged until the enemy dies."), health.intValue, 1, 250, GUILayout.ExpandWidth(false));
                attackDamage.floatValue = EditorGUILayout.Slider(new GUIContent("Attack Damage", "Base damage of the enemy, different attacks can change the damage value."), attackDamage.floatValue, 1f, 100f, GUILayout.ExpandWidth(false));
                attackSpeed.floatValue = EditorGUILayout.Slider(new GUIContent("Attack Speed", "Base attack speed of the enemy, in seconds."), attackSpeed.floatValue, 0.1f, 10f, GUILayout.ExpandWidth(false));
                
                minAttackRange.floatValue = EditorGUILayout.Slider(new GUIContent("Minimum Attack Range", "The minimum range the enemy must be from a target to be able to attack."), minAttackRange.floatValue, 1f, 40f, GUILayout.ExpandWidth(true));
                maxAttackRange.floatValue = EditorGUILayout.Slider(new GUIContent("Maximum Attack Range", "The maximum range the enemy can be from a target to be able to attack."), maxAttackRange.floatValue, enemyStats.minAttackRange, 40f, GUILayout.ExpandWidth(true));
                maxAttackRange.floatValue = maxAttackRange.floatValue < minAttackRange.floatValue ?
                                                maxAttackRange.floatValue = minAttackRange.floatValue : maxAttackRange.floatValue;
                
                walkingMovementSpeed.floatValue = EditorGUILayout.Slider(new GUIContent("Walking Speed", "The speed in which the enemy uses while patroling/searching."), walkingMovementSpeed.floatValue, 1f, 30f, GUILayout.ExpandWidth(true));
                runningMovementSpeed.floatValue = EditorGUILayout.Slider(new GUIContent("Running Speed", "The speed in which the enemy uses while chasing/retreating."), runningMovementSpeed.floatValue, enemyStats.walkingMovementSpeed, 40f, GUILayout.ExpandWidth(true));
                runningMovementSpeed.floatValue = runningMovementSpeed.floatValue < walkingMovementSpeed.floatValue ?
                                                runningMovementSpeed.floatValue = walkingMovementSpeed.floatValue : runningMovementSpeed.floatValue;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            // Detection Stats
            detectionStatsGroup = EditorGUILayout.BeginFoldoutHeaderGroup(detectionStatsGroup, "Detection Stats", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13});
            if(detectionStatsGroup){
                targetRegistrationTime.floatValue = EditorGUILayout.Slider(new GUIContent("Target Registration Time", "The amount of time a target has to be in LOS of the enemy before switching to the chase state."), targetRegistrationTime.floatValue, 0.1f, 5f, GUILayout.ExpandWidth(false));
                maxSpotRange.floatValue = EditorGUILayout.Slider(new GUIContent("Max Spot Range", "The max distance the enemy can see a target if the target is in LOS of the enemy."), maxSpotRange.floatValue, 1f, 300f, GUILayout.ExpandWidth(false));
            
                immediateChaseRange.floatValue = EditorGUILayout.Slider(new GUIContent("Immediate Chase Range", "If a target enters this range the enemy will enter chase state, regardless of LOS."), immediateChaseRange.floatValue, 1f, 300f, GUILayout.ExpandWidth(true));
                softChaseRange.floatValue = EditorGUILayout.Slider(new GUIContent("Soft Chase Range", "If a target enters this range the enemy will enter chase state as long as the enemy has LOS of the target."), softChaseRange.floatValue, immediateChaseRange.floatValue, 300f, GUILayout.ExpandWidth(true));
                softChaseRange.floatValue = softChaseRange.floatValue < immediateChaseRange.floatValue ?
                                                softChaseRange.floatValue = immediateChaseRange.floatValue : softChaseRange.floatValue;
                
                minSearchTime.floatValue = EditorGUILayout.Slider(new GUIContent("Minimum Search Time", "The minimum amount of time (in seconds) the enemy will search for a target if entered search state."), minSearchTime.floatValue, 1f, 300f, GUILayout.ExpandWidth(true));
                maxSearchTime.floatValue = EditorGUILayout.Slider(new GUIContent("Maximum Search Time", "The maximim amount of time (in seconds) the enemy will search for a target if entered search state."), maxSearchTime.floatValue, minSearchTime.floatValue, 330f, GUILayout.ExpandWidth(true));
                maxSearchTime.floatValue = maxSearchTime.floatValue < minSearchTime.floatValue ?
                                                maxSearchTime.floatValue = minSearchTime.floatValue : maxSearchTime.floatValue;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);


            // Enemy definitions
            definitionGroup = EditorGUILayout.BeginFoldoutHeaderGroup(definitionGroup, "Enemy Definitions", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13});
            if(definitionGroup){
                patrolType.enumValueIndex = (int)(PatrolType)EditorGUILayout.EnumPopup(new GUIContent("Patrol Type", "The behaviour that the enemy will perform in the patrol state."), enemyStats.patrolType);
                if(enemyStats.patrolType == PatrolType.GoalOriented){
                    goalType.enumValueIndex = (int)(GoalType)EditorGUILayout.EnumPopup(new GUIContent("Goal Type", "The type of goal the enemy will search for while in patrol state."), enemyStats.goalType);
                }
                chaseType.enumValueIndex = (int)(ChaseType)EditorGUILayout.EnumPopup(new GUIContent("Chase Type", "The behaviour that the enemy will perform in the chase state."), enemyStats.chaseType);
                attackType.enumValueIndex = (int)(AttackType)EditorGUILayout.EnumPopup(new GUIContent("Attack Type", "The behaviour that the enemy will perform in the attack state."), enemyStats.attackType);
                // TO-DO: Add more varaibles based on the enemy definition
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

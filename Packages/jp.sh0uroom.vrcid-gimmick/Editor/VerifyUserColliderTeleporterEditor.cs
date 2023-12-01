using UnityEngine;

using UnityEditor;
using UdonSharpEditor;

namespace sh0uRoom.VRCIDGimmick
{
    [CustomEditor(typeof(VerifyUserColliderTeleporter))]
    public class VerifyUserColliderTeleporterEditor : Editor
    {
        private SerializedProperty targetColliders;
        private SerializedProperty targetBone;
        private SerializedProperty teleportPos;
        private SerializedProperty checkInterval;

        private SerializedProperty loader;
        private SerializedProperty isWhiteList;
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

            serializedObject.Update();

            GetProperties();
            ShowGUI();

            serializedObject.ApplyModifiedProperties();
        }

        private void GetProperties()
        {
            targetColliders = serializedObject.FindProperty("targetColliders");
            targetBone = serializedObject.FindProperty("targetBone");
            teleportPos = serializedObject.FindProperty("teleportPos");
            checkInterval = serializedObject.FindProperty("checkInterval");

            loader = serializedObject.FindProperty("loader");
            isWhiteList = serializedObject.FindProperty("isWhiteList");
        }

        private void ShowGUI()
        {
            EditorGUILayout.HelpBox("プレイヤーがCollider範囲内に接触するとテレポートします\nColliderのisTriggerにチェックをつけてください", MessageType.Info);
            EditorGUILayout.PropertyField(targetColliders, new GUIContent("対象Collider"));
            EditorGUILayout.PropertyField(teleportPos, new GUIContent("テレポート先"));

            if (targetColliders.arraySize == 0)
            {
                EditorGUILayout.HelpBox("対象のコライダーが設定されていません", MessageType.Error);
            }
            else
            {
                for (var i = 0; i < targetColliders.arraySize; i++)
                {
                    var collider = targetColliders.GetArrayElementAtIndex(i);
                    if (collider.objectReferenceValue == null)
                    {
                        EditorGUILayout.HelpBox($"[Element {i}] 対象のコライダーが設定されていません", MessageType.Error);
                        return;
                    }
                    else
                    {
                        var colliderObj = collider.objectReferenceValue as Collider;
                        if (!colliderObj.isTrigger)
                        {
                            EditorGUILayout.HelpBox($"[Element {i}] {colliderObj.name}のisTriggerにチェックをつけてください", MessageType.Error);
                            return;
                        }
                    }
                }
                if (teleportPos.objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox("テレポート先が設定されていません", MessageType.Error);
                }
                else
                {
                    EditorGUILayout.Space(10);
                    EditorGUILayout.PropertyField(targetBone, new GUIContent("当たり判定Bone"));
                    EditorGUILayout.PropertyField(checkInterval, new GUIContent("判定頻度(秒)"));

                    EditorGUILayout.Space(10);
                    EditorGUILayout.HelpBox("UserIDLoaderを指定することで、特定のプレイヤーのみ除外することができます\nホワイトリスト化すると、IDリストに含まれているプレイヤーのみテレポートさせます", MessageType.Info);
                    EditorGUILayout.PropertyField(loader, new GUIContent("（オプション）除外するIDリスト"));
                    EditorGUILayout.PropertyField(isWhiteList, new GUIContent("ホワイトリスト化"));
                }
            }
        }
    }
}

using UnityEngine;

using UnityEditor;
using UdonSharpEditor;

namespace sh0uRoom.VRCIDGimmick
{
    [CustomEditor(typeof(VerifyUserTeleporter))]
    public class VerifyUserTeleporterEditor : Editor
    {
        private SerializedProperty loader;
        private SerializedProperty isBlackList;

        private SerializedProperty teleportPos;
        private SerializedProperty isTeleportOnStart;

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
            loader = serializedObject.FindProperty("loader");
            isBlackList = serializedObject.FindProperty("isBlackList");

            teleportPos = serializedObject.FindProperty("teleportPos");
            isTeleportOnStart = serializedObject.FindProperty("isTeleportOnStart");
        }

        private void ShowGUI()
        {
            EditorGUILayout.HelpBox("動作にはUserIDNetLoaderが必要です\nブラックリスト化すると、IDリストに含まれているプレイヤー以外をテレポートさせます", MessageType.Info);
            EditorGUILayout.PropertyField(loader, new GUIContent("参照するIDリスト"));

            if (loader.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("UserIDNetLoaderが指定されていません", MessageType.Error);
            }
            else
            {
                EditorGUILayout.PropertyField(isBlackList, new GUIContent("ブラックリスト化"));

                EditorGUILayout.Space(10);
                EditorGUILayout.PropertyField(teleportPos, new GUIContent("テレポート先"));
                if (teleportPos.objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox("テレポート先が指定されていません", MessageType.Error);
                }
                else
                {
                    EditorGUILayout.PropertyField(isTeleportOnStart, new GUIContent("開始時自動テレポート"));
                }
            }
        }
    }
}

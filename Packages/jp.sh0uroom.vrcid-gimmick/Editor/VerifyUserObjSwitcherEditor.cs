using UnityEngine;

using UnityEditor;
using UdonSharpEditor;

namespace sh0uRoom.VRCIDGimmick
{
    [CustomEditor(typeof(VerifyUserObjSwitcher))]
    public class VerifyUserObjSwitcherEditor : Editor
    {
        private SerializedProperty loader;
        private SerializedProperty targetObj;
        private SerializedProperty isNetworking;
        private SerializedProperty isOnceOnly;
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
            targetObj = serializedObject.FindProperty("targetObj");
            isNetworking = serializedObject.FindProperty("isNetworking");
            isOnceOnly = serializedObject.FindProperty("isOnceOnly");
        }

        private void ShowGUI()
        {
            EditorGUILayout.HelpBox("動作にはUserIDNetLoaderが必要です", MessageType.Info);
            EditorGUILayout.PropertyField(loader, new GUIContent("IDリスト"));
            EditorGUILayout.PropertyField(targetObj, new GUIContent("対象オブジェクト"));
            if (loader.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("UserIDNetLoaderが設定されていません", MessageType.Error);
            }
            else if (targetObj.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("対象オブジェクトが設定されていません", MessageType.Error);
            }
            else
            {
                EditorGUILayout.PropertyField(isOnceOnly, new GUIContent("一回だけ動作"));
                EditorGUILayout.PropertyField(isNetworking, new GUIContent("表示状態を同期(beta)"));
            }
        }
    }
}

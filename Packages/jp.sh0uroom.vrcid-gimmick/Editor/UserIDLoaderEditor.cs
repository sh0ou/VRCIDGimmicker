using UnityEngine;
using UnityEditor;
using UdonSharpEditor;

namespace sh0uRoom.VRCIDGimmick
{
    [CustomEditor(typeof(UserIDLoader))]
    public class UserIDLoaderEditor : Editor
    {
        private bool isOutputText;

        private SerializedProperty isInputURL;
        private SerializedProperty targetURL;
        private SerializedProperty isOutputLog;
        private SerializedProperty textUI;
        private SerializedProperty userIDs;

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

            serializedObject.Update();

            GetProperties();
            ShowBaseGUI();

            serializedObject.ApplyModifiedProperties();
        }

        private void GetProperties()
        {
            isInputURL = serializedObject.FindProperty("isInputURL");
            isOutputLog = serializedObject.FindProperty("isOutputLog");
            textUI = serializedObject.FindProperty("textUI");

            targetURL = serializedObject.FindProperty("targetURL");
            userIDs = serializedObject.FindProperty("userIDs");
        }

        private void ShowBaseGUI()
        {
            //URLで指定するか、手動で指定するか選択するボタン
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("IDの設定方法");
                if (isInputURL.boolValue)
                {
                    if (GUILayout.Button("URLから取得する"))
                    {
                        isInputURL.boolValue = false;
                    }
                }
                else
                {
                    if (GUILayout.Button("手動で指定する"))
                    {
                        isInputURL.boolValue = true;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            //URLから取得する場合
            if (isInputURL.boolValue)
            {
                ShowURLInputGUI();
            }
            else
            {
                ShowManualInputGUI();
            }

            //Textに出力するかどうか
            if (isOutputText || textUI.objectReferenceValue != null)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(textUI, new GUIContent("出力先のTextMeshPro"));
                    if (GUILayout.Button("x", GUILayout.Width(40)))
                    {
                        textUI.objectReferenceValue = null;
                        isOutputText = false;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                if (GUILayout.Button("IDをTextUIへ出力する"))
                {
                    isOutputText = true;
                }
            }
        }

        private void ShowURLInputGUI()
        {
            EditorGUILayout.HelpBox("VRChatIDが記載されたテキストファイルのURLを指定して下さい\nIDはカンマ(,)区切りで複数指定できます\n取得したデータはuserIDsから参照できます", MessageType.Info, true);
            EditorGUILayout.PropertyField(targetURL, new GUIContent("URL"));
            isOutputLog.boolValue = EditorGUILayout.Toggle("取得したIDをログに出力する?", isOutputLog.boolValue);
        }

        private void ShowManualInputGUI()
        {
            //Headerに記載してた内容を表示する
            EditorGUILayout.HelpBox("VRChatIDを手動で指定して下さい\n取得したデータはuserIDsから参照できます", MessageType.Info, true);

            //配列の要素数を指定する
            EditorGUILayout.PropertyField(userIDs, new GUIContent("IDリスト"), true);
        }
    }
}

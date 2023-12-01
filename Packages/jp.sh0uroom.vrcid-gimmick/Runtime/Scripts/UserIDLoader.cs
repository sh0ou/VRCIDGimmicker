using UdonSharp;
using UnityEngine;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
using TMPro;

namespace sh0uRoom.VRCIDGimmick
{
    [HelpURL("https://creators.vrchat.com/worlds/udon/string-loading")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    /// <summary>
    /// URLを指定してユーザーIDを取得する
    /// </summary>
    public class UserIDLoader : UdonSharpBehaviour
    {
        [UdonSynced] public bool isURLLoaded = false;
        public string[] userIDs;

        [UdonSynced, SerializeField] private VRCUrl targetURL;
        [SerializeField] private bool isInputURL;
        [SerializeField] private bool isOutputLog = false;
        [SerializeField] private TextMeshProUGUI textUI;
        private string outputText = "";

        private void Start()
        {
            if (isInputURL)
            {
                LoadURLText();
            }
            else
            {
                OutputIDText(userIDs);
            }
        }

        private void Update()
        {
            if (!isURLLoaded)
            {
                //データ取得チェック
                if (!string.IsNullOrEmpty(outputText))
                {
                    ConvertStringToArray(outputText);
                    Debug.Log($"[<color=#73ff9a>{nameof(UserIDLoader)}</color>]データ取得完了");
                    isURLLoaded = true;
                }
            }
        }

        /// <summary> ユーザーIDと配列を照合する </summary>
        public bool CheckUserIDValid(bool isBlackList = false)
        {
            foreach (var userID in userIDs)
            {
                if (userID == Networking.LocalPlayer.displayName)
                {
                    return isBlackList ? false : true;
                }
            }
            return isBlackList ? true : false;
        }

        public void LoadURLText() => SendCustomNetworkEvent(NetworkEventTarget.All, nameof(OnLoadURLText));

        /// <summary> リンク先のテキストデータを読み込む </summary>
        public void OnLoadURLText()
        {
            //httpsチェック
            var url = targetURL.Get();
            if (url.StartsWith("https://") == false)
            {
                Debug.LogError($"[<color=magenta>{nameof(UserIDLoader)}</color>]httpsで始まるURLではありません - {gameObject.name}");
                return;
            }
            else if (url.EndsWith(".txt") == false)
            {
                Debug.LogError($"[<color=magenta>{nameof(UserIDLoader)}</color>]txtファイルではありません - {gameObject.name}");
                return;
            }

            VRCStringDownloader.LoadUrl(targetURL, (IUdonEventReceiver)this);
            Debug.Log($"[<color=#73ff9a>{nameof(UserIDLoader)}</color>]Loading...");
        }

        /// <summary>
        /// テキストを配列に変換する
        /// </summary>
        /// <param name="player"></param>
        public void ConvertStringToArray(string str) => userIDs = str.Split(',');

        private void OutputIDText(string[] strs)
        {
            textUI.text = "";
            //IDごとに改行
            foreach (var id in strs)
            {
                textUI.text += id + "\n";
            }
        }

        #region VRCMethods
        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (!isInputURL) return;

            // プレイヤーが参加したらURL読み込み
            if (isURLLoaded)
            {
                RequestSerialization();
                OnLoadURLText();
            }
        }

        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            // HTMLタグが含まれていたらエラー
            if (outputText.Contains("<!DOCTYPE html>"))
            {
                Debug.LogError($"[<color=magenta>{nameof(UserIDLoader)}</color>]HTMLタグを検出しました。サイト上にエラーが発生しているか、間違ったURLを指定している可能性があります。 - {gameObject.name}");
                return;
            }

            Debug.Log($"[<color=73ff9a>{nameof(UserIDLoader)}</color>]Complete");

            // 結果出力
            ConvertStringToArray(outputText = result.Result);
            // ログ出力
            if (isOutputLog)
            {
                Debug.Log($"[<color=#73ff9a>{nameof(UserIDLoader)}</color>]取得内容: {result.Result} - {gameObject.name}");
            }
            // Textに出力
            if (textUI != null)
            {
                var str = result.Result.Split(',');
                OutputIDText(str);
            }
        }

        public override void OnStringLoadError(IVRCStringDownload result)
        {
            Debug.Log($"[<color=magenta>{nameof(UserIDLoader)}</color>]{result.ErrorCode}({result.Error}) - {gameObject.name}");
        }
        #endregion
    }
}
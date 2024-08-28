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
        [SerializeField] private TextMeshProUGUI[] textUIs;
        private string outputText = "";

        private const string DEBUG_PREFIX = "[<color=#73ff9a>UserIDLoader</color>]";
        private const string DEBUG_PREFIX_ERR = "[<color=magenta>UserIDLoader</color>]";

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
                    Debug.Log($"{DEBUG_PREFIX}データ取得完了");
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
                Debug.LogError($"{DEBUG_PREFIX_ERR}httpsで始まるURLではありません - {gameObject.name}");
                return;
            }
            else if (url.EndsWith(".txt") == false)
            {
                Debug.LogError($"{DEBUG_PREFIX_ERR}txtファイルではありません - {gameObject.name}");
                return;
            }

            VRCStringDownloader.LoadUrl(targetURL, (IUdonEventReceiver)this);
            Debug.Log($"{DEBUG_PREFIX}Loading...");
        }

        /// <summary>
        /// テキストを配列に変換する
        /// </summary>
        /// <param name="player"></param>
        public void ConvertStringToArray(string str) => userIDs = str.Split(',');

        private void OutputIDText(string[] strs)
        {
            if (textUIs == null) return;

            var str = "";
            //IDごとに改行
            foreach (var id in strs)
            {
                str += id + "\n";
            }

            foreach (var textUI in textUIs)
            {
                textUI.text = str;
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
                Debug.LogError($"{DEBUG_PREFIX_ERR}HTMLタグを検出しました。サイト上にエラーが発生しているか、間違ったURLを指定している可能性があります。 - {gameObject.name}");
                return;
            }

            Debug.Log($"{DEBUG_PREFIX}Complete");

            // 結果出力
            ConvertStringToArray(outputText = result.Result);
            // ログ出力
            if (isOutputLog)
            {
                Debug.Log($"{DEBUG_PREFIX}取得内容: {result.Result} - {gameObject.name}");
            }
            // Textに出力
            if (textUIs != null)
            {
                var str = result.Result.Split(',');
                OutputIDText(str);
            }
        }

        public override void OnStringLoadError(IVRCStringDownload result)
        {
            Debug.LogError($"{DEBUG_PREFIX_ERR}{result.ErrorCode}({result.Error}) - {gameObject.name}");
        }
        #endregion
    }
}
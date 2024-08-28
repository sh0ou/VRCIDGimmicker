using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace sh0uRoom.VRCIDGimmick
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VerifyUserTeleporter : UdonSharpBehaviour
    {
        [SerializeField] private UserIDLoader loader;
        [SerializeField] private bool isBlackList;
        [SerializeField] private Transform teleportPos;
        [SerializeField] private bool isTeleportOnStart;
        private const string DEBUG_PREFIX_ERR = "[<color=magenta>VerifyUserTeleporter</color>]";

        private void Start()
        {
            if (!CheckNull())
            {
                enabled = false;
                return;
            }
        }

        private bool CheckNull()
        {
            if (loader == null)
            {
                Debug.LogError($"{DEBUG_PREFIX_ERR}{nameof(UserIDLoader)}が見つかりませんでした。{nameof(UserIDLoader)}を設定してください - {gameObject.name}");
                return false;
            }
            if (teleportPos == null)
            {
                Debug.LogError($"{DEBUG_PREFIX_ERR}テレポート先が指定されていません - {gameObject.name}");
                return false;
            }
            return true;
        }

        public void Teleport() => Networking.LocalPlayer.TeleportTo(teleportPos.position, Quaternion.identity);

        #region VRCMethod
        public override void Interact()
        {
            if (loader.CheckUserIDValid(isBlackList))
            {
                Teleport();
            }
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (isTeleportOnStart && player.isLocal && loader.CheckUserIDValid(isBlackList))
            {
                SendCustomEventDelayedFrames(nameof(Teleport), 5);
            }
        }
        #endregion
    }
}

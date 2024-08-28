using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace sh0uRoom.VRCIDGimmick
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VerifyUserTeleporter : UdonSharpBehaviour
    {
        [SerializeField] private UserIDLoader loader;
        [SerializeField] private bool isBlackList = false;
        [SerializeField] private Transform teleportPos;
        [SerializeField] private bool isTeleportOnStart = false;
        private const string DEBUG_PREFIX_ERR = "[<color=magenta>VerifyUserTeleporter</color>]";

        private void Start()
        {
            if (!CheckNull())
            {
                enabled = false;
                return;
            }

            if (isTeleportOnStart)
            {
                Teleport();
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
        #endregion
    }
}

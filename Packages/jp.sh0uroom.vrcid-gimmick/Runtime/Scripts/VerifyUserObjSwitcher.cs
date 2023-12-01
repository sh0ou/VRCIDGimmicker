
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace sh0uRoom.VRCIDGimmick
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class VerifyUserObjSwitcher : UdonSharpBehaviour
    {
        [SerializeField] private UserIDLoader loader;
        [SerializeField] private GameObject targetObj;
        [SerializeField] private bool isOnceOnly = false;
        [SerializeField] private bool isNetworking = false;

        [UdonSynced] private bool isActivated = false;
        [UdonSynced] private bool isActiveObj = false;

        void Start()
        {
            if (!CheckNull())
            {
                enabled = false;
                return;
            }

            if (Networking.IsInstanceOwner)
            {
                isActiveObj = targetObj.activeSelf;
            }

            RequestSerialization();

            if (isActiveObj)
            {
                targetObj.SetActive(true);
            }
            else
            {
                targetObj.SetActive(false);
            }
        }

        private bool CheckNull()
        {
            if (loader == null)
            {
                Debug.LogError($"[<color=magenta>{nameof(VerifyUserObjSwitcher)}</color>]{nameof(UserIDLoader)}が設定されていません - {gameObject.name}");
                return false;
            }
            if (targetObj == null)
            {
                Debug.LogError($"[<color=magenta>{nameof(VerifyUserObjSwitcher)}</color>]対象オブジェクトが設定されていません - {gameObject.name}");
                return false;
            }
            return true;
        }

        public override void Interact()
        {
            RequestSerialization();

            if (isOnceOnly && isActivated) return;

            if (loader.CheckUserIDValid())
            {
                if (isNetworking)
                {
                    Networking.SetOwner(Networking.LocalPlayer, targetObj);

                    RequestSerialization();
                    isActiveObj = !isActiveObj;
                    isActivated = true;
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Activate");
                }
                else
                {
                    isActiveObj = !isActiveObj;
                    Activate();
                }
            }
        }

        public void Activate()
        {
            if (isOnceOnly && isActivated) return;
            targetObj.SetActive(isActiveObj);
        }
    }
}

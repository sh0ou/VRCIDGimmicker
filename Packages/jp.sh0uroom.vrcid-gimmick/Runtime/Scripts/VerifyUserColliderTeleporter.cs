﻿using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDKBase.Editor.Attributes;

namespace sh0uRoom.VRCIDGimmick
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VerifyUserColliderTeleporter : UdonSharpBehaviour
    {
        [SerializeField] private Collider[] targetColliders;
        [SerializeField] private VRCPlayerApi.TrackingDataType targetBone;
        [SerializeField] private Transform teleportPos;
        [SerializeField] private float checkInterval = 0.1f;
        private float timer = 0f;

        [SerializeField] private UserIDLoader loader;
        [SerializeField] private bool isWhiteList = false;

        private VRCPlayerApi player;

        private void Start()
        {
            player = Networking.LocalPlayer;

            if (!CheckNull()) enabled = false;
        }

        private bool CheckNull()
        {
            if (targetColliders.Length == 0 || teleportPos == null)
            {
                Debug.LogError($"[<color=magenta>{nameof(VerifyUserColliderTeleporter)}</color>]対象のコライダー、テレポート先が設定されていません");
                return false;
            }
            if (loader == null)
            {
                Debug.LogWarning($"[<color=yellow>{nameof(VerifyUserColliderTeleporter)}</color>]{nameof(UserIDLoader)}が設定されてません。全てのプレイヤーが対象になります");
                return false;
            }
            foreach (var collider in targetColliders)
            {
                if (!collider.isTrigger)
                {
                    Debug.LogWarning($"[<color=yellow>{nameof(VerifyUserColliderTeleporter)}</color>]{collider.name}のisTriggerにチェックをつけてください");
                    return false;
                }
            }
            return true;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= checkInterval)
            {
                timer = 0f;
                CheckPlayerHit();
            }
        }

        /// <summary> プレイヤーがコライダーに入ったらテレポートする </summary>
        private void CheckPlayerHit()
        {
            if (loader != null)
            {
                if (loader.CheckUserIDValid(isWhiteList)) return;
            }

            // プレイヤーのBone位置を取得
            var player_bonePos = player.GetTrackingData(targetBone).position;

            foreach (var collider in targetColliders)
            {
                // プレイヤーのBone位置がコライダーの中に入ったら
                if (collider.bounds.Contains(player_bonePos))
                {
                    // プレイヤーをテレポートさせる
                    player.TeleportTo(teleportPos.position, Quaternion.identity);
                }
            }
        }
    }
}

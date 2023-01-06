using System;
using UnityEngine;
using Cinemachine;

using NijiDive.Entities.Mobs.Player;
using NijiDive.Managers.Levels;

namespace NijiDive.CinemachineAddons
{
    // Based on LockCameraZ class from https://forum.unity.com/threads/follow-only-along-a-certain-axis.544511/
    [ExecuteInEditMode]
    [SaveDuringPlay]
    public class VirtualCameraConstrainedAxis : CinemachineExtension
    {
        [SerializeField] private float xOffset;
        [Tooltip("Set to 0 for snap transition")]
        [SerializeField] [Min(0f)] private float transitionSpeed = Constants.CHUNK_SIZE;

        private Vector3 prevPos;

        protected override void Awake()
        {
            base.Awake();

            if (!Application.isPlaying) return;

            SetCamFollow();
            if (LevelManager.singleton) LevelManager.singleton.OnLoadUpgrading.AddListener(MoveCamFollowToMinusOffset2D);
            else Debug.LogWarning($"No {nameof(LevelManager)} found");
        }

        private void SetCamFollow()
        {
            var playerTransform = FindObjectOfType<PlayerController>().transform;
            GetComponent<CinemachineVirtualCamera>().Follow = playerTransform;
        }

        // Places camera at [0, 0]
        private void MoveCamFollowToMinusOffset2D()
        {
            var offset2D = (Vector2)GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
            GetComponent<CinemachineVirtualCamera>().Follow.position = -offset2D;
        }

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Body)
            {
                var targetPosition = state.RawPosition;
                var xShifts = (int)Math.Round((targetPosition.x - xOffset) / Constants.CHUNK_SIZE, MidpointRounding.AwayFromZero);
                targetPosition.x = xShifts * Constants.CHUNK_SIZE + xOffset;

                if (transitionSpeed == 0f || Time.timeSinceLevelLoad < Time.deltaTime)
                {
                    state.RawPosition = targetPosition;
                    prevPos = targetPosition;
                }
                else
                {
                    state.RawPosition = Vector3.MoveTowards(prevPos, targetPosition, transitionSpeed * Time.deltaTime);
                    prevPos = state.RawPosition;
                }
            }
        }
    }
}
using System;
using UnityEngine;
using Cinemachine;

using NijiDive.Managers.Persistence;
using NijiDive.Controls.Player;

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
            PersistenceManager.OnLoaded.AddListener(SetCamFollow);
        }

        private void SetCamFollow()
        {
            var playerTransform = PersistenceManager.FindPersistentObjectOfType<PlayerController>().transform;
            GetComponent<CinemachineVirtualCamera>().Follow = playerTransform;
        }

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Body)
            {
                var targetPosition = state.RawPosition;
                var xShifts = (int)Math.Round((targetPosition.x - xOffset) / Constants.CHUNK_SIZE, MidpointRounding.AwayFromZero);
                targetPosition.x = xShifts * Constants.CHUNK_SIZE + xOffset;

                if (transitionSpeed == 0f || Time.time < Time.deltaTime)
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
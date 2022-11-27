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

        protected override void Awake()
        {
            base.Awake();
            PersistenceManager.OnLoaded.AddListener(SetCamFollow);
        }

        private void SetCamFollow()
        {
            GetComponent<CinemachineVirtualCamera>().Follow = PersistenceManager.FindPersistentObjectOfType<PlayerController>().transform;
        }

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Body)
            {
                var mapWidth = Constants.CHUNK_SIZE;
                var pos = state.RawPosition;
                var xShifts = (int)Math.Round((pos.x - xOffset) / mapWidth, MidpointRounding.AwayFromZero);
                pos.x = xShifts * mapWidth + xOffset;

                state.RawPosition = pos;
            }
        }
    }
}
using System;
using UnityEngine;
using Cinemachine;

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
            GetComponent<CinemachineVirtualCamera>().Follow = GameObject.FindGameObjectWithTag("Player").transform;
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
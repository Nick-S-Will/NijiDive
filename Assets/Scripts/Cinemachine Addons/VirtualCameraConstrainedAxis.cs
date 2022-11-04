using System;
using UnityEngine;
using Cinemachine;

namespace NijiDive.CinemachineAddons
{
    // Based on LockCameraZ class from https://forum.unity.com/threads/follow-only-along-a-certain-axis.544511/
    [ExecuteInEditMode]
    [SaveDuringPlay]
    [AddComponentMenu("")] // Hide in menu
    public class VirtualCameraConstrainedAxis : CinemachineExtension
    {
        [SerializeField] private int mapWidth = 13;

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Body)
            {
                var pos = state.RawPosition;
                var xShifts = (int)Math.Round(pos.x / mapWidth, MidpointRounding.AwayFromZero);
                pos.x = xShifts * mapWidth;
                state.RawPosition = pos;
            }
        }
    }
}
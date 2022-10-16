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
        [SerializeField] private int mapExtent = 6;

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Body)
            {
                var pos = state.RawPosition;
                var xDir = Mathf.RoundToInt(pos.x / Mathf.Abs(pos.x));
                var xShifts = Mathf.Abs(pos.x) >= mapExtent ? ((int)pos.x + xDir * mapExtent) / (2 * mapExtent) : ((int)pos.x / mapExtent);
                pos.x = 2 * mapExtent * xShifts;
                state.RawPosition = pos;
            }
        }
    }
}
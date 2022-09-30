using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NijiDive.Managers;

namespace NijiDive.Controls
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public abstract class HumanoidControls : MonoBehaviour
    {
        [Header("Moving")]
        [SerializeField] private float moveForce = 5f;
        [Tooltip("The force multiplier added when starting to move the opposite way")]
        [SerializeField] [Min(1f)] private float reverseForceMultiplier = 1f;
        [SerializeField] [Range(0f, 1f)] private float extraDeceleration = 0f;

        [Header("Jumping")]
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] [Min(0f)] private float jumpBufferTime = 0.25f, maxGroundDistance = 0.1f, coyoteTime = 0.25f, minJumpInterval = 0.5f;

        [Header("Visualizers")]
        [SerializeField] private Color gizmoColor = Color.red;
        [SerializeField] private bool showGroundCheck;

        private MapManager map;
        private Rigidbody2D rb2d;
        private Collider2D hitbox;
        private Coroutine jumpTry;
        private Bounds groundCheckBounds;
        private float lastTimeGrounded;

        protected bool OnGround => Time.time - lastTimeGrounded <= coyoteTime;

        protected virtual void Awake()
        {
            map = FindObjectOfType<MapManager>();
            if (map == null)
            {
                Debug.LogError($"No {typeof(MapManager)} found in scene");
                enabled = false;
            }

            hitbox = GetComponent<Collider2D>();
            rb2d = GetComponent<Rigidbody2D>();
        }

        protected virtual void FixedUpdate()
        {
            if (GroundCheck()) lastTimeGrounded = Time.time;
        }

        protected void Move(float xInput)
        {
            if (xInput == 0f)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x / (1f + extraDeceleration), rb2d.velocity.y);
            }
            else
            {
                var moveForce = xInput * this.moveForce * Vector2.right;
                if (xInput * rb2d.velocity.x < 0f) moveForce *= reverseForceMultiplier;
                rb2d.AddForce(moveForce);
            }
        }

        protected void TryJump()
        {
            if (jumpTry == null) jumpTry = StartCoroutine(TryJumpRoutine());
        }
        private IEnumerator TryJumpRoutine()
        {
            var startTime = Time.time;

            while (Time.time - startTime <= jumpBufferTime && isActiveAndEnabled)
            {
                if (OnGround)
                {
                    Jump();
                    break;
                }

                yield return new WaitForFixedUpdate();
            }

            startTime = Time.time;
            yield return new WaitUntil(() => !OnGround || Time.time - startTime > minJumpInterval || !isActiveAndEnabled);
            jumpTry = null;
        }
        private void Jump()
        {
            rb2d.velocity += jumpForce * Vector2.up;
        }

        private bool GroundCheck()
        {
            var boxPos = transform.position + (maxGroundDistance / 2f * Vector3.down);
            var boxSize = new Vector2(0.9f * hitbox.bounds.size.x, maxGroundDistance);
            if (showGroundCheck) groundCheckBounds = new Bounds(boxPos, boxSize);

            var collision = Physics2D.OverlapBox(boxPos, boxSize, 0f, map.GroundMask);
            return collision != null;
        }

        private void OnDrawGizmos()
        {
            if (showGroundCheck && groundCheckBounds != null)
            {
                Gizmos.color = gizmoColor;
                Gizmos.DrawCube(groundCheckBounds.center, groundCheckBounds.size);
            }
        }
    }
}
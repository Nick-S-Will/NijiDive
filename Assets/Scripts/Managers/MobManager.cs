using System.Collections.Generic;
using UnityEngine;

using NijiDive.Map.Chunks;
using NijiDive.Controls.Enemies;

namespace NijiDive.Managers.Mobs
{
    public class MobManager : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float enableDistance = Chunk.SIZE;

        private List<Transform> enemyTransforms = new List<Transform>();

        private void Start()
        {
            if (target == null)
            {
                Debug.LogError($"No {nameof(target)} assigned");
                enabled = false;
            }

            foreach (Transform t in transform)
            {
                if (t.GetComponent<Enemy>() != null)
                {
                    enemyTransforms.Add(t);
                    t.gameObject.SetActive(false);
                }
            }
        }

        private void Update()
        {
            if (target == null) enabled = false;

            foreach (var enemy in enemyTransforms.ToArray())
            {
                if (Mathf.Abs(target.position.y - enemy.position.y) < enableDistance)
                {
                    enemy.gameObject.SetActive(true);
                    enemyTransforms.Remove(enemy);
                }
            }
        }

        private void OnValidate()
        {
            if (target) enabled = true;
        }
    }
}
using UnityEngine;

namespace NijiDive.Controls.Enemies
{
    public abstract class Enemy : Mob
    {
        protected static Transform target;

        protected override void Awake()
        {
            if (target == null) target = GameObject.FindGameObjectWithTag("Player").transform;

            base.Awake();
        }

        private void OnApplicationQuit()
        {
            target = null;
        }
    }
}
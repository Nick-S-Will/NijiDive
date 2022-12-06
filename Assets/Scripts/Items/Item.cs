using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NijiDive.Items
{
    public abstract class Item : ScriptableObject
    {
        [SerializeField] protected Sprite sprite, uiSprite;
        [SerializeField] private string description;

        public string Description => description;
    }
}
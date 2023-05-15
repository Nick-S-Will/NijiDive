using UnityEngine;

namespace NijiDive
{
    public static class Constants
    {
        public const string GAME_SCENE_NAME = "GameScene";
        public const string UPGRADE_SCENE_NAME = "UpgradeScene";

        public const string PLAYER_TAG = "Player";
        public const string COMBO_TEXT_SORTING_LAYER = "Combo Text";
        public const string TEXT_MESH_SORTING_LAYER = "Text Mesh";

        /// <summary>
        /// Width and height of chunks in NijiDive
        /// </summary>
        public const int CHUNK_SIZE = 13;

        public static readonly Color MAIN_COLOR = Color.white;
        public static readonly Color DANGER_COLOR = Color.red;
        public static readonly Color VULNERABLE_COLOR = Color.blue;
        public static readonly Color NEGATIVE_COLOR = Color.black;
    }
}
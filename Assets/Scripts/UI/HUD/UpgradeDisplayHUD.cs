using UnityEngine;
using UnityEngine.SceneManagement;

using NijiDive.MenuItems;
using NijiDive.UI.Menu;

namespace NijiDive.UI.HUD
{
    public class UpgradeDisplayHUD : UIElement
    {
        [SerializeField] private Material rendererMaterial;
        [SerializeField] private Sprite placeholderSprite;
        [SerializeField] private int sortingLayer = 11;
        [Space]
        [SerializeField] [Min(1)] private int spriteCount = 4;
        [SerializeField] [Min(0f)] private float spriteSpacing = 1f;
        [SerializeField] [Min(0f)] private float centerGap = Constants.CHUNK_SIZE;

        [HideInInspector] [SerializeField] private SpriteRenderer[] upgradeSpritesRenderers;
        private int currentSpriteIndex;

        public override bool IsVisible => throw new System.NotImplementedException();

        void Start()
        {
            SceneManager.sceneLoaded += AddDisplayUpgradeSpriteToPurchaseEvent;

            EmptySpriteRenderers();
        }

        public override void SetVisible(bool visible)
        {
            foreach (Transform child in transform) child.gameObject.SetActive(visible);

            base.SetVisible(visible);
        }

        private int IndexCenterDifference(int index, int size)
        {
            var center = size / 2;
            if (size % 2 == 0) return Mathf.Min(Mathf.Abs(center - 1 - index), Mathf.Abs(center - index));
            else return Mathf.Min(Mathf.Abs(center + 1 - index), Mathf.Abs(center - index));
        }

        public override void UpdateShape()
        {
            base.UpdateShape();

            if (upgradeSpritesRenderers == null) return;

            int spriteCount = upgradeSpritesRenderers.Length;
            for (int i = 0; i < spriteCount; i++)
            {
                float xPosition = (centerGap / 2f) + (IndexCenterDifference(i, spriteCount) * spriteSpacing);

                var xDirection = i < spriteCount / 2f ? -1f : 1f;
                upgradeSpritesRenderers[i].transform.localPosition = xDirection * xPosition * Vector3.right;
            }
        }

        private SpriteRenderer CreateSpriteRenderer()
        {
            var name = "Upgrade Sprite";
            var gameObject = new GameObject(name, new System.Type[] { typeof(SpriteRenderer) });

            gameObject.transform.parent = transform;
            gameObject.transform.localPosition = Vector3.zero;

            var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.material = rendererMaterial;
            spriteRenderer.sprite = placeholderSprite;
            spriteRenderer.sortingOrder = sortingLayer;

            return spriteRenderer;
        }

        [ContextMenu("Create Displays")]
        private void CreateSpriteObjects()
        {
            if (Application.isPlaying)
            {
                Debug.LogError("Sprite renderer objects must be created outside of play mode", this);
                return;
            }

            if (upgradeSpritesRenderers != null)
            {
                foreach (var spriteRenderer in upgradeSpritesRenderers)
                {
                    if (spriteRenderer == null) continue;

                    DestroyImmediate(spriteRenderer.gameObject);
                }
            }
            
            upgradeSpritesRenderers = new SpriteRenderer[spriteCount];
            for (int i = 0; i < upgradeSpritesRenderers.Length; i++) upgradeSpritesRenderers[i] = CreateSpriteRenderer();
            
            UpdateShape();
        }

        private void EmptySpriteRenderers()
        {
            if (upgradeSpritesRenderers == null)
            {
                Debug.LogError($"{nameof(UpgradeDisplayHUD)}'s sprites haven't been created");
                return;
            }

            foreach (var spriteRenderer in upgradeSpritesRenderers) spriteRenderer.sprite = null;
        }

        private void DisplayUpgradeSprite(MenuItem menuItem)
        {
            if (menuItem is Upgrade upgrade)
            {
                upgradeSpritesRenderers[currentSpriteIndex].sprite = upgrade.MenuUISprite;
                currentSpriteIndex++;
            }
        }

        private void AddDisplayUpgradeSpriteToPurchaseEvent(Scene loadedScene, LoadSceneMode sceneLoadMode)
        {
            if (sceneLoadMode == LoadSceneMode.Additive) return;

            var upgradeMenu = FindObjectOfType<UpgradeMenuUI>();
            if (upgradeMenu) upgradeMenu.OnPurchase.AddListener(DisplayUpgradeSprite);
        }

        private void OnValidate()
        {
            UpdateShape();
        }
    }
}
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.Pausing;
using NijiDive.Entities.Contact;
using NijiDive.Entities.Mobs;

namespace NijiDive.Managers.Coins
{
    public class CoinManager : Manager
    {
        public UnityEvent OnCoinChange;
        [Space]
        [SerializeField] private Coin[] coinSizes;
        [SerializeField] [Min(0f)] private float coinSpawnSpeedMin = 1f, coinSpawnSpeedMax = 2f, coinEnableDelay = 0.5f;
        [Tooltip("Set to 0 for indefinite life time")]
        [SerializeField] [Min(0f)] private float coinLifeTime = 5f;

        public static CoinManager singleton;
        private static int coinCount, totalCoinCount;

        public static int CoinCount => coinCount;
        public static int TotalCoinCount => totalCoinCount;

        private void Awake()
        {
            if (singleton == null) singleton = this;
            else
            {
                Debug.LogError($"Multiple {nameof(CoinManager)}s found", this);
                gameObject.SetActive(false);
                return;
            }
        }

        private void Start()
        {
            if (coinSizes.Length != Enum.GetValues(typeof(CoinValue)).Length)
            {
                Debug.LogError($"There must be a coin prefab for each value of {nameof(CoinValue)}", this);
                return;
            }

            Mob.OnMobDeath.AddListener(SpawnCoins);
        }

        public override void Retry() 
        {
            coinCount = 0;
            totalCoinCount = 0;
        }

        private IEnumerator DelayCoinEnable(Coin coin)
        {
            coin.enabled = false;

            yield return new PauseManager.WaitWhilePausedAndForSeconds(coinEnableDelay, coin);

            coin.enabled = true;

            if (coinLifeTime > 0f)
            {
                yield return coin.FadeOut(coinLifeTime);
                Destroy(coin.gameObject);
            }
        }

        #region Coin Spawning
        private void SpawnCoinsOfSize(Coin coinSizePrefab, Vector3 spawnPoint, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var coin = Instantiate(coinSizePrefab, spawnPoint, Quaternion.identity, transform);

                var body = coin.GetComponent<Rigidbody2D>();
                body.velocity = UnityEngine.Random.Range(coinSpawnSpeedMin, coinSpawnSpeedMax) * UnityEngine.Random.insideUnitCircle;
                _ = StartCoroutine(DelayCoinEnable(coin));
            }
        }

        public void ParseAndSpawnCoinSizes(Vector3 spawnPoint, int coins)
        {
            var coinValues = Enum.GetValues(typeof(CoinValue));
            var sizeCount = new int[coinValues.Length];
            for (int i = coinValues.Length - 1; i >= 0; i--)
            {
                var value = (int)coinValues.GetValue(i);
                sizeCount[i] = coins / value;
                coins -= sizeCount[i] * value;
            }

            for (int i = 0; i < sizeCount.Length; i++) SpawnCoinsOfSize(coinSizes[i], spawnPoint, sizeCount[i]);
        }

        /// Has this signature to fit <see cref="Mob.OnDeath"/>
        private void SpawnCoins(MonoBehaviour killedMob, MonoBehaviour mobKiller, DamageType damageType)
        {
            var coinDropper = killedMob.GetComponent<ICoinDropping>();
            if (coinDropper != null) ParseAndSpawnCoinSizes(killedMob.transform.position, coinDropper.CoinCount);
        }
        #endregion

        public void CollectCoin(CoinValue value)
        {
            var coins = (int)value;
            coinCount += coins;
            totalCoinCount += coins;
            OnCoinChange?.Invoke();
        }
        public void UseCoins(int amount)
        {
            coinCount -= amount;
            OnCoinChange?.Invoke();

            if (coinCount < 0) Debug.LogError("Used more coins than available", this);
        }

        private void OnValidate()
        {
            if (coinSpawnSpeedMax < coinSpawnSpeedMin)
            {
                Debug.LogWarning($"{nameof(coinSpawnSpeedMax)} must be superior or equal to {nameof(coinSpawnSpeedMin)}");
                coinSpawnSpeedMax = coinSpawnSpeedMin;
            }
        }

        private void OnDestroy()
        {
            if (singleton == this) singleton = null;
        }
    }
}
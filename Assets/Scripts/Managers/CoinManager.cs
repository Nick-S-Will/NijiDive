using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.Mobs;
using NijiDive.Entities;

namespace NijiDive.Managers.Coins
{
    public class CoinManager : MonoBehaviour
    {
        public UnityEvent OnCoinChange;
        [Space]
        [SerializeField] private Coin[] coinSizes;
        [SerializeField] [Min(0f)] private float coinSpawnSpeedMin = 1f, coinSpawnSpeedMax = 2f, coinEnableDelay = 0.5f, coinLifeTime = 5f;

        private int coinCount;

        public int CoinCount => coinCount;

        public static CoinManager singleton;

        private void Awake()
        {
            if (singleton == null) singleton = this;
            else
            {
                Debug.LogError($"Multiple {nameof(CoinManager)}s found in scene", this);
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

            foreach (var m in MobManager.singleton.Mobs)
            {
                if (m is ICoinDropping) m.OnDeath.AddListener(SpawnCoins);
            }
        }

        private IEnumerator DelayCoinEnable(Coin coin)
        {
            coin.enabled = false;

            yield return new WaitForSeconds(coinEnableDelay);

            coin.enabled = true;
        }

        #region Coin Spawning
        private void SpawnCoinsOfSize(Coin coinSizePrefab, Vector3 spawnPoint, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var coin = Instantiate(coinSizePrefab, spawnPoint, Quaternion.identity, transform);
                if (coinLifeTime > 0f) Destroy(coin.gameObject, coinLifeTime);

                var body = coin.GetComponent<Rigidbody2D>();
                body.velocity = UnityEngine.Random.Range(coinSpawnSpeedMin, coinSpawnSpeedMax) * UnityEngine.Random.insideUnitCircle;
                if (coinEnableDelay > 0f) _ = StartCoroutine(DelayCoinEnable(coin));
            }
        }

        private void ParseAndSpawnCoinSizes(Vector3 spawnPoint, int coins)
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
        private void SpawnCoins(MonoBehaviour killedMob, GameObject mobKiller, DamageType damageType)
        {
            var coinDropper = killedMob.GetComponent<ICoinDropping>();
            if (coinDropper != null) ParseAndSpawnCoinSizes(killedMob.transform.position, coinDropper.CoinCount);
        }
        #endregion

        public void CollectCoin(CoinValue value)
        {
            coinCount += (int)value;
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
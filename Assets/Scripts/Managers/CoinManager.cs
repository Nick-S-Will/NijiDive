using System;
using UnityEngine;

using NijiDive.Managers.Mobs;
using NijiDive.Entities;

namespace NijiDive.Managers.Coins
{
    public class CoinManager : MonoBehaviour
    {
        [SerializeField] private Coin[] coinSizes;
        [SerializeField] [Min(0f)] private float coinSpawnSpeedMin = 1f, coinSpawnSpeedMax = 2f;

        private int coinCount;

        public int CoinCount => coinCount;

        public static CoinManager singleton;

        private void Awake()
        {
            if (singleton == null) singleton = this;
            else
            {
                Debug.LogError($"Multiple {typeof(CoinManager)}s exist", this);
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

            foreach (var m in MobManager.singleton.DisabledMobs)
            {
                if (m is ICoinDropping) m.OnDeath.AddListener(SpawnCoins);
            }
        }

        private void SpawnCoinsOfSize(Coin coinSizePrefab, Vector3 spawnPoint, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var coin = Instantiate(coinSizePrefab, spawnPoint, Quaternion.identity, transform);
                var body = coin.GetComponent<Rigidbody2D>();
                body.velocity = UnityEngine.Random.Range(coinSpawnSpeedMin, coinSpawnSpeedMax) * UnityEngine.Random.insideUnitCircle;
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

        public void CollectCoin(CoinValue value)
        {
            coinCount += (int)value;
            print(coinCount);
        }

        private void OnValidate()
        {
            if (coinSpawnSpeedMax < coinSpawnSpeedMin)
            {
                Debug.LogWarning($"{nameof(coinSpawnSpeedMax)} must be superior or equal to {nameof(coinSpawnSpeedMin)}");
                coinSpawnSpeedMax = coinSpawnSpeedMin;
            }
        }
    }
}
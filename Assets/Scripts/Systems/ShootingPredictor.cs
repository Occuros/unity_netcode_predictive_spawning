using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace Systems
{
    public struct PredictedShot
    {
        public float timePassed;
    }

    public readonly struct ShootingPredictor
    {
        private readonly int _totalToFire;
        private readonly float _fraction;
        private readonly float _timeBetweenShots;

        public ShootingPredictor(NetworkTick startTick, NetworkTick currentTick, int fireRate, float timeDelta)
        {
            _timeBetweenShots = 60f / fireRate;

            var ticksSince = currentTick.TicksSince(startTick);

            var elapsedTime = timeDelta * ticksSince;


            var totalShotsFired = 1f + elapsedTime / _timeBetweenShots;

            var shotFraction = math.frac(totalShotsFired);

            _fraction = shotFraction * _timeBetweenShots;

            var timeLeftForShots = timeDelta - _fraction;

            var shotThisFrame = timeLeftForShots >= 0;
            var additionalShot = shotThisFrame ? 1 : 0;

            var totalToFireValid = (int)(timeLeftForShots / _timeBetweenShots) + additionalShot;
            var isValid = timeDelta - _fraction > 0f;
            _totalToFire = isValid ? totalToFireValid : 0;
        }

        public struct Enumerator
        {
            public int totalToFire;

            public int leftToFire;

            public float timeBetweenShots;
            public float fraction;
            public PredictedShot Current { get; private set; }

            public bool MoveNext()
            {
                if (leftToFire <= 0f)
                {
                    return false;
                }

                leftToFire--;

                var delta = timeBetweenShots * (totalToFire - (leftToFire + 1));
                Current = new() { timePassed = fraction + delta };

                return true;
            }
        }

        public Enumerator GetEnumerator()
        {
            return new()
            {
                totalToFire = _totalToFire,
                leftToFire = _totalToFire,
                timeBetweenShots = _timeBetweenShots,
                fraction = _fraction
            };
        }
    }
}
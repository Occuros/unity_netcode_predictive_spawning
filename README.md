# Predictive Spawning in Netcode for Unity

 This repository is a minimal example of the current issue with predictive spawning.

If the delay and jitter are set, once the bullets are spawned predictively on the client, not just one is spawned, but a multitude of bullets on the client.

As they are physics-based, they even collide with each other, resulting in a wrong prediction.

Even though the Gun system verifies execution with `IsFirstTimeFullyPredictingTick`, it seems to not be enough to prevent the gun to spawn too many bullets.

Something is missing, and this repo should help investigate this.

## Controls

Move with `A` `S` `W` `D`, shoot with `left mouse button`.


Associated forum thread:

https://forum.unity.com/threads/v1-0-12-how-to-properly-predictive-spawn-bullet-for-a-machine-gun.1459483/

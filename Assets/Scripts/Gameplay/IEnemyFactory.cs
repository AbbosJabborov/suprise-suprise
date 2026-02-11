using UnityEngine;

namespace Gameplay
{
    public interface IEnemyFactory
    {
        GameObject CreateEnemy(EnemyType type, Vector3 position);
    }
}


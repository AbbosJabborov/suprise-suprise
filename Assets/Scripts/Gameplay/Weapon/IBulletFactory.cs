using UnityEngine;

namespace Gameplay.Weapon
{
    public interface IBulletFactory
    {
        void CreateBullet(Vector3 position, Vector2 direction, WeaponData weapon, bool isPlayerBullet);
    }
}
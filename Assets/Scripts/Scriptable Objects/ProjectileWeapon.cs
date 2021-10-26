using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileWeapon", menuName = "Weapon/Projectile", order = 1)]
public class ProjectileWeapon : Weapon
{
    public GameObject projectilePrefab;
    public bool rapidFire = false;
    [Range(0, 1)]
    public float shotDelayTime = .25f;
}


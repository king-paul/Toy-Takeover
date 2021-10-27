using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LaserWeapon", menuName = "Weapon/Laser", order = 2)]
public class LaserWeapon : Weapon
{
    public Material laserMaterial;
    public float laserRange = 50;

    [Range(0, 1)]
    public float shotDelay = 0.2f;
}
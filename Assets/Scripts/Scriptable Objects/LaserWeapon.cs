using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LaserWeapon", menuName = "Weapon/Laser", order = 2)]
public class LaserWeapon : Weapon
{
    public Material laserMaterial;
    public float laserRange = 50;
    public float damageRate = 1;
}
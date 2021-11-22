using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    MeleeEnemyAI meleeEnemy;

    // Start is called before the first frame update
    void Start()
    {
        meleeEnemy = transform.parent.GetComponent<MeleeEnemyAI>();
    }

    public void MeleeAttack()
    {

    }
}

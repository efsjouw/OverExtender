using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDespawner : MonoBehaviour {

    void OnTriggerExit(Collider other)
    {
        Destroy(this);
        Game.instance.enemyDestroyed();
    }
}

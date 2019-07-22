using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour {

    protected Vector3 direction;
    protected float speed;

    protected Manager manager;


    protected bool updateLife()
    {
        Bounds ob = manager.getOuterBounds();
        if (ob.Contains(transform.position)) return true;
        DestroyImmediate(gameObject);
        return false;
    }

}

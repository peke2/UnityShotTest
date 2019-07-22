using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGenerator : MonoBehaviour
{
	public Vector3 m_direction = new Vector3(1, 1, 0);
	public float m_speed = 0.5f;
	public float m_length = 10.0f;

    // Start is called before the first frame update
    void Start()
    {   
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetButtonDown("Fire1") )
		{
			//var laser = GetComponent<Laser>();
			var gobj = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/LaserObject"));
			var laser = gobj.GetComponent<Laser>();
			laser.shoot(new Vector3(0, 0, 0), m_direction, m_speed, m_length);
		}
	}
}

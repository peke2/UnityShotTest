using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : EnemyBase {


    public static Shot Spawn(Manager manager, Vector3 pos)
    {	
        GameObject shotBase = Instantiate(Resources.Load<GameObject>("Prefabs/Shot"));

        Renderer rd = shotBase.GetComponent<Renderer>();
        MaterialPropertyBlock pb = new MaterialPropertyBlock();
        pb.SetColor("_Color", new Color(0.6f, 1.0f, 0.8f, 1.0f));
        rd.SetPropertyBlock(pb);

        Shot shot = shotBase.AddComponent<Shot>();
        shot.init(manager, pos);

        return shot;
    }

	Tracer tracer;


    // Use this for initialization
    void Start()
	{
		
	}
	
	// Update is called once per frame
	void Update()
	{
		updateDirection();

		transform.position += direction * speed;
		tracer.addState(transform.position, direction);

		if(updateLife() == false)
		{
			GameObject.DestroyImmediate(tracer.gameObject);
		}
	}

    void init(Manager manager, Vector3 pos)
    {
        this.manager = manager;

        transform.position = pos;

        Vector3 targetPos = manager.getPlayer().transform.position;
        direction = (targetPos - pos).normalized;
		speed = Random.RandomRange(0.3f, 0.6f);

		GameObject gobj = new GameObject("Tracer");
		tracer = gobj.AddComponent<Tracer>();
	}

	void updateDirection()
	{
		Player player = manager.getPlayer();
		Vector3 pdir = (player.transform.position - transform.position).normalized;
		direction = Vector3.RotateTowards(direction, pdir, Mathf.Deg2Rad * 3, 0);
	}
}

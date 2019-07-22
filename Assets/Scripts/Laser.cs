using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class Laser : MonoBehaviour
{
	class Pos
	{
		public Pos(Vector3 pos, Vector3 dir)
		{
			start		= pos;
			end			= pos;
			direction = dir.normalized;
			//direction = dir;
			isCollided = false;
		}

		public Vector3 start		{get; set;}
		public Vector3 end			{get; set;}
		public Vector3 direction	{get; set;}
		public bool	isCollided		{get; set;}

		public int serial { get; set; }
	}

	List<Pos> m_posList;

	float m_speed = 0.1f;

	float m_laserLength = 8.0f;

	LineRenderer m_lineRenderer;

	Vector3 m_topRight = new Vector3(8, 6, 0);
	Vector3 m_bottomLeft = new Vector3(-8, -6, 0);

	AudioSource m_reflectionSe;

	private void Awake()
	{
		m_posList = new List<Pos>();
	}

	private void OnEnable()
	{
	}

	// Start is called before the first frame update
	void Start()
    {
		var se = Resources.Load<AudioClip>("Sounds/decision13");

		m_reflectionSe = GetComponent<AudioSource>();
		m_reflectionSe.clip = se;
		m_reflectionSe.volume = 0.3f;

		m_lineRenderer = gameObject.AddComponent<LineRenderer>();
		m_lineRenderer.startWidth = 0.1f;
		m_lineRenderer.endWidth = 0.1f;
		//m_lineRenderer.startColor = new Color(0.8f, 0.8f, 1.0f, 1.0f);
		//m_lineRenderer.endColor = new Color(0.8f, 0.8f, 1.0f, 0.0f);

		var color = Resources.Load<Material>("Materials/LaserColor");
		m_lineRenderer.material = color;
	}

	static int cnt = 0;

	// Update is called once per frame
	void Update()
    {
		Pos genPos = null;

		for(var i=0; i<m_posList.Count; i++)
		{
			var pos = m_posList[i];

			var mv = pos.direction * m_speed;
			if (pos.isCollided == false)
			{
				//Debug.Log("Move["+pos.serial.ToString()+"]");
				pos.start += mv;

				var v = pos.start - pos.end;
				if (v.magnitude > m_laserLength)
				{
					pos.end = pos.start - v.normalized * m_laserLength;
				}

				RaycastHit hit;
				if (Physics.Raycast(pos.end, pos.direction, out hit, v.magnitude, LayerMask.GetMask("Collider")))
				{
					//Debug.Log("Collide[" + pos.serial.ToString() + "]");

					//	当たった箇所で止める
					pos.start = hit.point;

					pos.isCollided = true;

					//	先頭だったら反射
					if (i == 0)
					{
						var dir = Vector3.Reflect(pos.direction, hit.normal);
						genPos = new Pos(pos.start, dir);
						genPos.serial = cnt++;
						//genPos.start = pos.start + dir * m_speed;
						//Debug.Log("Generated["+ genPos.serial.ToString() +"]");

						m_reflectionSe.Play();
					}
					else
					{
						//(2019.07.16)[todo]	先頭じゃなければ、衝突よりも前の要素を削除　→　他のオブジェクトなどに遮られた場合
					}
				}
			}
			else
			{
				if( i == m_posList.Count-1 )
				{
					//	末尾だけ移動
					pos.end += mv;

					//Debug.Log("Tail Move["+pos.serial+"]");
				}
			}
		}

		//	指定よりも短くなったら要素から削除
		var removed = new List<Pos>();
		foreach(var pos in m_posList)
		{
			//	一回の移動可能な距離未満になったら削除対象(突き抜け防止)
			var length = (pos.start - pos.end).magnitude;
			if ( m_speed - length > 0.05f )
			{
				//	remove_if() みたいなの無さそうなので削除リストに一旦保持
				removed.Add(pos);

				//Debug.Log("Removed["+pos.serial.ToString()+":"+ length.ToString() + ":" + (m_speed - length).ToString() + "]");
			}
		}
		//	実際の削除
		foreach(var rm in removed)
		{
			m_posList.Remove(rm);
		}

		//	先頭に挿入
		if (genPos != null)
		{
			m_posList.Insert(0, genPos);
		}

		//	描画への反映
		var lines = new List<Vector3>();
		if (m_posList.Count > 0)
		{
			lines.Add(m_posList[0].start);

			foreach (var pos in m_posList)
			{
				lines.Add(pos.end);
			}
			m_lineRenderer.positionCount = lines.Count;
			m_lineRenderer.SetPositions(lines.ToArray());

			//確認のための固定描画
			//var poss = new Vector3[]{
			//	new Vector3(0,0,0),
			//	new Vector3(1,0,0),
			//	new Vector3(1,1,0),
			//};
			//m_lineRenderer.positionCount = poss.Length;
			//m_lineRenderer.SetPositions(poss);
		}
		else
		{
		}

		//if( m_posList.Count > 0 )
		//{
		//	m_lineRenderer.enabled = true;
		//}
		//else
		//{
		//	m_lineRenderer.enabled = false;
		//}
	}

	public void shoot(Vector3 pos, Vector3 dir, float speed, float length)
	{
		m_posList.Clear();

		m_speed = speed;
		m_laserLength = length;
		m_posList.Add(new Pos(pos, dir));
	}

}

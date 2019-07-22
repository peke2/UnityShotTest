using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]

public class Tracer : MonoBehaviour
{
	const int MAX_FRAME_COUNT = 32;
	const int MAX_BUFF_COUNT = MAX_FRAME_COUNT;

	TraceBuffer traceBuffer;

	static Material material;
	static Texture2D texture;

	private void Awake()
	{
		if(material == null)
		{
			material = Instantiate(Resources.Load<Material>("Materials/ColorMaterial"));
		}

		if(texture == null)
		{
			texture = Resources.Load<Texture2D>("Textures/laser");
		}
	}


	// Use this for initialization
	void Start()
    {
		traceBuffer = new TraceBuffer(MAX_BUFF_COUNT, MAX_FRAME_COUNT);
	}

	// Update is called once per frame
	void Update()
    {
		updateState();
        updateMesh();
		drawDebug();

	}

	void drawDebug()
	{
		int buffLen = traceBuffer.BufferLen;

		for(int i=0; i<buffLen; i++)
		{
			TraceBuffer.State state = traceBuffer.getState(i);

			//	最新のものが一番明るい
			float rate = (float)(buffLen - i) / MAX_BUFF_COUNT;
			Color col = new Color(rate, rate, rate, 1);

			Vector3 st, ed;
			st = state.pos - state.rt * 0.1f;
			ed = state.pos + state.rt * 0.1f; 
			Debug.DrawLine(st, ed, Color.red * col);
			st = state.pos - state.fr * 0.1f;
			ed = state.pos + state.fr * 0.1f;
			Debug.DrawLine(st, ed, Color.blue * col);
		}
	}


	void updateMesh()
    {
		int buffLen = traceBuffer.BufferLen;

		//	ポリゴン成立には最低でも2つの情報が必要
		if(buffLen <= 1) return;

        Mesh mesh = new Mesh();

        int numVertices = buffLen * 2;

		Vector3[] vertices = new Vector3[numVertices];
		Vector2[] uvs = new Vector2[numVertices];
		Color[]	colors     = new Color[numVertices];
		int[] triangles    = new int[(buffLen - 1) * 6];

        float halfW = 0.2f;

        for(int i=0; i<buffLen; i++)
        {
			TraceBuffer.State state = traceBuffer.getState(i);

			int index0, index1;
			index0 = i * 2 + 0;
			index1 = index0 + 1;

			vertices[index0] = state.pos - state.rt*halfW;
            vertices[index1] = state.pos + state.rt * halfW;
            //colors[index0] = new Color(1.0f, 0.3f, 0.3f, 1.0f/MAX_FRAME_COUNT*state.frameCount);
            //colors[index1] = new Color(1.0f, 0.3f, 0.3f, 1.0f/MAX_FRAME_COUNT*state.frameCount);
			colors[index0] = Color.white;
			colors[index1] = Color.white;
			if(i == 0)
			{
				uvs[index0] = new Vector2(0, 0);
				uvs[index1] = new Vector2(1, 0);
			}
			else if(i == buffLen - 2)
			{
				uvs[index0] = new Vector2(0, 0.75f);
				uvs[index1] = new Vector2(1, 0.75f);
			}
			else if(i==buffLen-1)
			{
				uvs[index0] = new Vector2(0, 1);
				uvs[index1] = new Vector2(1, 1);
			}
			else
			{
				uvs[index0] = new Vector2(0, 0.25f);
				uvs[index1] = new Vector2(1, 0.25f);
			}
		}

		int offset = 0;
        for(int i=0; i<buffLen-1; i++)
        {
            triangles[i * 6 + 0] = offset + 0;
            triangles[i * 6 + 1] = offset + 1;
            triangles[i * 6 + 2] = offset + 2;
            triangles[i * 6 + 3] = offset + 1;
            triangles[i * 6 + 4] = offset + 3;
            triangles[i * 6 + 5] = offset + 2;
			offset += 2;
        }

		mesh.vertices  = vertices;
		mesh.uv        = uvs;
		mesh.colors    = colors;
		mesh.triangles = triangles;

		MaterialPropertyBlock mpb = new MaterialPropertyBlock();
		mpb.SetTexture("_MainTex", texture);
		mpb.SetColor("_Color", Color.white);

		GetComponent<MeshFilter>().mesh = mesh;
		MeshRenderer mr = GetComponent<MeshRenderer>();
		mr.material = material;
		mr.SetPropertyBlock(mpb);
    }


	public void addState(Vector3 pos, Vector3 front)
    {
		traceBuffer.addState(pos, front);
    }

    public void updateState()
    {
		traceBuffer.updateState();
    }
}
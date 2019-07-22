using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]

public class TraceBuffer
{
    public class State
    {
        public Vector3 pos;
        public Vector3 fr;
        public Vector3 rt;
        public int frameCount;
        int maxFrameCount;

        static Vector3 zdir = new Vector3(0, 0, -1);

        public State(int count)
        {
            this.maxFrameCount = count;
        }

        public void setState(Vector3 pos, Vector3 fr)
        {
            this.pos = pos;
            this.fr = fr.normalized;
            this.rt = Vector3.Cross(State.zdir, this.fr);
            this.frameCount = maxFrameCount;
        }
    }


    State[] stateBuffer;

	int maxBufferCount;
	int maxFrameCount;

	int bufferTopIndex;
    int bufferLen;

	public int BufferTopIndex
	{
		get
		{
			return bufferTopIndex;
		}
	}

	public int BufferLen
	{
		get
		{
			return bufferLen;
		}
	}

	public TraceBuffer(int maxBuffCount, int maxFrameCount)
    {
		this.maxBufferCount = maxBuffCount;
		this.maxFrameCount = maxFrameCount;

        stateBuffer = new State[this.maxBufferCount];
        for(int i = 0; i<this.maxBufferCount; i++)
        {
            stateBuffer[i] = new State(this.maxFrameCount);
        }

        bufferTopIndex = 0;
        bufferLen = 0;
    }

	/*
    public void update()
    {
		updateState();
	}*/

	private int calcIndex(int i)
	{
		int index = bufferTopIndex -1 - i;
		//	リング
		if(0 > index)
		{
			index += maxBufferCount;
		}
		return index;
	}

    public void addState(Vector3 pos, Vector3 front)
    {
        State state = stateBuffer[bufferTopIndex];
        state.setState(pos, front);

        if(bufferLen < maxBufferCount)
        {
            bufferLen += 1;
        }

        //  リング
        bufferTopIndex = (bufferTopIndex + 1) % maxBufferCount;
    }

    public void updateState()
    {
        int delCount = 0;
        for(int i=0; i<bufferLen; i++)
        {
			State state = getState(i);

			state.frameCount--;
            if( state.frameCount == 0 )
            {
                delCount++;
            }
        }

        //  順番に追加されるのでカウントは必ずtop>=lastの順番になる
        //  対象のカウント分だけバッファの長さを減らせばよい
        bufferLen -= delCount;
    }

	public State getState(int index)
	{
		//	登録が無い場合も対象外
		if((index < 0) || (index >= bufferLen))  return null;

		int i = calcIndex(index);
		
		return stateBuffer[i];
	}
}
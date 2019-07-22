using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;

using NUnit.Framework;

public class TestTraceBuffer
{
	[Test]
	public void testCreate()
	{
		TraceBuffer buff = new TraceBuffer(4, 3);
		Assert.AreEqual(0, buff.BufferTopIndex);
	}

	[Test]
	public void testAdd()
	{
		TraceBuffer buff = new TraceBuffer(4, 3);
		buff.addState(new Vector3(1, 0, 0), Vector3.forward);
		Assert.AreEqual(1, buff.BufferTopIndex);
		Assert.AreEqual(1, buff.BufferLen);

		buff.addState(new Vector3(2, 0, 0), Vector3.forward);
		buff.addState(new Vector3(3, 0, 0), Vector3.forward);
		Assert.AreEqual(3, buff.BufferTopIndex);
		Assert.AreEqual(3, buff.BufferLen);

		//	リングの境界
		buff.addState(new Vector3(4, 0, 0), Vector3.forward);
		Assert.AreEqual(0, buff.BufferTopIndex);
		Assert.AreEqual(4, buff.BufferLen);

		buff.addState(new Vector3(5, 0, 0), Vector3.forward);
		Assert.AreEqual(1, buff.BufferTopIndex);
		Assert.AreEqual(4, buff.BufferLen);
	}

	[Test]
	public void testUpdate()
	{
		TraceBuffer buff = new TraceBuffer(4, 3);
		buff.addState(new Vector3(1, 0, 0), Vector3.forward);
		buff.addState(new Vector3(2, 0, 0), Vector3.forward);
		buff.addState(new Vector3(3, 0, 0), Vector3.forward);

		TraceBuffer.State state;
				
		//	状態を更新
		buff.updateState();

		state = buff.getState(-1);	//	登録範囲外
		Assert.IsNull(state);

		state = buff.getState(0);	//	登録範囲内(境界)
		Assert.IsNotNull(state);

		state = buff.getState(2);   //	登録範囲内(境界)
		Assert.IsNotNull(state);

		state = buff.getState(3);	//	登録範囲外
		Assert.IsNull(state);
	}

	[Test]
	public void testUpdateFrameCount()
	{
		TraceBuffer buff = new TraceBuffer(4, 3);
		buff.addState(new Vector3(1, 0, 0), Vector3.forward);
		buff.addState(new Vector3(2, 0, 0), Vector3.forward);
		buff.addState(new Vector3(3, 0, 0), Vector3.forward);

		TraceBuffer.State state;

		state = buff.getState(0);
		Assert.AreEqual(3, state.frameCount);

		state = buff.getState(2);
		Assert.AreEqual(3, state.frameCount);

		//	状態を更新
		buff.updateState();

		state = buff.getState(1);
		Assert.AreEqual(2, state.frameCount);

		state = buff.getState(2);
		Assert.AreEqual(2, state.frameCount);

		buff.addState(new Vector3(4, 0, 0), Vector3.forward);

		//	状態を更新
		buff.updateState();

		state = buff.getState(3);
		Assert.AreEqual(1, state.frameCount);
		Assert.AreEqual(1, state.pos.x);

		state = buff.getState(1);
		Assert.AreEqual(1, state.frameCount);
		Assert.AreEqual(3, state.pos.x);

		state = buff.getState(0);
		Assert.AreEqual(2, state.frameCount);
		Assert.AreEqual(4, state.pos.x);

		buff.addState(new Vector3(5, 0, 0), Vector3.forward);

		state = buff.getState(0);
		Assert.AreEqual(3, state.frameCount);
		Assert.AreEqual(5, state.pos.x);

		state = buff.getState(3);
		Assert.AreEqual(1, state.frameCount);
		Assert.AreEqual(2, state.pos.x);

		state = buff.getState(2);
		Assert.AreEqual(1, state.frameCount);
		Assert.AreEqual(3, state.pos.x);

	}


	[Test]
	public void testUpdateRemove()
	{
		TraceBuffer buff = new TraceBuffer(4, 3);
		buff.addState(new Vector3(1, 0, 0), Vector3.forward);
		buff.addState(new Vector3(2, 0, 0), Vector3.forward);

		buff.updateState();

		buff.addState(new Vector3(3, 0, 0), Vector3.forward);

		Assert.AreEqual(3, buff.BufferLen);

		buff.updateState();
		buff.updateState();

		Assert.AreEqual(1, buff.BufferLen);

		TraceBuffer.State state;
		state = buff.getState(0);
		Assert.AreEqual(3, state.pos.x);

		buff.updateState();
		Assert.AreEqual(0, buff.BufferLen);

		buff.addState(new Vector3(4, 0, 0), Vector3.forward);
		Assert.AreEqual(1, buff.BufferLen);

		state = buff.getState(0);
		Assert.AreEqual(4, state.pos.x);

	}
}

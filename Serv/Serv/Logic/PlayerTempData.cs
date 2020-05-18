using System;
using System.Collections.Generic;

public class PlayerTempData
{
	public PlayerTempData()
	{
		status = Status.None;
	}
	//状态
	public enum Status
	{
		None,
		Room,
		Fight,
	}
	public Status status;
	//room状态
	public Room room;
	public int team = 1;
    public int boatModel = 0;
    public bool isOwner = false;
	//场景相关
	public long lastUpdateTime;
	public float posX;
	public float posY;
	public float posZ;
	public long lastShootTime;
	public float hp = 100;
    //
    public int spotNum;
    public float[] spotx;
    public float[] spoty;
    public float[] spotz;
}
using System;
using System.Collections.Generic;
using System.Linq; 

//房间
public class Room
{
	//状态
	public enum Status
	{
		Prepare = 1,
		Fight = 2 ,
	}
	public Status status = Status.Prepare;
	//用户
	public int maxPlayers = 6;
    //房间中用户字典
	public Dictionary<string,Player> list = new Dictionary<string,Player>();

    //索引号
    int AIIndex = 1;

	//添加用户
	public bool AddPlayer(Player player,int boatModelValue)
	{
		lock (list) 
		{
			if (list.Count >= maxPlayers)
				return false;
			PlayerTempData tempData = player.tempData;
			tempData.room = this; 
			tempData.team = SwichTeam ();
            tempData.boatModel = boatModelValue;
			tempData.status = PlayerTempData.Status.Room;
			
			if(list.Count == 0)
				tempData.isOwner = true;
			string id = player.id;
            if(list.ContainsKey(id))
            {
                id += AIIndex.ToString();
                AIIndex++;
            }
			list.Add(id, player);
		}
		return true;
	}
	
	//分配队伍
	public int SwichTeam()
	{
		int count1 = 0;
		int count2 = 0;
		foreach(Player player in list.Values)
		{
			if(player.tempData.team == 1) count1++;
			if(player.tempData.team == 2) count2++;
		}
		if (count1 <= count2)
			return 1;
		else
			return 2;
	}

	//删除用户
	public void DelPlayer(string id)
	{
		lock (list) 
		{
			if (!list.ContainsKey(id))
				return;
			bool isOwner = list[id].tempData.isOwner;
			list[id].tempData.status = PlayerTempData.Status.None;
			list.Remove(id);
			if(isOwner)
				UpdateOwner();
		}
	}

	//更换主机
	public void UpdateOwner()
	{
		lock (list) 
		{
			if(list.Count <= 0)
				return;
			
			foreach(Player player in list.Values)
			{
				player.tempData.isOwner = false;
			}
			
			Player p = list.Values.First();
			p.tempData.isOwner = true;
		}
	}

	//广播
	public void Broadcast(ProtocolBase protocol)
	{
		foreach(Player player in list.Values)
		{
			player.Send(protocol);
		}
	}

	//房间信息
	public ProtocolBytes GetRoomInfo()
	{
		ProtocolBytes protocol = new ProtocolBytes ();
		protocol.AddString ("GetRoomInfo");
		//房间信息
		protocol.AddInt (list.Count);
		//每个用户信息
		foreach(Player p in list.Values)
		{
			protocol.AddString(p.id);
			protocol.AddInt(p.tempData.team);
            protocol.AddInt(p.tempData.boatModel);
            //protocol.AddInt(p.data.win);
			//protocol.AddInt(p.data.fail);
			int isOwner = p.tempData.isOwner? 1: 0;
			protocol.AddInt(isOwner);
		}
		return protocol;
	}

	//房间能否开启模拟
	public bool CanStart()
	{
		if (status != Status.Prepare)
			return false;
		
		int count1 = 0;
		int count2 = 0;
		
		foreach(Player player in list.Values)
		{
			if(player.tempData.team == 1) count1++;
			if(player.tempData.team == 2) count2++;
		}

        //if (count1 < 1 || count2 < 1)
        if (count1 < 1 && count2 < 1)
            return false;
		
		return true;
	}


	public void StartFight()
	{
		ProtocolBytes protocol = new ProtocolBytes ();
		protocol.AddString ("Fight");
		status = Status.Fight;
		int teamPos1 = 1;
		int teamPos2 = 1;
		lock (list) 
		{
			protocol.AddInt(list.Count);
			foreach(var p in list)
			{
				p.Value.tempData.hp = 200;
				//protocol.AddString(p.id);
                protocol.AddString(p.Key);
                protocol.AddInt(p.Value.tempData.team);
                protocol.AddInt(p.Value.tempData.boatModel);
                if (p.Value.tempData.team == 1)
					protocol.AddInt(teamPos1++);
				else
					protocol.AddInt(teamPos2++);

                p.Value.tempData.status = PlayerTempData.Status.Fight;
			}
			Broadcast(protocol);
		}
	}
}
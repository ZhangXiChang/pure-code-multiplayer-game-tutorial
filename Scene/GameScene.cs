using System.Collections.Generic;
using Godot;
using System;

public partial class GameScene : Node2D
{
    public List<long> mPlayerIDList = new List<long>();

    public override void _EnterTree()
    {
        RpcConfig("ClientConnectionCompleted", new Godot.Collections.Dictionary
        {
            {"rpc_mode",(int)MultiplayerApi.RpcMode.AnyPeer},
            {"transfer_mode",(int)MultiplayerPeer.TransferModeEnum.Reliable},
            {"call_local",false},
            {"channel",0}
        });
        RpcConfig("AddPlayer", new Godot.Collections.Dictionary
        {
            {"rpc_mode",(int)MultiplayerApi.RpcMode.Authority},
            {"transfer_mode",(int)MultiplayerPeer.TransferModeEnum.Reliable},
            {"call_local",false},
            {"channel",0}
        });
    }
    public override void _Ready()
    {
        //服务端和本地客户端代码隔离
        if (Multiplayer.IsServer())
        {
            //添加服务端玩家
            AddPlayer(1);
        }
        else
        {
            //当本地客户端连接服务端完成
            Multiplayer.ConnectedToServer += ConnectedToServer;
        }
    }

    void ConnectedToServer()
    {
        //添加服务端玩家傀儡
        AddPlayer(1);
        //添加本地客户端玩家
        AddPlayer(Multiplayer.GetUniqueId());
        //通知服务端本地客户端准备完毕
        Rpc("ClientConnectionCompleted", Multiplayer.GetUniqueId());
    }
    void ClientConnectionCompleted(long id)
    {
        //添加远端客户端玩家
        AddPlayer(id);
        //通知其他远端客户端添加此客户端玩家
        foreach (var item in mPlayerIDList)
        {
            RpcId(item, "AddPlayer", id);
        }
        //通知此客户端添加其他远端客户端
        foreach (var item in mPlayerIDList)
        {
            RpcId(id, "AddPlayer", item);
        }
        //记录远端客户端玩家ID
        AddPlayerID(id);
    }
    void AddPlayerID(long id)
    {
        mPlayerIDList.Add(id);
    }
    void AddPlayer(long id)
    {
        var PlayerInstance = new Player();
        PlayerInstance.Name = "Player_" + id.ToString();
        PlayerInstance.SetMultiplayerAuthority((int)id);
        PlayerInstance.mCharacterPath = "res://Character/RainbowMan/RainbowMan.tscn";
        AddChild(PlayerInstance);
    }
    void RemovePlayer(long id)
    {
        for (int i = 0; i < GetChildCount(); i++)
        {
            if (GetChild(i).GetMultiplayerAuthority() == id)
            {
                RemoveChild(GetChild(i));
            }
        }
    }
}

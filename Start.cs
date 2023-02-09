using Godot;
using System;

public partial class Start : Control
{
    void CreateHostButton()
    {
        //创建服务端
        var MultiplayerPeer = new ENetMultiplayerPeer();
        MultiplayerPeer.CreateServer(10270);
        Multiplayer.MultiplayerPeer = MultiplayerPeer;
        //切换场景
        GetTree().ChangeSceneToFile("res://Scene/GameScene.tscn");
    }
    void JoinHostButton()
    {
        //创建客户端
        var MultiplayerPeer = new ENetMultiplayerPeer();
        MultiplayerPeer.CreateClient(GetNode<LineEdit>("VBoxContainer/IP").Text, 10270);
        Multiplayer.MultiplayerPeer = MultiplayerPeer;
        //切换场景
        GetTree().ChangeSceneToFile("res://Scene/GameScene.tscn");
    }
    void QuitGameButton()
    {
        GetTree().Quit();
    }
}

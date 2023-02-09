using Godot;
using System;

public partial class Player : Node
{
    public String mCharacterPath;

    CharacterBody2D mCharacter;
    float mDefaultGravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");

    public override void _EnterTree()
    {
        RpcConfig("SyncCharacter", new Godot.Collections.Dictionary
        {
            {"rpc_mode",(int)MultiplayerApi.RpcMode.Authority},
            {"transfer_mode",(int)MultiplayerPeer.TransferModeEnum.UnreliableOrdered},
            {"call_local",false},
            {"channel",1}
        });
    }
    public override void _Ready()
    {
        mCharacter = GD.Load<PackedScene>(mCharacterPath).Instantiate<CharacterBody2D>();
        mCharacter.Position = GetNode<Marker2D>("../PlayerBirthPoint").Position;
        AddChild(mCharacter);
    }
    public override void _PhysicsProcess(double delta)
    {
        if (IsMultiplayerAuthority() && mCharacter != null)
        {
            Vector2 velocity = mCharacter.Velocity;
            if (!mCharacter.IsOnFloor())
            {
                velocity.Y += mDefaultGravity * (float)delta;
            }
            if (Input.IsActionJustPressed("Jump") && mCharacter.IsOnFloor())
            {
                velocity.Y = -400.0f;
            }
            Vector2 direction = Input.GetVector("Move_Left", "Move_Right", " ", " ");
            if (direction != Vector2.Zero)
            {
                velocity.X = direction.X * 300.0f;
            }
            else
            {
                velocity.X = Mathf.MoveToward(mCharacter.Velocity.X, 0, 300.0f);
            }
            mCharacter.Velocity = velocity;
            mCharacter.MoveAndSlide();
            Rpc("SyncCharacter", mCharacter.Position);
        }
    }
    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("QuitGame"))
        {
            GetTree().Quit();
        }
    }

    void SyncCharacter(Vector2 position)
    {
        mCharacter.Position = position;
    }
}

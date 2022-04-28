using System.Collections.Generic;

using Godot;

using MathPuzzle.Scripts.Attributes;
using MathPuzzle.Scripts.Extensions;
using MathPuzzle.Scripts.Game.Blocks.Items;
using MathPuzzle.Scripts.Global;

namespace MathPuzzle.Scripts.Game
{
    public class Block : Node2D
    {
        [GetNode ("Icon")] public Sprite Icon { get; protected set; }

        public GameMap ParentMap;
        public Vector2 MapPosition;

        public string ID;

        public int ProcessOrder;
        public bool CannotMoveThisProcess;

        public BlockParams Params;
        public bool IsStatic;

        public bool IsFloor;
        public bool IsPlayer;
        public bool IsSatisfied;
        public BlockType BlockType;

        public Vector2 TargetPosition;
        public Vector2 TargetScale;

        public static readonly Vector2 BlockSize = Vector2.One * 64;

        public virtual void InitOnReady ()
        {
            if (IsStatic)
                Icon.Modulate = new Color (0.3F, 0.3F, 0.3F);
        }

        public virtual void Init (BlockParams blockParams)
        {
            Params = blockParams;
            if (Params.TryGet<bool> ("static", out var isStatic))
            {
                IsStatic = isStatic;
            }
        }

        public override string ToString ()
        {
            return GetType ().Name;
        }

        public override void _Process (float delta)
        {
            Position = Position.LinearInterpolate (TargetPosition, 30.0F * delta);
            Scale = Scale.LinearInterpolate (TargetScale, 30.0F * delta);
        }

        public void Spawn ()
        {
            TargetPosition = BlockSize * (MapPosition + new Vector2 (0.5F, 0.5F)) - ParentMap.Size / 2;
            Scale = Vector2.Zero;
            TargetScale = Vector2.One;
        }

        public void Delete ()
        {
            GetParent ().RemoveChild (this);
            G.Inst.CurrentLevel.UnusedBlocksNode.AddChild (this);
            Position = new Vector2 (-100, -100);
            TargetPosition = Position;
            Scale = Vector2.Zero;
            TargetScale = Scale;
        }

        public void Move ()
        {
            TargetPosition = BlockSize * (MapPosition + new Vector2 (0.5F, 0.5F)) - ParentMap.Size / 2;
            // Logger.Debug ($"{GetType().Name} move to {TargetPosition}");
        }

        public virtual void Refresh () { }

        public virtual bool Combine (Block b, out Block output)
        {
            output = this;
            return false;
        }

        public virtual bool Operate (Block a, Block b, out Block output)
        {
            output = a;
            return false;
        }

        public static bool IsValidVariable (params Block[] blocks)
        {
            foreach (var block in blocks)
            {
                if (block is Variable @var)
                {
                    if (!@var.ParentMap.VariableMap.TryGetValue (@var.VariableData, out var v) || v == null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
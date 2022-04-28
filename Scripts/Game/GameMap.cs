using System.Collections.Generic;
using System.Linq;

using Godot;

using MathPuzzle.Scripts.Attributes;
using MathPuzzle.Scripts.Extensions;
using MathPuzzle.Scripts.Game;

public class GameMap : Node2D
{
    [GetNode ("Sprite")] public Sprite Sprite { get; protected set; }

    [GetNode ("Blocks")] public Node2D BlocksNode { get; protected set; }

    [GetNode ("Floors")] public Node2D FloorsNode { get; protected set; }

    public Vector2 Size;

    public List<Block> Blocks = new List<Block> ();
    public List<Block> Floors = new List<Block> ();
    public Dictionary<string, Block> VariableMap = new Dictionary<string, Block> ();

    public readonly Dictionary<Vector2, Block> BlockMap = new Dictionary<Vector2, Block> ();
    public readonly Dictionary<Vector2, Block> FloorMap = new Dictionary<Vector2, Block> ();

    public Vector2 MapSize = new Vector2 ();

    public override void _Ready ()
    {
        this.InitNode ();
        Position = GetViewport ().Size / 2;
        Sprite.Scale = MapSize;
        Size = Sprite.Scale * Sprite.Texture.GetSize ();
        foreach (var b in Blocks)
        {
            BlocksNode.AddChild (b);
            b.Spawn ();
        }
        foreach (var f in Floors)
        {
            FloorsNode.AddChild (f);
            f.Spawn ();
        }
    }

    public bool CheckRange (Vector2 point)
    {
        return point.x >= 0 && point.x < MapSize.x && point.y >= 0 && point.y < MapSize.y;
    }

    public void RefreshMap ()
    {
        Blocks = BlockMap.Values.OrderBy (b => b?.ProcessOrder).Where (b => b != null).ToList ();
        Floors = FloorMap.Values.OrderBy (b => b?.ProcessOrder).Where (b => b != null).ToList ();
    }

    public void SetBlock (Vector2 pos, Block block, bool instant = false)
    {
        BlockMap[pos] = block;
        block.MapPosition = pos;
        block.ParentMap = this;
    }

    public void AddBlock (Vector2 pos, Block block, bool instant = false)
    {
        if (BlockMap.ContainsKey (pos)) return;
        BlockMap.Add (pos, block);
        block.MapPosition = pos;
        block.ParentMap = this;
    }

    public Block RemoveBlock (Vector2 pos, bool delete = false)
    {
        if (!BlockMap.TryGetValue (pos, out var block)) return null;
        BlockMap.Remove (pos);
        return block;
    }

    public void SetFloor (Vector2 pos, Block block, bool instant = false)
    {
        FloorMap[pos] = block;
        block.MapPosition = pos;
        block.ParentMap = this;
    }

    public void AddFloor (Vector2 pos, Block block, bool instant = false)
    {
        if (FloorMap.ContainsKey (pos)) return;
        FloorMap.Add (pos, block);
        block.MapPosition = pos;
        block.ParentMap = this;
    }

    public Block RemoveFloor (Vector2 pos, bool delete = false)
    {
        if (!FloorMap.TryGetValue (pos, out var block)) return null;
        FloorMap.Remove (pos);
        return block;
    }

    public void Process () { }
}
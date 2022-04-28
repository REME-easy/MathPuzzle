using System;
using System.Collections.Generic;
using System.Diagnostics;

using Godot;

using MathPuzzle.Scripts.Extensions;
using MathPuzzle.Scripts.Global;

using Tomlyn.Model;

namespace MathPuzzle.Scripts.Game
{
    public class GameLevelBuilder
    {
        public string ID;
        public TomlTable Code;
        public Dictionary<string, BlockPrototype> MappingTable;
        public Dictionary<string, MapPrototype> Maps;

        public static readonly PackedScene GameLevelFac = (PackedScene) ResourceLoader.Load ("res://Scenes/GameLevel.tscn");
        public static readonly PackedScene GameMapFac = (PackedScene) ResourceLoader.Load ("res://Scenes/GameMap.tscn");

        public GameLevelBuilder (string id)
        {
            ID = id;
            Code = LevelMgr.Levels[id];
            MappingTable = new Dictionary<string, BlockPrototype> ();
            Maps = new Dictionary<string, MapPrototype> ();
        }

        public bool Build (out GameLevel level)
        {
            var sw = new Stopwatch ();
            sw.Start ();
            level = GameLevelFac.Instance<GameLevel> ();
            try
            {
                ParseMappingTable ();
                ParsingMaps ();
                AddMaps (level);
                level.LevelID = ID;
            }
            catch (Exception ex)
            {
                Logger.Debug (ex);
                return false;
            }
            G.Inst.View.SetScene (level);
            G.Inst.CurrentLevel = level;
            sw.Stop ();
            Logger.Debug ($"load level use time:{sw.ElapsedMilliseconds} ms");
            return true;
        }

        private void ParseMappingTable ()
        {
            var blocks = Code["blocks"] as TomlTable;
            if (blocks == null)
                return;

            var en = blocks.GetEnumerator ();
            while (en.MoveNext ())
            {
                var proto = new BlockPrototype ();
                proto.Params = new BlockParams ();
                var typeName = "";
                var name = en.Current.Key;
                var prop = en.Current.Value;
                if (prop is string str)
                    typeName = str;
                else if (prop is TomlTable tab)
                {
                    typeName = tab["type"] as string;

                    if (tab.ContainsKey ("params"))
                    {
                        var @params = tab["params"] as TomlTable;
                        var en2 = @params.GetEnumerator ();
                        while (en2.MoveNext ())
                            proto.Params.Params.Add (en2.Current.Key, en2.Current.Value);
                    }
                }

                proto.ScenePath = DataMgr.Scenes[typeName];
                MappingTable.Add (name, proto);
            }
        }

        private void ParsingMaps ()
        {
            var maps = Code["maps"] as TomlTableArray;
            for (int i = 0; i < maps.Count; i++)
            {
                var map = maps[i] as TomlTable;
                var proto = new MapPrototype ();
                var name = map["name"] as string;
                proto.Tiles = new List<string> ();
                proto.Floors = new List<string> ();

                var tiles = map["tiles"] as TomlArray;
                for (var y = 0; y < tiles.Count; y++)
                {
                    proto.Tiles.Add (tiles[y] as string);
                }

                var floors = map["floors"] as TomlArray;
                for (var y = 0; y < floors.Count; y++)
                {
                    proto.Floors.Add (floors[y] as string);
                }

                Maps.Add (name, proto);
            }
        }

        private void AddMaps (GameLevel level)
        {
            foreach (var entry in Maps)
            {
                var map = GameMapFac.Instance<GameMap> ();
                AddBlocks (entry.Key, entry.Value, map);
                level.Maps.Add (map);
            }
        }

        private void AddBlocks (string name, MapPrototype prototype, GameMap map)
        {
            map.MapSize = new Vector2 (prototype.Tiles[0].Length, prototype.Tiles.Count);
            for (var y = 0; y < prototype.Tiles.Count; y++)
            {
                var row = prototype.Tiles[y];
                for (var x = 0; x < row.Length; x++)
                {
                    var ch = row[x];

                    if (ch == '.')
                        continue;

                    if (MappingTable.TryGetValue (ch.ToString (), out var proto))
                    {
                        var block = ResourceLoader.Load<PackedScene> (proto.ScenePath).Instance<Block> ();
                        // block = block.SafelySetScript<Block> (proto.ScriptPath);
                        block.Init (proto.Params);
                        block.MapPosition = new Vector2 (x, y);
                        block.ParentMap = map;
                        map.Blocks.Add (block);
                        map.BlockMap.Add (block.MapPosition, block);
                    }
                }
            }

            for (var y = 0; y < prototype.Floors.Count; y++)
            {
                var row = prototype.Floors[y];
                for (var x = 0; x < row.Length; x++)
                {
                    var ch = row[x];

                    if (ch == '.')
                        continue;

                    if (MappingTable.TryGetValue (ch.ToString (), out var proto))
                    {
                        var block = ResourceLoader.Load<PackedScene> (proto.ScenePath).Instance<Block> ();
                        // block = block.SafelySetScript<Block> (proto.ScriptPath);
                        block.Init (proto.Params);
                        block.IsFloor = true;
                        block.ParentMap = map;
                        block.MapPosition = new Vector2 (x, y);
                        map.Floors.Add (block);
                        map.FloorMap.Add (block.MapPosition, block);

                    }
                }
            }
        }

        public struct BlockPrototype
        {
            public string ScenePath;
            public BlockParams Params;
        }

        public struct MapPrototype
        {
            public List<string> Tiles;
            public List<string> Floors;
        }
    }
}
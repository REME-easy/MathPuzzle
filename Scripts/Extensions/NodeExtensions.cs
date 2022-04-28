using System;
using System.Reflection;
using Godot;
using MathPuzzle.Scripts.Attributes;

namespace MathPuzzle.Scripts.Extensions
{
    public static class NodeExtension
    {
        public static void InitNode(this Node node)
        {
            foreach (var field in node.GetType()
                         .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                foreach (var attr in field.GetCustomAttributes(true))
                {
                    if (!(attr is GetNodeAttribute nodeAttr)) continue;
                    var tmp = node.GetNode(nodeAttr.NodePath);
                    if (tmp == null)
                        throw new Exception($"cannot get node from path \"{nodeAttr.NodePath}\".");
                    try
                    {
                        field.SetValue(node, tmp);
                    }
                    catch (ArgumentException)
                    {
                        throw new Exception(
                            $"cannot set {field} to node with path \"{nodeAttr.NodePath}\".");
                    }
                }
            }

            foreach (var property in node.GetType()
                         .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                foreach (var attr in property.GetCustomAttributes(true))
                {
                    if (!(attr is GetNodeAttribute nodeAttr)) continue;
                    var tmp = node.GetNode(nodeAttr.NodePath);
                    if (tmp == null)
                        throw new Exception($"cannot get node from path \"{nodeAttr.NodePath}\".");
                    try
                    {
                        property.SetValue(node, tmp);
                    }
                    catch (ArgumentException)
                    {
                        throw new Exception(
                            $"cannot set {property} to node with path \"{nodeAttr.NodePath}\".");
                    }
                }
            }
        }
    }
}
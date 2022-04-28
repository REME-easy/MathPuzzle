using System;

namespace MathPuzzle.Scripts.Attributes
{
    /// <summary>
    /// 将指定路径的节点分配给该属性。
    /// </summary>
    [AttributeUsage (AttributeTargets.Field | AttributeTargets.Property)]
    public class GetNodeAttribute : Attribute {
        public string NodePath { get; }

        public GetNodeAttribute (string path) {
            NodePath = path;
        }
    }
}
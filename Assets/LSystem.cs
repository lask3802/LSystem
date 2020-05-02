using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DefaultNamespace
{
    public static class LSystem
    {
       
        public static void DrawForward(LSystemContext context, float distance)
        {
            var transform = context.TransformInfo;
            var newPoint = transform.Rotation*(new Vector3(0,distance,0)) + transform.Position;
            
            context.LineDrawer.PushLine(transform.Position, newPoint);
            
            transform.Position = newPoint;
            context.TransformInfo = transform;
        }

        public static void Rotate(LSystemContext context, Quaternion quaternion)
        {
            var contextTransformInfo = context.TransformInfo;
            contextTransformInfo.Rotation *= quaternion;
            context.TransformInfo = contextTransformInfo;
        }

        public static void PushTransform(LSystemContext context)
        {
            context.TransformStack.Push(context.TransformInfo);
        }

        public static void PopTransform(LSystemContext context)
        {
            context.TransformInfo = context.TransformStack.Pop();
        }

        public static string NextIteration(string state, LSystemRule[] rules)
        {
            var sb = new StringBuilder();
            foreach (var var in state)
            {
                var matched = false;
                foreach (var rule in rules)
                {
                    if (char.ToUpper(var) == char.ToUpper(rule.Variable))
                    {
                        matched = true;
                        sb.Append(rule.NewVariable);
                    }
                }
                if (!matched) sb.Append(var);
            }

            return sb.ToString();
        }

        public static void ExecuteAction(LSystemContext context, char variable, IDictionary<char,Action<LSystemContext>> actions)
        {
            if (actions.TryGetValue(variable, out var action))
            {
                action.Invoke(context);
            }
        }

        public static IEnumerator ExecuteActions(LSystemContext context, string state,
            IDictionary<char, Action<LSystemContext>> actions)
        {
            foreach (var variable in state)
            {
                ExecuteAction(context, variable, actions);
                yield return null;
            }
        }

    }
    
    public class LSystemContext
    {
        public Stack<TransformInfo> TransformStack;
        public TransformInfo TransformInfo;
        public readonly ILineDrawer LineDrawer;

        public LSystemContext(ILineDrawer lineDrawer)
        {
            LineDrawer = lineDrawer;
            TransformStack = new Stack<TransformInfo>();
            TransformInfo = new TransformInfo {Rotation = Quaternion.LookRotation(Vector3.forward)};
        }

        public void ResetTransformInfo()
        {
            TransformInfo.Rotation = Quaternion.LookRotation(Vector3.forward);
            TransformInfo.Position = Vector3.zero;
            
        }
    }

    [Serializable]
    public struct TransformInfo
    {
        public Vector3 Position;
        public Quaternion Rotation;
    }

    [Serializable]
    public class LSystemRule
    {
        public char Variable;
        public string NewVariable;
    }
    
    [Serializable]
    public class LSystemAction
    {
        public char Variable;
        public Action<LSystemContext> Action;
    }
}
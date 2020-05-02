using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class TriangleSystem : MonoBehaviour
    {
        public MeshLineDrawer LineDrawer;
        public string StartState;
        public float DrawFowardLength = 1;
        public int Round;
        public float RotateDegree = 25;
        public LSystemRule[] Rules;

        private float mCurrentLength;

        private LSystemContext mContext;
       

        IEnumerator Start()
        {
            Application.targetFrameRate = 500;
            mContext = new LSystemContext(LineDrawer);
            
            var actions = new Dictionary<char, Action<LSystemContext>>
            {
                {'+', context => LSystem.Rotate(context, Quaternion.Euler(0,0,RotateDegree))},
                {'-', context => LSystem.Rotate(context, Quaternion.Euler(0,0,-RotateDegree))},
                {'F', context => LSystem.DrawForward(context, mCurrentLength)},
                {'G', context => LSystem.DrawForward(context, mCurrentLength)},
                {'[', LSystem.PushTransform},
                {']', LSystem.PopTransform},
            };

            var state = StartState;
            mCurrentLength = DrawFowardLength;
            for (var currentRound = 0; currentRound < Round; currentRound++)
            {
                state = LSystem.NextIteration(state, Rules);
                Debug.Log(state);
            }
            
            var iter = LSystem.ExecuteActions(mContext, state, actions);
            while (iter.MoveNext())
            {
                for (var cnt = 0; cnt < state.Length*0.01; cnt++)
                {
                    if (!iter.MoveNext()) break;
                }

                yield return null;
            }
        }
    }
}
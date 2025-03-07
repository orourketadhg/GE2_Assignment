﻿using ie.TUDublin.GE2.Components.Steering;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

namespace ie.TUDublin.GE2.Systems.Steering.SteeringJobs {

    /// <summary>
    /// Job to calculate Arrive steering forces
    /// </summary>
    [BurstCompile]
    public struct WanderJob : IJobEntityBatch {
        
        [NativeSetThreadIndex] private int _nativeThreadIndex;
        [NativeDisableParallelForRestriction] public NativeArray<Random> RandomArray;
        public float DeltaTime;
        
        // Component Handlers
        [ReadOnly] public ComponentTypeHandle<Translation> TranslationHandle;
        [ReadOnly] public ComponentTypeHandle<Rotation> RotationHandle;
        
        public ComponentTypeHandle<WanderData> WanderHandle;

        public void Execute(ArchetypeChunk batchInChunk, int batchIndex) {
            // Get component arrays from batch
            var wanderData = batchInChunk.GetNativeArray(WanderHandle);
            var translationData = batchInChunk.GetNativeArray(TranslationHandle);
            var rotationData = batchInChunk.GetNativeArray(RotationHandle);
            
            var random = RandomArray[_nativeThreadIndex];

            for (int i = 0; i < batchInChunk.Count; i++) {
                // get entities components and data
                var wander = wanderData[i];
                var position = translationData[i].Value;
                var rotation = rotationData[i].Value;
                
                // calculate wander forces
                var displacement = wander.Jitter * random.NextFloat3Direction() * DeltaTime;
                wander.Target += displacement;
                wander.Target = math.normalize(wander.Target);
                wander.Target *= wander.Radius;
                
                wander.LocalTarget = ( math.forward() * wander.Distance) + wander.Target;
                
                wander.WorldTarget = math.mul(rotation, wander.LocalTarget) + position;
                wander.Force = ( wander.WorldTarget - position );

                // return data
                wanderData[i] = wander;
            }
            
            RandomArray[_nativeThreadIndex] = random;
            
        }
        
    }

}
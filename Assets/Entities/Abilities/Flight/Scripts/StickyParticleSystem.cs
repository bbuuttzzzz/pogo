using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

public class StickyParticleSystem : MonoBehaviour
{
    // Start is called before the first frame update
    public ParticleSystem Target;

    public AnimationCurve SpaceChangeCurve;
    Matrix4x4 lastWorldToLocalMatrix;
    JobHandle lastJobHandle;

    UpdateParticlesJob job;

    public void Awake()
    {
        job = new UpdateParticlesJob(default, SpaceChangeCurve);
    }

    // Update is called once per frame
    void Update()
    {
        lastJobHandle.Complete();
        job.transformMatrix = transform.localToWorldMatrix * lastWorldToLocalMatrix;
        lastJobHandle = job.Schedule(Target);

        lastWorldToLocalMatrix = transform.worldToLocalMatrix;
    }

    private void OnDestroy()
    {
        lastJobHandle.Complete();
        job.Dispose();
    }


    struct UpdateParticlesJob : IJobParticleSystem
    {
        public Matrix4x4 transformMatrix;
        private NativeArray<float> lookupTable;

        public UpdateParticlesJob(Matrix4x4 transformMatrix, AnimationCurve curve, int lookupTableResolution = 100) : this()
        {
            this.transformMatrix = transformMatrix;
            lookupTable = new NativeArray<float>(lookupTableResolution, Allocator.Persistent);
            for(int n = 0; n < lookupTableResolution; n++)
            {
                lookupTable[n] = curve.Evaluate(n / (float)(lookupTableResolution - 1));
            }
        }

        public void Execute(ParticleSystemJobData jobData)
        {
            var positions = jobData.positions;
            var lifeTimePercentages = jobData.aliveTimePercent;
            for (int i = 0; i < jobData.count; i++)
            {
                float tRaw = lifeTimePercentages[i] / 100f;
                Vector3 transformFollowedPosition = transformMatrix.MultiplyPoint(positions[i]);
                positions[i] = Vector3.Lerp(positions[i], transformFollowedPosition, SampleLookupTable(tRaw));
            }
        }

        private float SampleLookupTable(float t)
        {
            int index = (int)(t * lookupTable.Length);
            return lookupTable[Math.Clamp(index, 0, lookupTable.Length)];
        }

        public void Dispose()
        {
            lookupTable.Dispose();
        }
    }
}

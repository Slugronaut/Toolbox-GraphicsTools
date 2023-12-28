using System.Collections.Generic;
using Peg.Trackables;
using Peg.Collections;
using Peg.Lib;
using UnityEngine;



namespace Peg.Graphics
{
    /// <summary>
    /// Performs a raycast and fades the alpha
    /// of any object detected that has a 
    /// RendererAlphaProp component attached to it.
    /// 
    /// TODO: Update() needs some work to remove garbage generation
    /// 
    /// WARNING: There might be a memory leak with the timed fading mechanism!!
    /// </summary>
    public class LineOfSightFader : AbstractTracker
    {
        [Tooltip("How frequently to raycast and adjust alpha values.")]
        public float Freq = 0.1f;
        [Tooltip("The thickness of the raycast.")]
        public float Radius = 2.0f;
        [Tooltip("The layers to raycast on.")]
        public LayerMask Layers;
        [Tooltip("Subtracted from the ray that is cast from this object to the target. Increasing this may be useful for decreasing the number of objects that fade when just behind the target.")]
        public float DistanceReduction = 1.0f;
        [Tooltip("An additional offset vector that is applied to the target position. Useful for adjusting the raycast point relative to the target.")]
        public Vector3 TargetOffset;
        [Tooltip("A falloff curve that affects how alpha is scaled based on distance to center of the raycast.")]
        public AnimationCurve Falloff;
        [Range(0,1)]
        public float MinAlpha = 0.1f;
        public float FadeSpeed = 2;

        public enum AdjustMode
        {
            HitPoint,
            Centroid,
            TragetCenter,
        }

        [Tooltip("Adjusts the hit point so that it redirect to the center of the object rather that the actual hit.")]
        public bool AdjustHit = true;

        [Tooltip("If fadable geometry has the potention to be removed at runtime, this will ensure null-refs are handled. Leave disabled if not needed for performance gain.")]
        public bool CheckNullRefs = false;

        //[Tooltip("The angle beyond which a collision won't count.")]
        //public float AngleThreshold = 5f;

#if UNITY_EDITOR
        [Tooltip("Draws debug rays. Editor-only. Removed on build")]
        public bool DebugRays = false;
#endif

        float LastTime;
        List<RendererAlphaProp> FrameAccum = new List<RendererAlphaProp>(20);
        List<RendererAlphaProp> LifetimeAccum = new List<RendererAlphaProp>(20);

        /// <summary>
        /// 
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
        }

        void OnDisable()
        {
            foreach(var fin in FadeIns)
            {
                if (!TypeHelper.IsReferenceNull(fin)) fin.Alpha = 1;
            }

            foreach(var fout in FadeOuts)
            {
                if (!TypeHelper.IsReferenceNull(fout)) fout.Alpha = 1;
            }

            foreach(var accum in LifetimeAccum)
            {
                if (!TypeHelper.IsReferenceNull(LifetimeAccum)) accum.Alpha = 1;
            }

            FadeIns.Clear();
            FadeOuts.Clear();
            FrameAccum.Clear();
            LifetimeAccum.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            float time = Time.time;
            if (time - LastTime >= Freq)
            {
                LastTime = time;

                for (int i = 0; i < Positions.Count; i++)
                    ApplyTimedFade(Positions[i]);

                //anything that we didn't confirm as being affected this frame, we need to remove.
                //It is likely something that was affected previously and now needs to be reset.
                RendererAlphaProp prop = null;
                for (int i = 0; i < LifetimeAccum.Count; i++)
                {
                    prop = LifetimeAccum[i];

                    if(CheckNullRefs && TypeHelper.IsReferenceNull(prop))
                    {
                        LifetimeAccum.Remove(prop);
                        continue;
                    }

                    if (!FrameAccum.Contains(prop))
                    {
                        //prop.Alpha = 1;
                        LifetimeAccum.Remove(prop);
                        i--;
                        //schedule for fade-in
                        FadeIns.Add(prop);
                        if (FadeOuts.Contains(prop)) FadeOuts.Remove(prop);
                    }
                }
                FrameAccum.Clear();
            }
            FadeAccumulations(MinAlpha, FadeSpeed);
        }

        static readonly List<Vector3> CentroidList = new(2);
        /// <summary>
        /// All objects that are spherecast between this object and the target are faded out over time.
        /// Any objects previously faded out that are no longer occulding the target will be faded back
        /// in over time.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="minFade"></param>
        /// <param name="time"></param>
        public void ApplyTimedFade(Vector3 targetPos)
        {
            var hits = SharedArrayFactory.Hit50;
            Vector3 myPos = MyTrans.position;
            Vector3 line = targetPos + TargetOffset - myPos;
            //Debug.DrawLine(myPos, targetPos + TargetOffset, Color.red);
            float rayDist = line.magnitude - DistanceReduction;
            Vector3 forward = line.normalized;
            int x = Physics.SphereCastNonAlloc(myPos, Radius, forward, hits, rayDist, Layers, QueryTriggerInteraction.Collide);

            if (x > 0)
            {
                for (int i = 0; i < x; i++)
                {
                    var hit = hits[i];
                    var comp = hit.collider.gameObject.GetComponent<RendererAlphaProp>();
                    if (comp != null)
                    {
                        /*
                        //don't fade things that aren't within an angle of acceptance
                        if (comp.UseNormal)
                        {
                            var f = comp.Facing;
                            float angle = Vector3.Angle(line, f);
                            if (Mathf.Abs(90 - angle) < AngleThreshold)
                                continue;
                        }
                        */

                        //if the object that was hit is further away than our target, don't fade
                        CentroidList.Clear();
                        CentroidList.Add(targetPos);
                        CentroidList.Add(hit.point + (forward * Radius * 0.9f));

                        Vector3 adjusted;

                        if (AdjustHit)
                            adjusted = hit.collider.ClosestPointOnBounds(MathUtils.GetCentroid(CentroidList));// new Vector3(hit.point.x, targetPos.y, hit.point.z) - (forward*Radius);
                        else adjusted = hit.point;

                        #if UNITY_EDITOR
                        if (DebugRays)
                        {
                            Debug.DrawLine(adjusted, myPos, Color.red);
                            //Debug.DrawLine(targetPos, myPos, Color.green);
                            //Debug.DrawLine(hit.point, myPos, Color.blue);
                        }
                        #endif
                        if ((adjusted - myPos).sqrMagnitude > ((targetPos - myPos).sqrMagnitude - (DistanceReduction*DistanceReduction)))
                            continue;

                        var vec = myPos + (forward * (hit.distance + Radius));
                        //Debug.DrawLine(myPos, hit.point, Color.green);
                        vec = vec - hit.point;
                        float vecMag = vec.magnitude;
                        if (hit.distance + vecMag < rayDist - Radius)
                        {
                            FrameAccum.Add(comp);
                            if (!LifetimeAccum.Contains(comp))
                            {
                                LifetimeAccum.Add(comp);
                                //schedule for fade-out
                                FadeOuts.Add(comp);
                                if (FadeIns.Contains(comp)) FadeIns.Remove(comp);
                            }
                        }
                    }

                }
            }
        }

        readonly HashSet<RendererAlphaProp> FadeIns = new();
        readonly HashSet<RendererAlphaProp> FadeOuts = new();
        readonly List<RendererAlphaProp> rem = new(10);


        /// <summary>
        /// Helper that fades recently added or removed props
        /// </summary>
        void FadeAccumulations(float minFade, float fadeSpeed)
        {
            //do fade-outs first
            rem.Clear();
            foreach (var fade in FadeOuts)
            {
                if(CheckNullRefs && TypeHelper.IsReferenceNull(fade))
                    rem.Add(fade);
                else if (fade.Alpha <= minFade)
                {
                    fade.Alpha = minFade;
                    rem.Add(fade);
                }
                else fade.Alpha -= fadeSpeed * Time.unscaledDeltaTime;
            }

            for (int i = 0; i < rem.Count; i++)
                FadeOuts.Remove(rem[i]);


            //next do fade-ins
            rem.Clear();
            foreach (var fade in FadeIns)
            {
                if (CheckNullRefs && TypeHelper.IsReferenceNull(fade))
                    rem.Add(fade);
                else if (fade.Alpha >= 1)
                {
                    fade.Alpha = 1;
                    rem.Add(fade);
                }
                else fade.Alpha += fadeSpeed * Time.unscaledDeltaTime;
            }

            for (int i = 0; i < rem.Count; i++)
                FadeIns.Remove(rem[i]);
        }

        /// <summary>
        /// Fades all objects that are sphercast between this object and the target. The closer
        /// they are to the spherecast's center-line, the more they are faded.
        /// </summary>
        /// <param name="target"></param>
        public void ApplyDistanceFade(Transform target)
        {
            if (target == null) return;
            
            var targetPos = target.position;
            var hits = SharedArrayFactory.Hit50;
            Vector3 myPos = MyTrans.position;
            Vector3 line = targetPos + TargetOffset - myPos;
            //Debug.DrawLine(myPos, target.position + TargetOffset, Color.red);
            float rayDist = line.magnitude - DistanceReduction;
            Vector3 forward = line.normalized;
            int x = Physics.SphereCastNonAlloc(myPos, 0.1f, forward, hits, rayDist, Layers, QueryTriggerInteraction.Collide);

            if (x > 0)
            {
                for (int i = 0; i < x; i++)
                {
                    var hit = hits[i];
                    var comp = hit.collider.gameObject.GetComponent<RendererAlphaProp>();
                    if (comp != null)
                    {
                        //if the object that was hit is further away than our target, don't fade
                        //if ((hit.collider.bounds.center - myPos).sqrMagnitude > ((targetPos - myPos).sqrMagnitude - (DistanceReduction*DistanceReduction)))
                        //    continue;

                        var vec = myPos + (forward * (hit.distance + Radius));
                        //Debug.DrawLine(myPos, hit.point, Color.green);
                        vec = vec - hit.point;
                        float vecMag = vec.magnitude;
                        if (hit.distance + vecMag < rayDist - Radius)
                        {
                            FrameAccum.Add(comp);
                            if (!LifetimeAccum.Contains(comp)) LifetimeAccum.Add(comp);
                            float a = Mathf.Max(Falloff.Evaluate(vecMag / Radius), MinAlpha);
                            comp.Alpha = a;
                        }
                    }

                }
            }

            //anything that we didn't confirm as being affected this frame, we need to remove.
            //It is likely something that was affected previously and now needs to be reset.
            RendererAlphaProp prop = null;
            for (int i = 0; i < LifetimeAccum.Count; i++)
            {
                prop = LifetimeAccum[i];
                if (!FrameAccum.Contains(prop))
                {
                    prop.Alpha = 1;
                    LifetimeAccum.Remove(prop);
                    i--;
                }
            }
            FrameAccum.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        protected override void OnBeginTracking(TrackableSpawnedEvent msg) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        protected override void OnEndTracking(TrackableRemovedEvent msg) { }
    }
}

﻿using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace AnimeRx
{
    public static partial class Anime
    {
        public static IObservable<Vector4> Play(Vector4 from, Vector4 to, IAnimator animator)
        {
            return Play(from, to, animator, DefaultScheduler);
        }

        public static IObservable<Vector4> Play(Vector4 from, Vector4 to, IAnimator animator, IScheduler scheduler)
        {
            return PlayInternal(animator, Vector4.Distance(from, to), scheduler)
                .Select(x => Vector4.LerpUnclamped(from, to, x));
        }

        public static IObservable<Vector4> Play(Vector4[] path, IAnimator animator)
        {
            return Play(path, animator, DefaultScheduler);
        }

        public static IObservable<Vector4> Play(Vector4[] path, IAnimator animator, IScheduler scheduler)
        {
            var distance = new List<float>();
            var sum = 0.0f;
            for (var i = 0; i < path.Length - 1; ++i)
            {
                var d = Vector4.Distance(path[i], path[i + 1]);
                distance.Add(sum + d);
                sum += d;
            }
            return PlayInternal(animator, sum, scheduler)
                .Select(x =>
                {
                    var a = x * sum;
                    var i = 0;
                    for (; i < distance.Count - 1; i++)
                    {
                        if (distance[i] > a) break;
                    }

                    var b = i == 0 ? 0 : distance[i - 1];
                    return Vector4.LerpUnclamped(path[i], path[i + 1], (a - b) / (distance[i] - b));
                });
        }

        public static IObservable<Vector4> Play(this IObservable<Vector4> self, Vector4 from, Vector4 to, IAnimator animator)
        {
            return Play(self, from, to, animator, DefaultScheduler);
        }

        public static IObservable<Vector4> Play(this IObservable<Vector4> self, Vector4 from, Vector4 to, IAnimator animator, IScheduler scheduler)
        {
            return self.Concat(Play(from, to, animator, scheduler));
        }

        public static IObservable<Vector4> Play(this IObservable<Vector4> self, Vector4 from, Vector4[] path, IAnimator animator)
        {
            return Play(self, from, path, animator, DefaultScheduler);
        }

        public static IObservable<Vector4> Play(this IObservable<Vector4> self, Vector4 from, Vector4[] path, IAnimator animator, IScheduler scheduler)
        {
            var merged = new Vector4[path.Length + 1];
            merged[0] = from;
            Array.Copy(path, 0, merged, 1, path.Length);
            return self.Concat(Play(merged, animator, scheduler));
        }

        public static IObservable<Vector4> Play(this IObservable<Vector4> self, Vector4 to, IAnimator animator)
        {
            return Play(self, to, animator, DefaultScheduler);
        }

        public static IObservable<Vector4> Play(this IObservable<Vector4> self, Vector4 to, IAnimator animator, IScheduler scheduler)
        {
            return self.Select(x => Play(x, to, animator, scheduler)).Switch();
        }

        public static IObservable<Vector4> Play(this IObservable<Vector4> self, Vector4[] path, IAnimator animator)
        {
            return Play(self, path, animator, DefaultScheduler);
        }

        public static IObservable<Vector4> Play(this IObservable<Vector4> self, Vector4[] path, IAnimator animator, IScheduler scheduler)
        {
            return self.Select(x =>
            {
                var merged = new Vector4[path.Length + 1];
                merged[0] = x;
                Array.Copy(path, 0, merged, 1, path.Length);
                return Play(merged, animator, scheduler);
            }).Switch();
        }

        public static IObservable<Vector4> PlayRelative(Vector4 from, Vector4 relative, IAnimator animator)
        {
            return PlayRelative(from, relative, animator, DefaultScheduler);
        }

        public static IObservable<Vector4> PlayRelative(Vector4 from, Vector4 relative, IAnimator animator, IScheduler scheduler)
        {
            return Play(from, from + relative, animator, scheduler);
        }

        public static IObservable<Vector4> PlayRelative(this IObservable<Vector4> self, Vector4 from, Vector4 relative, IAnimator animator)
        {
            return PlayRelative(self, from, relative, animator, DefaultScheduler);
        }

        public static IObservable<Vector4> PlayRelative(this IObservable<Vector4> self, Vector4 from, Vector4 relative, IAnimator animator, IScheduler scheduler)
        {
            return self.Concat(Play(from, from + relative, animator, scheduler));
        }
    }
}
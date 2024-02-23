#nullable enable

using System.Collections.Generic;
using UnityEngine;

namespace Svanesjo.MRIoT.Utility
{
    public static class VectorTools
    {
        public static Vector3 GetMeanVector(List<Vector3> arr)
        {
            if (arr.Count == 0)
                return Vector3.zero;

            var mean = Vector3.zero;
            foreach (var vector in arr)
                mean += vector;

            return mean / arr.Count;
        }
    }
}

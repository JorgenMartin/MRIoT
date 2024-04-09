#nullable enable

using UnityEngine;

namespace Svanesjo.MRIoT.Utility
{
    public static class QuaternionTools
    {
        public static Quaternion DifferenceFrom(this Quaternion to, Quaternion from)
        {
            return Quaternion.Inverse(from) * to;
        }
    }
}

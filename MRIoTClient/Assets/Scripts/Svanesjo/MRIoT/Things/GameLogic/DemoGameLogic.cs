using Exact;
using UnityEngine;

namespace Svanesjo.MRIoT.Things.GameLogic
{

    [RequireComponent(typeof(ExactManager))]
    public abstract class DemoGameLogic : MonoBehaviour
    {
        public abstract void OnTapped(Device device);
    }
}

using Exact;
using UnityEngine;

namespace Svanesjo.MRIoT.Things
{

    [RequireComponent(typeof(ExactManager))]
    public abstract class DemoGame : MonoBehaviour
    {
        public abstract void OnTapped(Device device);
    }
}

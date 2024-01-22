// Based on https://docs-multiplayer.unity3d.com/netcode/current/components/networktransform/#owner-authoritative-mode

using Unity.Netcode.Components;
using UnityEngine;

namespace Svanesjo.MRIoT.Networking
{
    [DisallowMultipleComponent]
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}

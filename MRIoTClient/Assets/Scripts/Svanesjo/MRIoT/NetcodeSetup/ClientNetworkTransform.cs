// Based on https://docs-multiplayer.unity3d.com/netcode/current/components/networktransform/#owner-authoritative-mode

#nullable enable

using Unity.Netcode.Components;
using UnityEngine;

namespace Svanesjo.MRIoT.NetcodeSetup
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

using System;
using UnityEngine;

namespace PRO
{
    [Flags]
    public enum GameLayer
    {
        Default = 1 << 0,

        Role = 1 << 8,
        Building = 1 << 9,

        Block = 1 << 11,


        Particle = 1 << 14,
        Particle_Block = 1 << 15,
        Particle_Block_Role = 1 << 16,
        Particle_Role = 1 << 17,

        NoPhysics = 1 << 31,
    }
    public static class GameLayerEx
    {
        public static int ToUnityLayer(this GameLayer layer) => (int)Math.Log((double)layer, 2.0);
    }
}

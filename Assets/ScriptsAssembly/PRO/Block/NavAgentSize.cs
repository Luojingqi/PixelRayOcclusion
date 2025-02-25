using UnityEngine;

namespace PRO
{
    public struct NavAgentMould
    {
        public Vector2Int size;
        public Vector2Int offset;
        public Vector2Int center => new Vector2Int(size.x / 2 - offset.x, (size.y + 1) / 2 - offset.y);
        public Vector2 centerW => new Vector2(size.x / 2 - offset.x, (size.y + 1) / 2 - offset.y) * Pixel.Size;
        public NavAgentMould(Vector2Int size, Vector2Int offset)
        {
            this.size = size;
            this.offset = offset;
        }
    }
}

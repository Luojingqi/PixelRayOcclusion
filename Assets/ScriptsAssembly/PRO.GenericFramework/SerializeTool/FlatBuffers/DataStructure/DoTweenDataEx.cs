using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Google.FlatBuffers;

namespace PRO.Flat.Ex
{
    public static class DoTweenDataEx
    {
        public static Offset<DoTweenData_Quaternion> ToDisk(this TweenerCore<Quaternion, Quaternion, NoOptions> tweener, FlatBufferBuilder builder)
        {
            var endValue = tweener.endValue;
            return DoTweenData_Quaternion.CreateDoTweenData_Quaternion(builder,
                endValue.X, endValue.Y, endValue.Z, endValue.W,
                tweener.Duration() - tweener.fullPosition);
        }
        public static Offset<DoTweenData_Vector3> ToDisk(this TweenerCore<Vector3, Vector3, VectorOptions> tweener, FlatBufferBuilder builder)
        {
            var endValue = tweener.endValue;
            return DoTweenData_Vector3.CreateDoTweenData_Vector3(builder,
                endValue.X, endValue.Y, endValue.Z,
                tweener.Duration() - tweener.fullPosition);
        }
        public static TweenerCore<UnityEngine.Quaternion, UnityEngine.Quaternion, NoOptions> LocalRotateQuaternionToRAM(this UnityEngine.Transform transform, DoTweenData_Quaternion diskData)
        {
            return transform.DOLocalRotateQuaternion(diskData.EndValue.ToRAM(), diskData.EndTime);
        }
        public static TweenerCore<UnityEngine.Vector3, UnityEngine.Vector3, VectorOptions> LocalMoveToRAM(this UnityEngine.Transform transform, DoTweenData_Vector3 diskData)
        {
            return transform.DOLocalMove(diskData.EndValue.ToRAM(), diskData.EndTime);
        }
    }
}

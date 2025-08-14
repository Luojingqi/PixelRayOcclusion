using NodeCanvas.Framework;
using ParadoxNotion.Animation;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization.FullSerializer;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{

    [Name("Ease Tween")]
    [Category("Movement/Direct")]
    public class EaseTransformTween : ActionTask<Transform>
    {

        public enum TransformMode
        {
            Position,
            Rotation,
            Scale
        }

        public enum TweenMode
        {
            Absolute,
            Additive
        }

        public enum PlayMode
        {
            Normal,
            PingPong
        }

        public TransformMode transformMode;
        public TweenMode mode;
        public PlayMode playMode;
        [fsSerializeAs("targetPosition")]
        public BBParameter<Vector3> targetVector;
        public EaseType easeType = EaseType.QuadraticInOut;
        public BBParameter<float> time = 0.5f;

        private Vector3 original;
        private Vector3 final;
        private bool ponging = false;

        protected override void OnExecute() {

            if ( ponging )
                final = original;

            if ( transformMode == TransformMode.Position )
                original = agent.localPosition;
            if ( transformMode == TransformMode.Rotation )
                original = agent.localEulerAngles;
            if ( transformMode == TransformMode.Scale )
                original = agent.localScale;

            if ( !ponging )
                final = targetVector.value + ( mode == TweenMode.Additive ? original : Vector3.zero );

            ponging = playMode == PlayMode.PingPong;

            if ( ( original - final ).magnitude < 0.1f )
                EndAction();
        }

        protected override void OnUpdate() {

            var value = Easing.Ease(easeType, original, final, elapsedTime / time.value);

            if ( transformMode == TransformMode.Position )
                agent.localPosition = value;
            if ( transformMode == TransformMode.Rotation )
                agent.localEulerAngles = value;
            if ( transformMode == TransformMode.Scale )
                agent.localScale = value;

            if ( elapsedTime >= time.value )
                EndAction();
        }
    }
}
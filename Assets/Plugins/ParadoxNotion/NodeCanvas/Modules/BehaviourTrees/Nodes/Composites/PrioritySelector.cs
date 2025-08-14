using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization.FullSerializer;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees
{

    ///----------------------------------------------------------------------------------------------
    class PrioritySelector_0 : BTComposite
    {
        [SerializeField] public List<BBParameter<float>> priorities = null;
    }
    ///----------------------------------------------------------------------------------------------

    [Category("Composites")]
    [Description("Used for Utility AI, the Priority Selector executes the child with the highest utility weight. If it fails, the Priority Selector will continue with the next highest utility weight child until one Succeeds, or until all Fail (similar to how a normal Selector does).\n\nEach child branch represents a desire, where each desire has one or more considerations which are combined together.\nConsiderations are a pair of input value and curve, which together produce the consideration utility weight.\n\nIf Dynamic option is enabled, will continously evaluate utility weights and execute the child with the highest one regardless of what status it return.")]
    [ParadoxNotion.Design.Icon("Priority")]
    [Color("b3ff7f")]
    [fsMigrateVersions(typeof(PrioritySelector_0))]
    public class PrioritySelector : BTComposite, IMigratable<PrioritySelector_0>
    {
        ///----------------------------------------------------------------------------------------------
        void IMigratable<PrioritySelector_0>.Migrate(PrioritySelector_0 model) {
            this.desires = new List<Desire>();
            foreach ( var priority in model.priorities ) {
                var desire = new Desire();
                this.desires.Add(desire);
                var consideration = desire.AddConsideration(graphBlackboard);
                consideration.input = priority;
            }
        }
        ///----------------------------------------------------------------------------------------------

        //The operation for combining the consideration utilities
        public enum ConsiderationsOperation
        {
            Average,
            Min,
            Max,
        }

        [System.Serializable]
        public class Desire
        {
            [fsIgnoreInBuild]
            public string name;
            [fsIgnoreInBuild]
            public bool foldout;
            public ConsiderationsOperation considerationsOperation;
            public List<Consideration> considerations = new();

            public Consideration AddConsideration(IBlackboard bb) {
                var result = new Consideration(bb);
                considerations.Add(result);
                return result;
            }

            public void RemoveConsideration(Consideration consideration) { considerations.Remove(consideration); }

            public float GetCompoundUtility() {
                if ( considerationsOperation == ConsiderationsOperation.Average ) {
                    float total = 0;
                    for ( var i = 0; i < considerations.Count; i++ ) {
                        total += considerations[i].utility;
                    }
                    return total / considerations.Count;
                }
                if ( considerationsOperation == ConsiderationsOperation.Max ) {
                    float result = float.NegativeInfinity;
                    for ( var i = 0; i < considerations.Count; i++ ) {
                        result = Mathf.Max(result, considerations[i].utility);
                    }
                    return result;
                }
                if ( considerationsOperation == ConsiderationsOperation.Min ) {
                    float result = float.PositiveInfinity;
                    for ( var i = 0; i < considerations.Count; i++ ) {
                        result = Mathf.Min(result, considerations[i].utility);
                    }
                    return result;
                }
                return 0;
            }
        }

        [System.Serializable]
        public class Consideration
        {
            public BBParameter<float> input;
            public BBParameter<AnimationCurve> function;
            public BBParameter<float> inputMin;
            public BBParameter<float> inputMax;

            public float utility {
                get
                {
                    var curve = function.value;
                    if ( curve == null ) { return input.value; }
                    if ( curve.keys?.LastOrDefault().time > 1 ) { //this is only done for backwards compatibility
                        return curve.Evaluate(input.value);
                    }
                    return curve.Evaluate(Mathf.InverseLerp(inputMin.value, inputMax.value, input.value));
                }
            }
            public Consideration() {
                input = 1f;
                function = null;
                inputMin = 0f;
                inputMax = 1f;
            }

            public Consideration(IBlackboard blackboard) {
                input = new BBParameter<float> { value = 1f, bb = blackboard };
                function = new BBParameter<AnimationCurve> { bb = blackboard };
                inputMin = new BBParameter<float> { value = 0f, bb = blackboard };
                inputMax = new BBParameter<float> { value = 1f, bb = blackboard };
            }
        }

        ///----------------------------------------------------------------------------------------------

        [Tooltip("If enabled, will continously evaluate utility weights and immediately execute the child with the highest one. In this mode child return status does not matter.")]
        public bool dynamic;
        [AutoSortWithChildrenConnections]
        public List<Desire> desires;

        private Connection[] orderedConnections;
        private int current = 0;

        public override void OnChildConnected(int index) {
            if ( desires == null ) { desires = new List<Desire>(); }
            if ( desires.Count < outConnections.Count ) { desires.Insert(index, new Desire()); }
        }

        public override void OnChildDisconnected(int index) { desires.RemoveAt(index); }

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {

            if ( dynamic ) {
                var highestPriority = float.NegativeInfinity;
                var best = 0;
                for ( var i = 0; i < outConnections.Count; i++ ) {
                    var priority = desires[i].GetCompoundUtility();
                    if ( priority > highestPriority ) {
                        highestPriority = priority;
                        best = i;
                    }
                }

                if ( best != current ) {
                    outConnections[current].Reset();
                    current = best;
                }
                return outConnections[current].Execute(agent, blackboard);
            }

            ///----------------------------------------------------------------------------------------------

            if ( status == Status.Resting ) {
                orderedConnections = outConnections.OrderBy(c => desires[outConnections.IndexOf(c)].GetCompoundUtility()).ToArray();
            }

            for ( var i = orderedConnections.Length; i-- > 0; ) {
                status = orderedConnections[i].Execute(agent, blackboard);
                if ( status == Status.Success ) {
                    return Status.Success;
                }

                if ( status == Status.Running ) {
                    current = i;
                    return Status.Running;
                }
            }

            return Status.Failure;
        }

        protected override void OnReset() { current = 0; }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------

#if UNITY_EDITOR

        //..
        public override string GetConnectionInfo(int i) {
            var desire = desires[i];
            var desireName = string.IsNullOrEmpty(desire.name) ? "DESIRE " + i.ToString() : desire.name;
            var result = "<size=12><b>" + desireName.ToUpper() + "</b></size>\n";
            for ( var j = 0; j < desire.considerations.Count; j++ ) {
                result += desire.considerations[j].input.ToString() + " (" + desire.considerations[j].utility.ToString("0.00") + ")" + "\n";
            }
            return result += string.Format("<b>{0}</b> ({1})", desire.considerationsOperation, desire.GetCompoundUtility().ToString("0.00"));
        }

        //..
        public override void OnConnectionInspectorGUI(int i) {
            var desire = desires[i];

            desire.name = UnityEditor.EditorGUILayout.TextField("Friendly Name", desire.name);
            if ( GUILayout.Button("Add Consideration") ) { desire.AddConsideration(graphBlackboard); }

            var optionsB = new EditorUtils.ReorderableListOptions();
            optionsB.allowAdd = false;
            optionsB.allowRemove = true;
            EditorUtils.ReorderableList(desire.considerations, optionsB, (j, pickedB) =>
            {
                var consideration = desire.considerations[j];
                GUILayout.BeginVertical("box");
                consideration.input = (BBParameter<float>)Editor.BBParameterEditor.ParameterField(EditorUtils.GetTempContent("Input", null, "The input value of the consideration"), consideration.input, true);
                consideration.function = (BBParameter<AnimationCurve>)Editor.BBParameterEditor.ParameterField(EditorUtils.GetTempContent("Cuve", null, "The utility function curve. It will be evaluated by the input value after input is adjusted to a 0 to 1 range by min and max below. Curve should also be in a 0 to 1 range."), consideration.function);
                if ( consideration.function.value != null ) {
                    var curveInRange = consideration.function.value.keys.LastOrDefault().time <= 1 && consideration.function.value.keys.FirstOrDefault().time >= 0;
                    if ( !curveInRange ) {
                        UnityEditor.EditorGUILayout.HelpBox("Curve should ideally be in a range of 0 to 1 on x axis (time), and use the input min and max parameters below to map the input on the curve instead.\nThe input min and max parameters are currently ignored.", UnityEditor.MessageType.Info);
                    }
                    GUI.enabled = curveInRange;
                    consideration.inputMin = (BBParameter<float>)Editor.BBParameterEditor.ParameterField(EditorUtils.GetTempContent("Input Min", null, "The expected input min value"), consideration.inputMin);
                    consideration.inputMax = (BBParameter<float>)Editor.BBParameterEditor.ParameterField(EditorUtils.GetTempContent("Input Max", null, "The expected input max value"), consideration.inputMax);
                    GUI.enabled = true;
                }
                GUILayout.EndVertical();
            });

            if ( desire.considerations.Count > 1 ) {
                desire.considerationsOperation = (ConsiderationsOperation)UnityEditor.EditorGUILayout.EnumPopup(string.Empty, desire.considerationsOperation);
            }

            EditorUtils.Separator();
        }

        protected override void OnNodeGUI() {
            if ( dynamic ) { GUILayout.Label("<b>DYNAMIC</b>"); }
        }

        //..
        protected override void OnNodeInspectorGUI() {

            if ( outConnections.Count == 0 ) {
                GUILayout.Label("Make some connections first");
                return;
            }

            dynamic = UnityEditor.EditorGUILayout.Toggle(new GUIContent("Dynamic", "If enabled, will continously evaluate utility weights and execute the child with the highest one accordingly. In this mode child return status does not matter."), dynamic);

            EditorUtils.Separator();
            EditorUtils.CoolLabel("Desires");

            var optionsA = new EditorUtils.ReorderableListOptions();
            optionsA.allowAdd = false;
            optionsA.allowRemove = false;
            EditorUtils.ReorderableList(desires, optionsA, (i, pickedA) =>
            {
                var desire = desires[i];
                var desireName = string.IsNullOrEmpty(desire.name) ? "DESIRE " + i.ToString() : desire.name;
                desire.foldout = UnityEditor.EditorGUILayout.Foldout(desire.foldout, new GUIContent(desireName));
                if ( desire.foldout ) {
                    OnConnectionInspectorGUI(i);
                }
            });
        }
#endif

    }
}
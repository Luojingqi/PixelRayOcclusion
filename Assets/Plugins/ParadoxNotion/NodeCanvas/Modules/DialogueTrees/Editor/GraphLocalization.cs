#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using NodeCanvas.Framework;
using ParadoxNotion.Serialization;
using System.Collections.Generic;
using NodeCanvas.DialogueTrees;
using ParadoxNotion.Design;

namespace NodeCanvas.Editor
{

    public class GraphLocalization : EditorWindow
    {

        private bool willRepaint;
        private Vector2 scrollPos;
        private List<Statement> statements;
        private Locales currentLocale;

        private Graph targetGraph => GraphEditor.currentGraph;

        ///----------------------------------------------------------------------------------------------

        //...
        public static void ShowWindow() {
            var editor = GetWindow<GraphLocalization>();
            editor.GatherStatements();
            editor.Show();
        }

        //...
        void OnEnable() {
            titleContent = new GUIContent("Localization Editor", StyleSheet.canvasIcon);

            minSize = new Vector2(750, 200);
            wantsMouseMove = true;
            wantsMouseEnterLeaveWindow = true;
            Graph.onGraphSerialized -= OnGraphSerialized;
            Graph.onGraphSerialized += OnGraphSerialized;
            GraphEditor.onCurrentGraphChanged -= OnGraphChanged;
            GraphEditor.onCurrentGraphChanged += OnGraphChanged;

            GatherStatements();

            willRepaint = true;
        }

        //...
        void OnDisable() {
            Graph.onGraphSerialized -= OnGraphSerialized;
            GraphEditor.onCurrentGraphChanged -= OnGraphChanged;
        }

        //...
        void OnGraphChanged(Graph graph) {
            GatherStatements();
        }

        //...
        void OnGraphSerialized(Graph graph) {
            if ( graph == targetGraph ) {
                GatherStatements();
                willRepaint = true;
            }
        }

        //...
        void GatherStatements() {

            statements = new List<Statement>();

            if ( targetGraph == null ) { return; }

            JSONSerializer.SerializeAndExecuteNoCycles(typeof(NodeCanvas.Framework.Internal.GraphSource), targetGraph.GetGraphSource(), (o, d) =>
            {
                if ( o is Statement statement ) {
                    statements.Add(statement);
                }
            });
        }


        ///----------------------------------------------------------------------------------------------

        //...
        void OnGUI() {
            EditorGUILayout.HelpBox("1) Select the language you wish to view and edit.\n2) If there is no data for the specified language, hit the Append Language button to add the currently selected language.\n3) If you wish to remove the currently selected language, hit the Remove Language.\nNote: The Default language is the one used by default in the graph editor.", MessageType.Info);
            if ( targetGraph == null ) { return; }

            GUILayout.BeginHorizontal();
            currentLocale = (Locales)UnityEditor.EditorGUILayout.EnumPopup(currentLocale);
            GUI.enabled = currentLocale != Locales.Default;
            if ( GUILayout.Button("Append Language", GUILayout.Width(150)) ) {
                if ( currentLocale != Locales.Default ) {
                    foreach ( var statement in statements ) {
                        if ( statement.localizations == null ) statement.localizations = new Dictionary<Locales, Statement.Localization>();
                        if ( !statement.localizations.ContainsKey(currentLocale) ) {
                            statement.localizations.Add(currentLocale, new Statement.Localization());
                        }

                    }
                }
            }

            if ( GUILayout.Button("Remove Language", GUILayout.Width(150)) ) {
                if ( currentLocale != Locales.Default ) {
                    foreach ( var statement in statements ) {
                        if ( statement.localizations != null ) {
                            statement.localizations.Remove(currentLocale);
                            if ( statement.localizations.Count == 0 ) {
                                statement.localizations = null;
                            }
                        }
                    }
                }
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            EditorUtils.BoldSeparator();

            if ( statements == null || statements.Count == 0 ) {
                ShowNotification(new GUIContent("The graph has no localizable statements"));
                return;
            }

            GUILayout.BeginHorizontal();
            EditorUtils.CoolLabel("Default");
            if ( currentLocale != Locales.Default ) {
                EditorUtils.CoolLabel(currentLocale.ToString());
            }
            GUILayout.EndHorizontal();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

            foreach ( var statement in statements ) {
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);

                GUILayout.BeginVertical();
                statement.text = UnityEditor.EditorGUILayout.TextArea(statement.text, Styles.wrapTextArea, GUILayout.Width(350), GUILayout.ExpandWidth(true), GUILayout.Height(50));
                statement.audio = UnityEditor.EditorGUILayout.ObjectField(statement.audio, typeof(AudioClip), false, GUILayout.Width(350), GUILayout.ExpandWidth(true)) as AudioClip;
                GUILayout.EndVertical();

                GUILayout.Space(20);

                if ( currentLocale != Locales.Default ) {
                    if ( statement.localizations != null ) {
                        if ( statement.localizations.TryGetValue(currentLocale, out Statement.Localization localization) ) {
                            GUILayout.BeginVertical();
                            localization.text = UnityEditor.EditorGUILayout.TextArea(localization.text, Styles.wrapTextArea, GUILayout.Width(350), GUILayout.ExpandWidth(true), GUILayout.Height(50));
                            localization.audio = UnityEditor.EditorGUILayout.ObjectField(localization.audio, typeof(AudioClip), false, GUILayout.Width(350), GUILayout.ExpandWidth(true)) as AudioClip;
                            GUILayout.EndVertical();
                        } else {
                            GUILayout.Label("No " + currentLocale + " localization exists for this statement.\nPress Append Language to add it.", GUILayout.Width(350), GUILayout.ExpandWidth(true));
                        }
                    } else {
                        GUILayout.Label("No " + currentLocale + " localization exists for this statement.\nPress Append Language to add it.", GUILayout.Width(350), GUILayout.ExpandWidth(true));
                    }
                }

                GUILayout.Space(10);
                GUILayout.EndHorizontal();
                EditorUtils.Separator();
            }

            EditorGUILayout.EndScrollView();

            if ( willRepaint ) {
                Repaint();
            }

            if ( GUI.changed ) {
                targetGraph.SelfSerialize();
                UndoUtility.SetDirty(targetGraph);
            }
        }
    }
}

#endif
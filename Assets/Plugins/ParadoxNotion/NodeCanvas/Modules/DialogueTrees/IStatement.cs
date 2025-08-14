using ParadoxNotion;
using NodeCanvas.Framework;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace NodeCanvas.DialogueTrees
{

    ///<summary>An interface to use for what is being said by a dialogue actor</summary>
    public interface IStatement
    {
        string text { get; }
        AudioClip audio { get; }
        string meta { get; }

        string GetLocalizedText(Locales lang);
        AudioClip GetLocalizedAudio(Locales lang);
    }

    ///<summary>Holds data of what's being said by a dialogue actor</summary>
    [System.Serializable]
    public class Statement : IStatement
    {
        ///<summary>A statement localization</summary>
        [System.Serializable]
        public class Localization
        {
            public string text;
            public AudioClip audio;
        }

        [SerializeField] private string _text = string.Empty;
        [SerializeField] private AudioClip _audio;
        [SerializeField] private string _meta = string.Empty;
        [SerializeField] private Dictionary<Locales, Localization> _localizations;

        public string text {
            get => _text;
            set => _text = value;
        }

        public AudioClip audio {
            get => _audio;
            set => _audio = value;
        }

        public string meta {
            get => _meta;
            set => _meta = value;
        }

        public Dictionary<Locales, Localization> localizations {
            get => _localizations;
            set => _localizations = value;
        }

        //required
        public Statement() { }
        public Statement(string text) {
            this.text = text;
        }

        public Statement(string text, AudioClip audio) {
            this.text = text;
            this.audio = audio;
        }

        public Statement(string text, AudioClip audio, string meta) {
            this.text = text;
            this.audio = audio;
            this.meta = meta;
        }

        ///<summary>Returns the localized text for provided language. If it doesn't exist, returns the default text.</summary>
        public string GetLocalizedText(Locales lang) {
            if ( lang != Locales.Default && localizations.TryGetValue(lang, out Localization loc) ) {
                return loc.text;
            }
            return text;
        }

        ///<summary>Returns the localized AudioClip for provided language. If it doesn't exist, returns the default AudioClip.</summary>
        public AudioClip GetLocalizedAudio(Locales lang) {
            if ( lang != Locales.Default && localizations.TryGetValue(lang, out Localization loc) ) {
                return loc.audio;
            }
            return audio;
        }

        ///<summary>Replace the text of the statement found in brackets, with blackboard variables and actor parameters names and returns a Statement copy</summary>
        public IStatement ProcessStatementBrackets(IBlackboard bb, DialogueTree dlg) {
            var copy = ParadoxNotion.Serialization.JSONSerializer.Clone<Statement>(this);

            copy.text = copy.text.ReplaceWithin('[', ']', (input) =>
            {
                if ( bb != null ) { //referenced blackboard replace
                    var variable = bb.GetVariable(input, typeof(object));
                    if ( variable != null ) { return variable.value.ToString(); }
                }

                if ( input.Contains("/") ) { //global blackboard replace
                    var globalBB = GlobalBlackboard.Find(input.Split('/').First());
                    if ( globalBB != null ) {
                        var variable = globalBB.GetVariable(input.Split('/').Last(), typeof(object));
                        if ( variable != null ) { return variable.value.ToString(); }
                    }
                }

                if ( dlg != null ) {
                    var actor = dlg.GetActorReferenceByName(input);
                    return actor.name;
                }

                return input;
            });

            return copy;
        }

        public override string ToString() {
            return text;
        }
    }
}
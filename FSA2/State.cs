using System;
using System.Collections.Generic;
using System.Linq;

namespace FSA
{
    [Serializable]
    public class State : IEquatable<State>
    {
        //private List<Transition> _transitions;
        //private static List<Transition> noneTransitions = new List<Transition>();

        private Transition[] _transitions;
        private static Transition[] noneTransitions = new Transition[0];

        public bool Accept { get; set; }

        [NonSerialized]
        private int _number;
        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }

        

        public Transition[] Transitions
        {
            get { return _transitions; }
        }

        public State()
        {
            _transitions = noneTransitions;
        }

        public void AddTransition(Transition t)
        {
            if (_transitions.Length == 0)
            {
                _transitions = new Transition[1];
            }
            else
            {
                Array.Resize(ref _transitions,_transitions.Length+1);
            }
            _transitions[_transitions.Length - 1] = t;
        }

        public State Step(char c)
        {
            State state = null;
            for (int i = 0; i < _transitions.Length; i++)
            {
                var tr = _transitions[i];
                if (tr.Value == c)
                {
                    state = tr.To;
                }
            }
            return state;
        }

        
        public bool Equals(State other)
        {
            if (other == null)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            var otherTr = other.Transitions;
            if (other.Accept != Accept || _transitions.Length != otherTr.Length)
            {
                return false;
            }
            for (int i = 0; i < _transitions.Length; i++)
            {
                if ((_transitions[i].Value != otherTr[i].Value) || _transitions[i].To != otherTr[i].To)
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            int hash = Accept ? 1 : 0;
            hash ^= (hash * 31) + _transitions.Length;
            for (int i = 0; i < _transitions.Length; i++)
            {
                var tr = _transitions[i];
                hash ^= hash*31 + tr.Value;
                hash ^= System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(tr.To);
            }
            return hash;
        }

        public bool HasChildren
        {
            get { return _transitions.Length > 0; }
        }
    }
}

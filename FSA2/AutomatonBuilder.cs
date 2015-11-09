using System.Collections.Generic;

namespace FSA
{
    public class AutomatonBuilder
    {
        private State _root = new State();
        private Dictionary<State,State> _register = new Dictionary<State, State>();
        public static Automaton Build(IEnumerable<string> strings)
        {
            AutomatonBuilder builder = new AutomatonBuilder();
            foreach (var s in strings)
            {
                builder.Add(s.ToCharArray());
            }
            Automaton automaton = new Automaton
            {
                Initial = builder.Complete()
            };
            return automaton;
        }

        private State Complete()
        {
            if (_root.HasChildren)
            {
                ReplaceOrRegister(_root);
            }
            _register = null;
            return _root;
        }

        private void Add(char[] current)
        {
            int pos = 0;
            int max = current.Length;
            State next;
            State state = _root;
            while (pos < max && (next = LastChild(state,current[pos])) != null)
            {
                state = next;
                pos++;
            }

            if (state.HasChildren)
            {
                ReplaceOrRegister(state);
            }
            AddSuffix(state, current, pos);
        }

        private void ReplaceOrRegister(State state)
        {
            var child = state.Transitions[state.Transitions.Length - 1].To;
            if (child.HasChildren)
            {
                ReplaceOrRegister(child);
            }
            State reg;
            if (_register.TryGetValue(child, out reg))
            {
                state.Transitions[state.Transitions.Length - 1].To = reg;
            }
            else
            {
                _register.Add(child,child);
            }
        }

        private static void AddSuffix(State state, char[] current, int fromIndex)
        {
            for (int i = fromIndex; i < current.Length; i++)
            {
                state = GetNewState(state, current[i]);
            }
            state.Accept = true;
        }

        private static State GetNewState(State state, char c)
        {
            State nState = new State();
            state.AddTransition(new Transition(c,nState));
            return nState;
        }

        private static State LastChild(State state,char label)
        {
            State s = null;
            int index = state.Transitions.Length - 1;
            if (index >= 0 && state.Transitions[index].Value==label)
            {
                s = state.Transitions[index].To;
            }
            return s;
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace FSA
{
    public class Automaton
    {
        public State Initial { get; set; }

        public bool Contains(string s)
        {
            State p = Initial;
            for (int i = 0; i < s.Length; i++)
            {
                State q = p.Step(s[i]);
                if (q==null)
                {
                    return false;
                }
                p = q;
            }
            return p.Accept;
        }

        public void Store(Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream,Initial);
        }

        public void Load(Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Initial = (State) formatter.Deserialize(stream);
        }

        public IList<State> GetNumberedStates()
        {
            HashSet<State> visited = new HashSet<State>();
            Queue<State> worklist = new Queue<State>(4);
            List<State> numberedStates = new List<State>(4);
            int upto = 0;
            Initial.Number = upto++;
            worklist.Enqueue(Initial);
            visited.Add(Initial);
            numberedStates.Add(Initial);
            
            while (worklist.Count > 0)
            {
                var s = worklist.Dequeue();
                foreach (var t in s.Transitions)
                {
                    if (!visited.Contains(t.To))
                    {
                        t.To.Number = upto++;
                        visited.Add(t.To);
                        worklist.Enqueue(t.To);
                        numberedStates.Add(t.To);
                    }
                }
            }
            return numberedStates;
        }

        public string ToDot()
        {
            StringBuilder b = new StringBuilder("digraph Automaton {\n");
            b.Append("  rankdir = LR;\n");
            var states = GetNumberedStates();
            foreach (var s in states)
            {
                b.Append("  ").Append(s.Number);
                if (s.Accept)
                {
                    b.Append(" [shape=doublecircle,label=\"\"];\n");
                }
                else
                {
                    b.Append(" [shape=circle,label=\"\"];\n");
                }
                if (s==Initial)
                {
                    b.Append("  initial [shape=plaintext,label=\"\"];\n");
                    b.Append("  initial -> ").Append(s.Number).Append("\n");
                }
                foreach (var t in s.Transitions)
                {
                    b.Append("  ").Append(s.Number);
                    t.AppendDot(b);
                }
            }
            return b.Append("}\n").ToString();
        }
    }
}
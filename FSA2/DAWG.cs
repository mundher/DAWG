using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSA
{
    public class DAWG
    {
        private Dictionary<char,int> _charMap = new Dictionary<char, int>(64);
        private Node[] _nodes;
        private const int EOW = 1073741824;
        private const int EOL = -2147483648;
        private const int CHILD_MASK = 0x3FFFFFFF;
        public DAWG(State initial)
        {
            Dictionary<State, int> dict;
            Queue<State> q;
            int lastChildPos;
            var nodes = Init(initial.Transitions, out dict, out q, out lastChildPos);
            //var lastChildPos = nodes.Count;
            while (q.Count > 0)
            {
                var st = q.Dequeue();
                foreach (var tr in st.Transitions)
                {
                    var to = tr.To;
                    int childPos=0;
                    if (to.HasChildren && !dict.TryGetValue(to, out childPos))
                    {
                        childPos = lastChildPos;
                        lastChildPos += to.Transitions.Count();
                        dict.Add(to,childPos);
                        q.Enqueue(to);
                    }
                    nodes.Add(new Node(tr.Value, childPos, to.Accept));
                }
                var len = nodes.Count - 1;
                var last = nodes[len];
                last.Info |= EOL;
                nodes[len] = last;
            }
            _nodes = nodes.ToArray();
        }

        public bool Contains(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return false;
            }
            int idx;
            if (!_charMap.TryGetValue(s[0],out idx))
            {
                return false;
            }
            var node = _nodes[Search(s, 0, idx)];
            if ((node.Eow) != 0)
            {
                //Console.WriteLine("Word Found");
                return true;
            }
            return false;
        }

        private int Search(string s, int pos, int index)
        {
            int currentIndex = index;
            var c = s[pos];
            //Console.WriteLine("---------------------------");
            //Console.WriteLine("seek {0} in position {1}",c,pos);
            while (currentIndex != 0)
            {
                var node = _nodes[currentIndex];
                if (c > node.Value)
                {
                    //Console.WriteLine("Node {0} Letter {1} - Letter too small", currentIndex, node.Value);
                    currentIndex = (node.EoL) == 0 ? currentIndex + 1 : 0;
                }
                else if(c < node.Value)
                {
                    //Console.WriteLine("Node {0} Letter {1} - Letter too big", currentIndex, node.Value);
                    return 0;
                }
                else if(s.Length == pos+1)
                {
                    return currentIndex;
                    //Console.WriteLine("Node {0} Letter {1} = Letter match", currentIndex, node.Value);
                    
                    //Console.WriteLine("Word Not Found");
                    
                }
                else
                {
                    //Console.WriteLine("Node {0} Letter {1} - Letter match", currentIndex, node.Value);
                    return Search(s, pos + 1, _nodes[currentIndex].Child);
                }
            }
            return 0;
        }

        public List<string> StartWith(string s,int max)
        {
            var lst = new List<string>(max);
            int idx;
            if (!_charMap.TryGetValue(s[0], out idx))
            {
                return lst;
            }
            var currentIndex = Search(s, 0, idx);
            var node = _nodes[currentIndex];
            if (node.Info == 0)
            {
                return lst;
            }
            StringBuilder sb = new StringBuilder(s);
            if (node.Eow !=0)
            {
                lst.Add(s);
            }
            
            
            Collect(node.Child, max, lst, sb);
            return lst;
        }

        private void Collect(int currentIndex, int max, List<string> lst, StringBuilder sb)
        {
            while (currentIndex !=0 && lst.Count < max)
            {
                var node = _nodes[currentIndex++];
                sb.Append(node.Value);
                if (node.Eow != 0)
                {
                    lst.Add(sb.ToString());
                }
                Collect(node.Child,max,lst,sb);
                sb.Remove(sb.Length - 1, 1);
                if (node.EoL != 0)
                {
                    return;
                }
            }
        }

        private List<Node> Init(Transition[] trans, out Dictionary<State, int> dict, out Queue<State> q, out int lastChildPos)
        {
            var len = trans.Length;
            dict = new Dictionary<State, int>(len);
            q = new Queue<State>(len);
            var nodes = new List<Node>(len + 1);
            nodes.Add(new Node((char)0,0));
            lastChildPos = len+1;
            for (int i = 0; i < len; i++)
            {
                var tr = trans[i];
                _charMap.Add(tr.Value,i+1);
                var to = tr.To;
                int childPos=0;
                if (to.HasChildren && !dict.TryGetValue(to,out childPos))
                {
                    childPos = lastChildPos;
                    lastChildPos += to.Transitions.Length;
                    dict.Add(to,childPos);
                    q.Enqueue(to);
                }
                nodes.Add(new Node(tr.Value,childPos,to.Accept));
            }
            var last = nodes[len];
            last.Info |= EOL;
            nodes[len] = last;
            return nodes;
        }

        
        public struct Node
        {
            public char Value;
            public int Info;

            public Node(char c, int info)
            {
                Value = c;
                Info = info;
            }

            public Node(char c, int info, bool eow)
            {
                Value = c;
                Info = info | ((eow) ? EOW : 0);
            }

            public void ToEol()
            {
                Info |= EOL;
            }

            public int Eow { get { return Info & EOW; }}

            public int EoL { get { return Info & EOL; } }

            public int Child { get { return Info & CHILD_MASK; } }

            public override string ToString()
            {
                var str = string.Format("{0} , {1}", Value, Info & CHILD_MASK);
                if ((Info& EOW)!=0)
                {
                    str += " EOW";
                }
                if ((Info & EOL) != 0)
                {
                    str += " EOL";
                }
                return str;
            }
        }
    }

    
}
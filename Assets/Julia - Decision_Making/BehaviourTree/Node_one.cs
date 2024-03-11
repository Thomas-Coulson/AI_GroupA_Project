using System.Collections;
using System.Collections.Generic;


namespace BehaviourTree
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public class Node_one
    {
        protected NodeState state;

        public Node_one parent;
        protected List<Node_one> children = new List<Node_one>();

        private Dictionary<string, object> _dataContext = new Dictionary<string, object>();

        public Node_one()
        {
            parent = null;
        }
        public Node_one(List<Node_one> children)
        {
            foreach (Node_one child in children)
                _Attach(child);
        }
        private void _Attach(Node_one node)
        {
            node.parent = this;
            children.Add(node);
        }

        public virtual NodeState Evaluate() => NodeState.FAILURE;

        public void SetData(string key, object value)
        {
            _dataContext[key] = value;
        }
        public object GetData(string key)
        {
            object value = null;
            if (_dataContext.TryGetValue(key, out value))
                return value;

            Node_one node = parent;
            while (node != null)
            {
                value = node.GetData(key);
                if (value != null)
                    return value;
                node = node.parent;
            }
            return null;
        }

        public bool ClearData(string key)
        {
            if (_dataContext.ContainsKey(key))
            {
                _dataContext.Remove(key);
                return true;
            }

            Node_one node = parent;
            while (node != null)
            {
                bool cleared = node.ClearData(key);
                if (cleared)
                    return true;
                node = node.parent;
            }
            return false;
        }
    }
}



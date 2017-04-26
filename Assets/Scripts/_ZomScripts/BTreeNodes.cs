// BTree - base behavior tree nodes
// by Zomawia Sailo
// thanks to Esme for help

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTreeNodes : MonoBehaviour {

    public enum BTStatus { success, pending, failure};

    public interface IBTNode<T>
    {
        BTStatus execute(T agent);
    }

    public class BTCompNode<T> : IBTNode<T>
    {
        public List<IBTNode<T>> children = new List<IBTNode<T>>();
        public virtual BTStatus execute (T agent) { return BTStatus.success; }
    }

    public class Selector<T> : BTCompNode<T>
    {
        public override BTStatus execute(T agent)
        {
            foreach (var child in children)
                if (child.execute(agent) == BTStatus.success)
                    return BTStatus.success;
            return BTStatus.failure;
        }
    }

    public class Sequence<T> : BTCompNode<T>
    {
        public override BTStatus execute(T agent)
        {
            foreach (var child in children)
                if (child.execute(agent) == BTStatus.failure)
                    return BTStatus.failure;
            return BTStatus.success;
        }
    }       
}

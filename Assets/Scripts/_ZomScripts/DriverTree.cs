//UberDriverAI Behavior Tree
//by Zomawia Sailo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriverTree : MonoBehaviour {

    public static BTreeNodes.IBTNode<UberDriverAI> root;

    // guaranteed to be called before accessing a static member
    static DriverTree()
    {
        var rootSeq = new BTreeNodes.Sequence<UberDriverAI>();

        var Sequence = new BTreeNodes.Sequence<UberDriverAI>();
        var Selector = new BTreeNodes.Selector<UberDriverAI>();

        Selector.children.Add(new BTreeConditions.HasPassenger());
        Sequence.children.Add(new BTreeConditions.HasRequest());
        Sequence.children.Add(new BTreeConditions.DriveToRequest());
        Sequence.children.Add(new BTreeConditions.IsCloseToRequest());
        Sequence.children.Add(new BTreeConditions.PickupRequest());
        Selector.children.Add(Sequence);        

        var Sequence2 = new BTreeNodes.Sequence<UberDriverAI>();
        Sequence2.children.Add(new BTreeConditions.DriveToDestination());
        Sequence2.children.Add(new BTreeConditions.IsDriverAtDestination());
        Sequence2.children.Add(new BTreeConditions.RemovePassenger());      
        
        rootSeq.children.Add(Selector);
        rootSeq.children.Add(Sequence2);

        root = rootSeq;
    }

    public void RunTree(UberDriverAI agent)
    {
        root.execute(agent);
    }
}

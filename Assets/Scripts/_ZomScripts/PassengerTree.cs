//Passenger Behavior Tree
//by Zomawia Sailo
//and some help from Terry

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerTree : MonoBehaviour {

    public static BTreeNodes.IBTNode<_PedestrianAI> root;

    // guaranteed to be called before accessing a static member
    static PassengerTree()
    {
        var rootSeq = new BTreeNodes.Sequence<_PedestrianAI>();

        var Sequence = new BTreeNodes.Sequence<_PedestrianAI>();
        var Selector = new BTreeNodes.Selector<_PedestrianAI>();
        
        Selector.children.Add(new BTreeConditions.IsInCar());

        Sequence.children.Add(new BTreeConditions.RequestRide());
        Sequence.children.Add(new BTreeConditions.WaitForRide());
        Sequence.children.Add(new BTreeConditions.IsRideClose());
        Sequence.children.Add(new BTreeConditions.GetInTheCar());

        Selector.children.Add(Sequence);

        var Sequence2 = new BTreeNodes.Sequence<_PedestrianAI>();

        Sequence2.children.Add(new BTreeConditions.OutOfCar());
        Sequence2.children.Add(new BTreeConditions.IsDestinationClose());
        Sequence2.children.Add(new BTreeConditions.DoWalk());
        Sequence2.children.Add(new BTreeConditions.IsAtDestination());
        Sequence2.children.Add(new BTreeConditions.AssignNewDestination());

        rootSeq.children.Add(new BTreeConditions.IsTargetFar());
        rootSeq.children.Add(Selector);
        rootSeq.children.Add(Sequence2);

        root = rootSeq;

    }

    public void RunTree(_PedestrianAI agent)
    {
        root.execute(agent);
    }
}

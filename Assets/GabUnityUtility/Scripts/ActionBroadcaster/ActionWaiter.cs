using GabUnity;
using GabUnity.ActionBroadcaster;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ActionWaiter : MonoSingleton<ActionWaiter>
{
    public readonly List<(string[] qualifiers, Func<Transform, bool> callback)> waitlist = new();

    public static void Broadcast(string action, Transform where) => Broadcast(action, where, out _);
    public static void Broadcast(string action, Transform where, out bool received)
    {
       // Debug.Log("Attempting to resolve : " + action + (where != null ? " | FROM : " + where.gameObject.name : ""));

        bool removed_atleast_one = false;
        Instance.waitlist.RemoveAll(tuple =>
        {
            foreach (var qualifier in tuple.qualifiers)
                if (action.Contains(qualifier) == false)
                    return false;

            if (tuple.callback.Invoke(where))
            {
                removed_atleast_one = true;
                return true;
            }
            else
                return false;
        });

        if (removed_atleast_one)
            received = true;
        else
            received = false;
    }
    public static void RegisterWaiter(string action_name, Func<Transform, bool> waiter) => Instance.waitlist.Add((new string[]{action_name}, waiter));
    public static void RegisterNonStrictWaiter(string[] keywords, Func<Transform, bool> waiter) => Instance.waitlist.Add((keywords, waiter));
    public static bool CheckIfWaiting(string action_name) => Instance.waitlist.Any(tuple => tuple.qualifiers[0].Equals(action_name));
}

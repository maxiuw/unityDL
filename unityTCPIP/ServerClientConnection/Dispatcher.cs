using System.Collections;
using System.Collections.Generic;
using Core.Singletons;
using UnityEngine;
using System;
using TMPro;


public class Dispatcher : Singleton<Dispatcher>
{
    // queue that has a log, we constantly look at the queue
    // and if we get a message, we instantly dispatch it 
    // we use dispatcher to put the messages to the main queue 
    // Start is called before the first frame update
    private static readonly Queue<Action> actions = new Queue<Action>();
   
    // Update is called once per frame
    void Update()
    {
        // dequeuing the actions 
        // check if we have any actions in the queue, if yes, process the action and deque 
        while (actions.Count > 0)
            actions.Dequeue().Invoke(); // dequeue and execute 
    }

    public void Enqueue(IEnumerator action) {

        // locking to avioid multi threading 
        // A coroutine allows you to spread tasks across several frames
        lock(actions) {
            actions.Enqueue(() => { // () in this case coresponds to action 
                StartCoroutine(action); // add new actions to the queue 
            });
        }
    }

    // converting action to ienumnerator 
    public void Enqueue(Action action) => Enqueue(ActionWrapper(action)); // overwritting enqueue 
    
    // creating action wrapper
    IEnumerator ActionWrapper(Action action) {
        action();
        yield return null;
    }
}

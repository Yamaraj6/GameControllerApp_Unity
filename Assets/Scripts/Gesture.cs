using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IGesture
{
    bool CompareTo(HandPosition firstPosition, HandPosition lastPosition);
    IEnumerator StartAction();
}

public class StaticGesture : IGesture
{
    private HandPosition hand_position;
    private Action action;

    public StaticGesture(HandPosition handPosition, Action Action)
    {
        hand_position = handPosition;
        action = Action;
    }

    public bool CompareTo(HandPosition firstPosition, HandPosition lastPosition)
    {
        return (hand_position == firstPosition && hand_position == lastPosition);
    }

    public IEnumerator StartAction()
    {
        action.Invoke();
        yield return new WaitForSeconds(0);
    }
}

public class DynamicGesture : IGesture
{
    private static float BREAK_TIME = 1;

    private HandPosition first_position;
    private HandPosition last_position;
    private bool is_;
    private Action action;
    private Action<GameObject> creat_an_object;
    private GameObject object_to_creat;

    public DynamicGesture(HandPosition firstPosition, HandPosition lastPosition, Action Action)
    {
        first_position = firstPosition;
        last_position = lastPosition;
        action = Action;
        creat_an_object = null;
        object_to_creat = null;
        is_ = true;
    }

    public DynamicGesture(HandPosition firstPosition, HandPosition lastPosition, 
        Action<GameObject> createAnObject, GameObject objectToCreat)
    {
        action = null;
        first_position = firstPosition;
        last_position = lastPosition;
        creat_an_object = createAnObject;
        object_to_creat = objectToCreat;
        is_ = true;
    }

    public bool CompareTo(HandPosition firstPosition, HandPosition lastPosition)
    {
        return (first_position == firstPosition && last_position == lastPosition);
    }

    public IEnumerator StartAction()
    {
        if (is_)
        {
            if (creat_an_object == null)
            {
                is_ = false;
                action.Invoke();
                yield return new WaitForSeconds(BREAK_TIME);
                is_ = true;
            }
            else
            {
                is_ = false;
                creat_an_object.Invoke(object_to_creat);
                yield return new WaitForSeconds(BREAK_TIME);
                is_ = true;
            }
        }
    }
}

public class Gestures
{
    private DynamicGesture grab;
    private DynamicGesture throw_out;
    private DynamicGesture drop;
    private DynamicGesture push;

    private StaticGesture hold;
    private StaticGesture hold_fist;

    private DynamicGesture click;
    private DynamicGesture thumb_index_circle;

    public Gestures(GameObject spellToRespawn, GameObject gobToRespawn)
    {
        Features feature = new Features(GameObject.Find("HandController"));

        grab = new DynamicGesture(HandPosition.Open, HandPosition.Fist, feature.Grab);
        throw_out = new DynamicGesture(HandPosition.Fist, HandPosition.Open, feature.Throw);
        drop = new DynamicGesture(HandPosition.Fist, HandPosition.Hold, feature.Drop);
        push = new DynamicGesture(HandPosition.Fist, HandPosition.BentThumb, feature.Push);
        
        hold_fist = new StaticGesture(HandPosition.Fist, feature.Hold);
        
        click = new DynamicGesture(HandPosition.Open, HandPosition.BentThumb, feature.RespObject, gobToRespawn);
        thumb_index_circle = new DynamicGesture(HandPosition.Open, HandPosition.ThumbIndexCircle,
            feature.SpellCast, spellToRespawn);
    }

    public void RecognizeGesture(HandPosition firstPosition, HandPosition lastPosition, MonoBehaviour monoBehaviour)
    {
        if (grab.CompareTo(firstPosition, lastPosition))
        {
            monoBehaviour.StartCoroutine(grab.StartAction());
        }
        else if (throw_out.CompareTo(firstPosition, lastPosition))
        {
            monoBehaviour.StartCoroutine(throw_out.StartAction());
        }
        else if (drop.CompareTo(firstPosition, lastPosition))
        {
            monoBehaviour.StartCoroutine(drop.StartAction());
        }
        else if (push.CompareTo(firstPosition, lastPosition))
        {
            monoBehaviour.StartCoroutine(push.StartAction());
        }
        else if (thumb_index_circle.CompareTo(firstPosition, lastPosition))
        {
            monoBehaviour.StartCoroutine(thumb_index_circle.StartAction());
        }
        else if (click.CompareTo(firstPosition, lastPosition))
        {
            monoBehaviour.StartCoroutine(click.StartAction());
        }
        else if (hold_fist.CompareTo(firstPosition, lastPosition))
        {
            monoBehaviour.StartCoroutine(hold_fist.StartAction());
        }
    }
}
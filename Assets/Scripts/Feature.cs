using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Features
{
    GameObject hand;
    GameObject gob_on_ray_end;
    Transform trs_held_place;
    Transform trs_held_obj;

    public Features(GameObject Hand)
    {
        hand = Hand;
        gob_on_ray_end = GameObject.Find("EndRayObject");
        trs_held_place = GameObject.Find("HeldObject").transform;
        trs_held_obj = null;
    }

    public void Grab()
    {
        Transform _objectOnRaycast = GetObjectOnRaycast();
        if (_objectOnRaycast != null)
        {
            _objectOnRaycast.position = trs_held_place.position;
            trs_held_obj = _objectOnRaycast;
        }
    }

    public void Throw()
    {
        if (trs_held_obj != null)
        {
            trs_held_obj.GetComponent<Rigidbody>().velocity =
                      gob_on_ray_end.transform.position;
            trs_held_obj = null; 
        }
    }

    public void Push()
    {
        Transform _objectOnRaycast = GetObjectOnRaycast();
        if (_objectOnRaycast != null)
        {
            _objectOnRaycast.GetComponent<Rigidbody>().velocity =
                      gob_on_ray_end.transform.position;
        }
    }

    public void Hold()
    {
        if (trs_held_obj != null)
        {
            trs_held_obj.transform.position = trs_held_place.transform.position;
        }
    }

    public void Drop()
    {
        trs_held_obj = null;
    }

    private Transform GetObjectOnRaycast()
    {
        RaycastHit hit;
        Ray handRay = new Ray(hand.transform.position, -hand.transform.up);
        
        if (Physics.Raycast(handRay, out hit, HandController.RAYCAST_DISTANCE))
        {
            if (hit.collider.tag == "RigidBodyObject")
            {
                return hit.collider.transform;
            }
        }
        return null;
    }
    
    public void SpellCast(GameObject spell)
    {
        hand.GetComponent<MonoBehaviour>().StartCoroutine(ISpellCast(spell));
    }

    public void RespObject(GameObject gameObject)
    {
        hand.GetComponent<MonoBehaviour>().StartCoroutine(IRespObject(gameObject));
    }

    private IEnumerator ISpellCast(GameObject spell)
    {
        var _castedSpell = GameObject.Instantiate(spell);
        _castedSpell.transform.position = trs_held_place.transform.position;
        _castedSpell.transform.rotation = trs_held_place.transform.rotation;
        yield return new WaitWhile(() => _castedSpell.GetComponent<ParticleSystem>().IsAlive());
        GameObject.Destroy(_castedSpell);
    }

    private IEnumerator IRespObject(GameObject gameObject)
    {
        var _respGob = GameObject.Instantiate(gameObject);
        _respGob.transform.position = trs_held_place.transform.position;
        _respGob.transform.rotation = trs_held_place.transform.rotation;
        yield return new WaitForSeconds(1);
    }
}
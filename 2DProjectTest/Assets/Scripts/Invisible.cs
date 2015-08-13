using UnityEngine;
using System.Collections;

public class Invisible : MonoBehaviour, ICanReset
{
    public void reset()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.enabled = false;
        }
    }
}

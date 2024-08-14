using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Interactable_4 : Interactable
{
    // Empty
    public override void effect()
    {
        interactable_value += 1 * Time.deltaTime;
        if (interactable_value >= interactable_value_max)
        {
            decrease_exposure();
            //Reset
            interactable_value = 0;
        }
    }
}

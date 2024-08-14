using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_1 : Interactable
{
    //Inherited
    public override void effect()
    {
        //Increase exposure level
        level.increase_exposure_amount(25.0f);
    }
}

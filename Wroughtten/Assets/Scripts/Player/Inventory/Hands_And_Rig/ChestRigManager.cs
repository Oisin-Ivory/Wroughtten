using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestRigManager : MonoBehaviour
{
    [SerializeField] ChestRigSlot[] slots;

    public void StoreObject(int slot, GameObject obj){
        if(obj == null) return;
        if((slot > slots.Length)||(slot < 0))return;
        if(slots[slot].hasObject())return;
        slots[slot].StoreObject(obj);
    }

    public GameObject RetrieveItem(int slot){
        if((slot > slots.Length-1)||(slot < 0)) return null;

        GameObject retrievedItem = slots[slot].RetrieveObject();
        slots[slot].NullObj();
//        print("retrievedItem: "+retrievedItem.name);
        return retrievedItem;
    }

    public bool IsFree(int slot){
        return !(slots[slot].hasObject());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class Preset
{
    private int LIMIT = 25;
    private int current;
    private List<int> quantities;

    public Preset(){
        quantities = new List<int>();

        for(int i = 0; i < 6; i++)
            quantities.Add(0);
        current = 0;
        
    }

    public int getD4(){
        return quantities[0];
    }

    public int getD6(){
        return quantities[1];
    }

    public int getD8(){
        return quantities[2];
    }

    public int getD10(){
        return quantities[3];
    }

    public int getD12(){
        return quantities[4];
    }

    public int getD20(){
        return quantities[5];
    }

    public int GetIndex(int index){
        return quantities[index];
    }

    public bool Increment(int index){
        if((current + 1) <= LIMIT){
            quantities[index]++;
            current++;
            return true;
        }
        else{
            return false;
        }
    }

    public bool Decrement(int index){
        if(current != 0 && quantities[index] != 0){
            quantities[index]--;
            current--;
            return true;
        }
        else{
            return false;
        }
    }

    public List<int> GetPresetList(){
        return quantities;
    }
}

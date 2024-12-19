using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class DataNumber
{
    public int Adata;
    public int Bdata;
    public int Cdata;
    public int Ddata;

    public int NewNumFind()
    {
        for (int i = 0; i < 100; i++)
        {
            if (i != Adata && i != Bdata && i != Cdata && i != Ddata)
            {
                return i;
            }
        }
        return -1;
    }
}

public class Data0
{
    public List<int> questLine;
    public int timeCheck;
    public int dataNumber;

    public List<ItemAmount> itemAmount;
    public List<int> status;
    public List<Module> module;
    public List<Trait> trait;
    public List<Drone> drone;
}

[Serializable]
public class ItemAmount
{
    public string name;
    public int amount;
}
[Serializable]
public class Module
{
    public string m_name;
    public int m_level;
    public int m_position;
    public int m_code;
}
[Serializable]
public class Trait
{
    public string t_na_name;
    public int t_level;
    public string t_po_name;
}
[Serializable]
public class Drone
{
    public int d_code;
    public int d_info;
    public string d_name;
    public int d_max_hp;
    public int d_hp;
    public int d_armor;
    public int d_speed;
    public int d_max_weight;
    public int d_weight;
    public int d_ap;

    public List<string> d_gear;
    public List<magazine> d_magazine;
    public List<ItemAmount> d_itemAmount;
}
[Serializable]
public class magazine
{
    public string weapon;
    public string ammunition;
    public int max;
    public int current;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class vehicle_script : MonoBehaviour
{
    public GameObject livingRoom;
    public GameObject toCockpit_car_btn;
    public GameObject toHallway_car_btn;
    public GameObject toProduce_mod_btn;
    public GameObject storage;
    public GameObject toInventory_mod_btn;
    public GameObject toGarage_car_btn2;
    public GameObject garage;
    public GameObject toParkingLot_mod_btn;
    public GameObject toLivingroom_car_btn;
    public GameObject cockpit;
    public GameObject toLivingroom_car_btn2;
    public GameObject toStatus_mod_btn;
    public GameObject toBed_mod_btn;
    public GameObject hallway;
    public GameObject toStorage_car_btn;
    public GameObject toSystem3_mod_btn;
    public GameObject toGarage_car_btn;
    public GameObject toLivingRoom_car_btn;

    public GameObject produce_mod;
    public GameObject inventory_mod;
    public GameObject toStorage_mod_btn;
    public GameObject toLivingRoom_mod_btn;
    public GameObject toGarage_mod_btn;
    public GameObject droneUpgrade_mod;
    public GameObject carUpgrade_mod_s;

    public GameObject status_icon_s;
    public GameObject timeCheck_icon_s;

    // Start is called before the first frame update
    void Start()
    {
        livingRoom.SetActive(true);
        status_icon_s.SetActive(true);
        timeCheck_icon_s.SetActive(true);
    }

    public void toCockpit_car_btn_clicked()
    {
        livingRoom.SetActive(false);
        cockpit.SetActive(true);
    }
    public void toHallway_car_btn_clicked()
    {
        livingRoom.SetActive(false);
        hallway.SetActive(true);
    }
    public void toProduce_mod_btn_clicked()
    {
        livingRoom.SetActive(false);
        produce_mod.SetActive(true);
        status_icon_s.SetActive(false);
        timeCheck_icon_s.SetActive(false);
    }

    public void toInventory_mod_btn_clicked()
    {
        storage.SetActive(false);
        inventory_mod.SetActive(true);
        status_icon_s.SetActive(false);
        timeCheck_icon_s.SetActive(false);
    }
    public void toHallway_car_btn2_clicked()
    {
        storage.SetActive(false);
        hallway.SetActive(true);
    }

    public void toParkingLot_mod_btn_clicked()
    {
        droneUpgrade_mod.SetActive(true);
        garage.SetActive(false);
        status_icon_s.SetActive(false);
        timeCheck_icon_s.SetActive(false);
    }
    public void toHallway_car_btn3_clicked()
    {
        garage.SetActive(false);
        hallway.SetActive(true);
    }

    public void toLivingroom_car_btn2_clicked()
    {
        cockpit.SetActive(false);
        livingRoom.SetActive(true);
    }
    public void toStatus_mod_btn_clicked()
    {
        SceneManager.LoadScene("WorldMap_scen");
    }
    public void toBed_mod_btn_clicked()
    {

    }

    public void toStorage_car_btn_clicked()
    {
        hallway.SetActive(false);
        storage.SetActive(true);
    }
    public void toSystem3_mod_btn_clicked()
    {
        hallway.SetActive(false);
        carUpgrade_mod_s.SetActive(true);
        status_icon_s.SetActive(false);
        timeCheck_icon_s.SetActive(false);
    }
    public void toGarage_car_btn_clicked()
    {
        hallway.SetActive(false);
        garage.SetActive(true);
    }
    public void toLivingRoom_car_btn_clicked()
    {
        hallway.SetActive(false);
        livingRoom.SetActive(true);
    }

    public void toStorage_mod_btn_clicked()
    {
        storage.SetActive(true);
        inventory_mod.SetActive(false);
        status_icon_s.SetActive(true);
        timeCheck_icon_s.SetActive(true);
    }
    public void toLivingRoom_mod_btn_clicked()
    {
        livingRoom.SetActive(true);
        produce_mod.SetActive(false);
        status_icon_s.SetActive(true);
        timeCheck_icon_s.SetActive(true);
    }
    public void toHallway_mod_btn_clicked()
    {
        hallway.SetActive(true);
        carUpgrade_mod_s.SetActive(false);
        status_icon_s.SetActive(true);
        timeCheck_icon_s.SetActive(true);
    }
    public void toGarage_car_mod_clicked()
    {
        garage.SetActive(true);
        droneUpgrade_mod.SetActive(false);
        status_icon_s.SetActive(true);
        timeCheck_icon_s.SetActive(true);
    }
}


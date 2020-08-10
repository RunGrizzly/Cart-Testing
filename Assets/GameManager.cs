using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    // public static GameManager ins;

    // [Header("Sub Managers")]
    // public UIManager uiManager;
    // [Header("Data")]
    // public PathCreator pathCreator;
    // public GameObject playerPrefab;

    // //ImportantTrackPoints.
    // Vector3 trackStartPoint;

    // private void Awake()
    // {
    //     ins = this;
    // }

    // private void Start()
    // {
    //     //Spawn player at start point.
    //     trackStartPoint = pathCreator.path.GetPointAtTime(0);
    // }

    // void InitialiseRace()
    // {
    //     SpawnPlayer();
    // }

    // void SpawnPlayer()
    // {
    //     GameObject playerClone = Instantiate(playerPrefab);
    //     playerClone.transform.rotation = pathCreator.path.GetRotation(0);

    //     uiManager.screenUI.worldCamera = playerClone.GetComponent<PlayerCart>().playerCamera;
    // }
}
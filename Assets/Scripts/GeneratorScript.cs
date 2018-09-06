﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorScript : MonoBehaviour {

    //Rooms
    [SerializeField] GameObject[] availableRooms;
    [SerializeField] List<GameObject> currentRooms;
    float screenWidthInPoints;

    //Objects
    [SerializeField] GameObject[] availableObjects;
    [SerializeField] List<GameObject> objects;
    [SerializeField] float objectsMinDistance = 5.0f;
    [SerializeField] float objectsMaxDistance = 10.0f;
    [SerializeField] float objectsMinY = -1.4f;
    [SerializeField] float objectsMaxY = 1.4f;
    [SerializeField] float objectsMinRotation = -45.0f;
    [SerializeField] float objectsMaxRotation = 45.0f;


    // Use this for initialization
    void Start () {
        float height = 2.0f * Camera.main.orthographicSize;
        screenWidthInPoints = height * Camera.main.aspect;
        StartCoroutine(GeneratorCheck());
    }

    // Update is called once per frame
    void Update () {
		
	}

    void AddObject(float lastObjectX) {
        int randomIndex = Random.Range(0, availableObjects.Length);
        GameObject obj = (GameObject)Instantiate(availableObjects[randomIndex]);
        float objectPositionX = lastObjectX + Random.Range(objectsMinDistance, objectsMaxDistance);
        float randomY = Random.Range(objectsMinY, objectsMaxY);
        obj.transform.position = new Vector3(objectPositionX, randomY, 0);
        float rotation = Random.Range(objectsMinRotation, objectsMaxRotation);
        obj.transform.rotation = Quaternion.Euler(Vector3.forward * rotation);
        objects.Add(obj);
    }

    void GenerateObjectsIfRequired() {
        float playerX = transform.position.x;
        float removeObjectsX = playerX - screenWidthInPoints;
        float addObjectX = playerX + screenWidthInPoints;
        float farthestObjectX = 0;
        List<GameObject> objectsToRemove = new List<GameObject>();

        foreach (var obj in objects) {
            float objX = obj.transform.position.x;
            farthestObjectX = Mathf.Max(farthestObjectX, objX);
            if (objX < removeObjectsX)
                objectsToRemove.Add(obj);
        }

        foreach (var obj in objectsToRemove) {
            objects.Remove(obj);
            Destroy(obj);
        }
        if (farthestObjectX < addObjectX)
            AddObject(farthestObjectX);
    }

    void AddRoom(float farthestRoomEndX) {
        int randomRoomIndex = Random.Range(0, availableRooms.Length);
        GameObject room = (GameObject)Instantiate(availableRooms[randomRoomIndex]);
        float roomWidth = room.transform.Find("floor").localScale.x;
        float roomCenter = farthestRoomEndX + roomWidth * 0.5f;
        room.transform.position = new Vector3(roomCenter, 0, 0);
        currentRooms.Add(room);
    }

    void GenerateRoomIfRequired() {
        List<GameObject> roomsToRemove = new List<GameObject>();
        bool addRooms = true;
        float playerX = transform.position.x;
        float removeRoomX = playerX - screenWidthInPoints;
        float addRoomX = playerX + screenWidthInPoints;
        float farthestRoomEndX = 0;

        foreach (var room in currentRooms) {
            float roomWidth = room.transform.Find("floor").localScale.x;
            float roomStartX = room.transform.position.x - (roomWidth * 0.5f);
            float roomEndX = roomStartX + roomWidth;

            if (roomStartX > addRoomX) {
                addRooms = false;
            }

            if (roomEndX < removeRoomX) {
                roomsToRemove.Add(room);
            }

            farthestRoomEndX = Mathf.Max(farthestRoomEndX, roomEndX);
        }

        foreach (var room in roomsToRemove) {
            currentRooms.Remove(room);
            Destroy(room);
        }

        if (addRooms) {
            AddRoom(farthestRoomEndX);
        }
    }

    IEnumerator GeneratorCheck()
    {
        while (true)
        {
            GenerateRoomIfRequired();
            GenerateObjectsIfRequired();
            yield return new WaitForSeconds(0.25f);
        }
    }
}

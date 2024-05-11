using System;
using UnityEngine;

public class IDGenerator : MonoBehaviour
{
    // This will hold the unique ID for the object
    public string uniqueID;

    // Call this method to get the unique ID
    public string GetUniqueID()
    {
        // If the unique ID is not generated yet, generate it
        if (string.IsNullOrEmpty(uniqueID))
        {
            // Generate a unique ID using a combination of GameObject name and a random number
            uniqueID = Guid.NewGuid().ToString();
        }
        return uniqueID;
    }
}

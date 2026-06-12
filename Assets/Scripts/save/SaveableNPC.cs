using UnityEngine;

[RequireComponent(typeof(SaveableObject))]
public class SaveableNPC : MonoBehaviour
{
    [Header("Флаги диалогов")]
    public bool HasTalked = false;

    SaveableObject saveableObject;

    void Awake()
    {
        saveableObject = GetComponent<SaveableObject>();
    }

    public SaveableNPCData GetSaveData()
    {
        return new SaveableNPCData
        {
            ObjectID = saveableObject.ObjectID,
            IsActive = gameObject.activeSelf,
            Position = transform.position,
            HasTalked = HasTalked
        };
    }

    public void LoadData(SaveableNPCData data)
    {
        HasTalked = data.HasTalked;

        if (!data.IsActive)
        {
            Destroy(gameObject);
        }
    }
}

[System.Serializable]
public class SaveableNPCData
{
    public string ObjectID;
    public bool IsActive;
    public Vector2 Position;
    public bool HasTalked;
}
using UnityEngine;

/// <summary>
/// Компонент для объектов, состояние которых нужно сохранять
/// </summary>
public class SaveableObject : MonoBehaviour
{
    [Header("Идентификация")]
    [Tooltip("Уникальный ID объекта (генерируется автоматически)")]
    public string ObjectID;

    [Header("Что сохранять")]
    public bool SavePosition = false;
    public bool SaveActiveState = true;  // Активен/удалён ли объект

    void OnValidate()
    {
        // Генерируем уникальный ID при создании объекта
        if (string.IsNullOrEmpty(ObjectID))
        {
            ObjectID = System.Guid.NewGuid().ToString();
        }
    }

    /// <summary>
    /// Получить данные для сохранения
    /// </summary>
    public SaveableObjectData GetSaveData()
    {
        return new SaveableObjectData
        {
            ObjectID = ObjectID,
            IsActive = gameObject.activeSelf,
            Position = SavePosition ? (Vector2)transform.position : Vector2.zero
        };
    }

    /// <summary>
    /// Применить загруженные данные
    /// </summary>
    public void LoadData(SaveableObjectData data)
    {
        if (!data.IsActive)
        {
            // Объект был удалён/деактивирован
            Destroy(gameObject);
        }
        else if (SavePosition)
        {
            transform.position = data.Position;
        }
    }
}

/// <summary>
/// Данные одного сохраняемого объекта
/// </summary>
[System.Serializable]
public class SaveableObjectData
{
    public string ObjectID;
    public bool IsActive;
    public Vector2 Position;
}
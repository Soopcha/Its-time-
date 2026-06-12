using UnityEngine;
using System.Collections.Generic;

public class PerlinNoiseMap : MonoBehaviour
{
    [Header("Настройки шума Перлина")]
    public bool UsePerlinNoise = true;          // Использовать шум для размытия границ
    public float NoiseScale = 10f;              // Масштаб шума
    public float OffsetX = 0f;
    public float OffsetY = 0f;
    [Range(0f, 0.5f)]
    public float NoiseStrength = 0.1f;          // Сила размытия границ (0 = чёткие границы)

    [Header("Направление зон")]
    public ZoneAxis Axis = ZoneAxis.LeftToRight;  // По какой оси раскладываем зоны

    public enum ZoneAxis
    {
        LeftToRight,    // Слева направо (по X)
        RightToLeft,    // Справа налево
        BottomToTop,    // Снизу вверх (по Y)
        TopToBottom     // Сверху вниз
    }

    [Header("Зоны (в порядке от начала к концу)")]
    public List<ZoneSegment> Zones = new List<ZoneSegment>();

    [System.Serializable]
    public class ZoneSegment
    {
        public string Name;                      
        public ZoneType Type;                    
        [Range(0f, 100f)]
        public float EndPercent;                

        public ZoneSegment(string name, ZoneType type, float endPercent)
        {
            Name = name;
            Type = type;
            EndPercent = endPercent;
        }
    }

    public enum ZoneType
    {
        Swamp,
        Field,
        Forest
    }

    // Заполнить дефолтными зонами
    void Reset()
    {
        Zones = new List<ZoneSegment>
        {
            new ZoneSegment("Лес", ZoneType.Forest, 30f),
            new ZoneSegment("Поле", ZoneType.Field, 70f),
            new ZoneSegment("Болото", ZoneType.Swamp, 100f)
        };
    }

    public void RandomizeOffset()
    {
        OffsetX = Random.Range(0f, 10000f);
        OffsetY = Random.Range(0f, 10000f);
    }

    // Получить позицию на оси (0 до 1)
    float GetAxisPosition(int x, int y, int width, int height)
    {
        switch (Axis)
        {
            case ZoneAxis.LeftToRight:
                return (float)x / (width - 1);      

            case ZoneAxis.RightToLeft:
                return 1f - (float)x / (width - 1); 

            case ZoneAxis.BottomToTop:
                return (float)y / (height - 1);    

            case ZoneAxis.TopToBottom:
                return 1f - (float)y / (height - 1); 

            default:
                return (float)x / (width - 1);
        }
    }

    // Получить значение шума для размытия границ
    float GetNoiseBias(int x, int y, int width, int height)
    {
        if (!UsePerlinNoise || NoiseStrength <= 0)
            return 0f;

        float xCoord = (float)x / width * NoiseScale + OffsetX;
        float yCoord = (float)y / height * NoiseScale + OffsetY;

        // Шум от -0.5 до 0.5, умноженный на силу
        return (Mathf.PerlinNoise(xCoord, yCoord) - 0.5f) * NoiseStrength * 2f;
    }

    // Главный метод — получить тип зоны для координаты
    public ZoneType GetZoneType(int x, int y, int width, int height)
    {
        if (Zones == null || Zones.Count == 0)
            return ZoneType.Field;

        // Позиция на оси от 0 до 1
        float position = GetAxisPosition(x, y, width, height);

        // Добавляем шум для размытия границ
        position += GetNoiseBias(x, y, width, height);
        position = Mathf.Clamp01(position);

        float percent = position * 100f;

        // Ищем в какую зону попадает
        foreach (var zone in Zones)
        {
            if (percent <= zone.EndPercent)
                return zone.Type;
        }

        // Если не нашли — вернуть последнюю зону
        return Zones[Zones.Count - 1].Type;
    }

    // Сгенерировать карту зон
    public ZoneType[,] GenerateZoneMap(int width, int height)
    {
        ZoneType[,] zones = new ZoneType[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                zones[x, y] = GetZoneType(x, y, width, height);
            }
        }

        return zones;
    }
}
using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TerrainGenerator _terrainGenerator;

    [Space] public TMP_InputField _seedField;

    private void Awake()
    {
        _seedField.text = _terrainGenerator._seed.ToString();
    }

    public void SetNoise(TMP_Dropdown dropdown)
    {
        _terrainGenerator._activeNoise = (TerrainGenerator.NoiseType)dropdown.value;
    }

    public void SetSeed(TMP_InputField field)
    {
        if(int.TryParse(field.text, out int result))
        {
            _terrainGenerator._seed = result;
        }
    }
    // todo: map size, max height, rotation
    // todo: perlin - octaves, persistence, offset, scale
    // todo: diamond-square - offset range, smoothness
}

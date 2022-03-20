using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TerrainGenerator _terrainGenerator;
    public Perlin _perlin;

    [Space]
    public TMP_InputField _seedField;
    public TMP_Dropdown _noiseDropdown;
    public TMP_InputField _sizeField;
    public TMP_InputField _heightField;
    [Header("Perlin")]
    public TMP_InputField _perlinOctavesField;
    public TMP_InputField _perlinPersistenceField;
    public TMP_InputField _perlinOffsetXField;
    public TMP_InputField _perlinOffsetYField;
    public TMP_InputField _perlinScaleField;
    [Space]
    public TMP_InputField _rotateSpeedField;

    private void Awake()
    {
        _seedField.text = _terrainGenerator._seed.ToString();
        _noiseDropdown.value = (int)_terrainGenerator._activeNoise;
        _sizeField.text = _terrainGenerator._mapSize.ToString();
        _heightField.text = _terrainGenerator._maxHeight.ToString();

        _perlinOctavesField.text = _perlin._octaves.ToString();
        _perlinPersistenceField.text = _perlin._persistence.ToString();
        _perlinOffsetXField.text = _perlin._offset.x.ToString();
        _perlinOffsetYField.text = _perlin._offset.y.ToString();
        _perlinScaleField.text = _perlin._scale.ToString();

        _rotateSpeedField.text = _terrainGenerator._rotateSpeed.ToString();
    }

    public void SetSeed()
    {
        if(int.TryParse(_seedField.text, out int result))
        {
            _terrainGenerator._seed = result;
        }
        else
        {
            _seedField.text = _terrainGenerator._seed.ToString();
        }
    }

    public void SetNoise()
    {
        _terrainGenerator._activeNoise = (TerrainGenerator.NoiseType)_noiseDropdown.value;
    }

    public void SetSize()
    {
        if(int.TryParse(_sizeField.text, out int result))
        {
            _sizeField.text = _terrainGenerator.SetMapSize(result).ToString();
        }
        else
        {
            _sizeField.text = _terrainGenerator._mapSize.ToString();
        }
    }

    public void SetHeight()
    {
        if(int.TryParse(_heightField.text, out int result))
        {
            _terrainGenerator._maxHeight = result;
        }
        else
        {
            _heightField.text = _terrainGenerator._maxHeight.ToString();
        }
    }
    
    public void PerlinSetOctaves()
    {
        if(int.TryParse(_perlinOctavesField.text, out int result))
        {
            _perlin._octaves = result;
        }
        else
        {
            _perlinOctavesField.text = _perlin._octaves.ToString();
        }
    }
    
    public void PerlinSetPersistence()
    {
        if(float.TryParse(_perlinPersistenceField.text, out float result))
        {
            _perlin._persistence = result;
        }
        else
        {
            _perlinPersistenceField.text = _perlin._persistence.ToString();
        }
    }

    public void PerlinSetOffset()
    {
        float x = _perlin._offset.x;
        float y = _perlin._offset.y;
        if(float.TryParse(_perlinOffsetXField.text, out float i))
        {
            x = i;
        }
        else
        {
            _perlinOffsetYField.text = x.ToString();
        }
        
        if(float.TryParse(_perlinOffsetYField.text, out float j))
        {
            y = j;
        }
        else
        {
            _perlinOffsetYField.text = y.ToString();
        }

        _perlin._offset = new Vector2(x, y);
    }

    public void PerlinSetScale()
    {
        if(float.TryParse(_perlinScaleField.text, out float result))
        {
            _perlin._scale = result;
        }
        else
        {
            _perlinScaleField.text = _perlin._scale.ToString();
        }
    }

    public void ToggleRotation()
    {
        _terrainGenerator._rotate = !_terrainGenerator._rotate;
    }

    public void SetRotateSpeed()
    {
        if(float.TryParse(_rotateSpeedField.text, out float result))
        {
            _terrainGenerator._rotateSpeed = result;
        }
        else
        {
            _rotateSpeedField.text = _terrainGenerator._rotateSpeed.ToString();
        }
    }
    
    // todo: rotation
    // todo: diamond-square - offset range, smoothness
}

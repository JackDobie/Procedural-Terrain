using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TerrainGenerator _terrainGenerator;
    
    [Header("Noise")]
    public Perlin _perlin;
    public DiamondSquare _diamondSquare;
    public Worley _worley;

    [Space]
    public TMP_InputField _seedField;
    public TMP_Dropdown _noiseDropdown;
    public TMP_InputField _sizeField;
    public TMP_InputField _heightField;
    [Header("Perlin")]
    public GameObject _perlinMenu;
    public TMP_InputField _perlinOctavesField;
    public TMP_InputField _perlinPersistenceField;
    public TMP_InputField _perlinOffsetXField;
    public TMP_InputField _perlinOffsetYField;
    public TMP_InputField _perlinScaleField;
    [Header("Diamond-Square")]
    public GameObject _DSMenu;
    public TMP_InputField _DSOffsetField;
    public TMP_InputField _DSSmoothnessField;
    [Header("Worley")]
    public GameObject _worleyMenu;
    public TMP_InputField _worleyCountField;
    public TMP_InputField _worleyDistanceField;
    [Header("Erosion")]
    public HydraulicErosion _hydraulic;
    public ThermalErosion _thermal;
    [Space]
    public TMP_Dropdown _erosionDropdown;
    [Header("Hydraulic Erosion")]
    public GameObject _hydraulicMenu;
    public TMP_InputField _hydraulicIterationsField;
    public TMP_InputField _hydraulicParticleCountField;
    public TMP_InputField _hydraulicGravityField;
    public TMP_InputField _hydraulicErosionRadiusField;
    public TMP_InputField _hydraulicInertiaField;
    public TMP_InputField _hydraulicCapacityField;
    public TMP_InputField _hydraulicErosionSpeedField;
    public TMP_InputField _hydraulicEvaporationSpeedField;
    public TMP_InputField _hydraulicDepositSpeedField;
    public TMP_InputField _hydraulicMinSlopeField;
    [Header("Thermal Erosion")]
    public GameObject _thermalMenu;
    public TMP_InputField _thermalIterationsField;
    public TMP_InputField _thermalMinAngleField;
    public TMP_InputField _thermalCapacityField;
    [Space]
    public Toggle _ridgedToggle;
    [Space]
    public TMP_InputField _rotateSpeedField;

    private void Start()
    {
        _seedField.characterLimit = int.MaxValue.ToString().Length - 1;
        _seedField.text = _terrainGenerator._seed.ToString();
        _noiseDropdown.value = (int)_terrainGenerator._activeNoise;
        _sizeField.text = _terrainGenerator._mapSize.ToString();
        _heightField.text = _terrainGenerator._maxHeight.ToString();

        _perlinOctavesField.text = _perlin._octaves.ToString();
        _perlinPersistenceField.text = _perlin._persistence.ToString();
        _perlinOffsetXField.text = _perlin._offset.x.ToString();
        _perlinOffsetYField.text = _perlin._offset.y.ToString();
        _perlinScaleField.text = _perlin._scale.ToString();

        _DSOffsetField.text = _diamondSquare._offsetRange.ToString();
        _DSSmoothnessField.text = _diamondSquare._smoothness.ToString();

        _worleyCountField.text = _worley._pointsCount.ToString();
        _worleyDistanceField.text = _worley._pointDistance.ToString();
        
        _rotateSpeedField.text = _terrainGenerator._rotateSpeed.ToString();
        
        switch (_terrainGenerator._activeNoise)
        {
            case TerrainGenerator.NoiseType.Perlin:
                _perlinMenu.SetActive(true);
                _DSMenu.SetActive(false);
                _worleyMenu.SetActive(false);
                break;
            case TerrainGenerator.NoiseType.DiamondSquare:
                _perlinMenu.SetActive(false);
                _DSMenu.SetActive(true);
                _worleyMenu.SetActive(false);
                break;
            case TerrainGenerator.NoiseType.Worley:
                _perlinMenu.SetActive(false);
                _DSMenu.SetActive(false);
                _worleyMenu.SetActive(true);
                break;
            default:
                _perlinMenu.SetActive(false);
                _DSMenu.SetActive(false);
                _worleyMenu.SetActive(false);
                break;
        }

        _erosionDropdown.value = (int) _terrainGenerator._activeErosion;
        switch (_terrainGenerator._activeErosion)
        {
            case TerrainGenerator.ErosionType.Hydraulic:
                _hydraulicMenu.SetActive(true);
                _thermalMenu.SetActive(false);
                break;
            case TerrainGenerator.ErosionType.Thermal:
                _hydraulicMenu.SetActive(false);
                _thermalMenu.SetActive(true);
                break;
            case TerrainGenerator.ErosionType.Both:
                _hydraulicMenu.SetActive(true);
                _thermalMenu.SetActive(true);
                break;
            default:
                _hydraulicMenu.SetActive(false);
                _thermalMenu.SetActive(false);
                break;
        }

        _hydraulicIterationsField.text = _hydraulic._iterations.ToString();
        _hydraulicParticleCountField.text = _hydraulic._particleCount.ToString();
        _hydraulicGravityField.text = _hydraulic._gravity.ToString();
        _hydraulicErosionRadiusField.text = _hydraulic._erosionRadius.ToString();
        _hydraulicInertiaField.text = _hydraulic._inertia.ToString();
        _hydraulicCapacityField.text = _hydraulic._capacity.ToString();
        _hydraulicEvaporationSpeedField.text = _hydraulic._evaporationSpeed.ToString();
        _hydraulicErosionSpeedField.text = _hydraulic._erosionSpeed.ToString();
        _hydraulicDepositSpeedField.text = _hydraulic._depositSpeed.ToString();
        _hydraulicMinSlopeField.text = _hydraulic._minSlope.ToString();

        _thermalIterationsField.text = _thermal._iterations.ToString();
        _thermalMinAngleField.text = _thermal._minAngle.ToString();
        _thermalCapacityField.text = _thermal._capacity.ToString();

        _ridgedToggle.isOn = _terrainGenerator._ridged;
    }

    private void SetSeed()
    {
        if(int.TryParse(_seedField.text, out int result))
        {
            _terrainGenerator._seed = result;
        }
        else
        {
            _seedField.text = _terrainGenerator._seed.ToString();
        }
        
        if (_terrainGenerator._seed == 0)
        {
            _terrainGenerator._seed = System.DateTime.Now.Millisecond;
            _seedField.text = _terrainGenerator._seed.ToString();
        }
    }

    public void SetNoise()
    {
        _terrainGenerator._activeNoise = (TerrainGenerator.NoiseType)_noiseDropdown.value;
        
        switch (_terrainGenerator._activeNoise)
        {
            case TerrainGenerator.NoiseType.Perlin:
                _perlinMenu.SetActive(true);
                _DSMenu.SetActive(false);
                _worleyMenu.SetActive(false);
                break;
            case TerrainGenerator.NoiseType.DiamondSquare:
                _perlinMenu.SetActive(false);
                _DSMenu.SetActive(true);
                _worleyMenu.SetActive(false);
                break;
            case TerrainGenerator.NoiseType.Worley:
                _perlinMenu.SetActive(false);
                _DSMenu.SetActive(false);
                _worleyMenu.SetActive(true);
                break;
            default:
                _perlinMenu.SetActive(false);
                _DSMenu.SetActive(false);
                _worleyMenu.SetActive(false);
                break;
        }
    }

    private void SetSize()
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

    private void SetHeight()
    {
        if(float.TryParse(_heightField.text, out float result))
        {
            _terrainGenerator._maxHeight = result;
        }
        else
        {
            _heightField.text = _terrainGenerator._maxHeight.ToString();
        }
    }

    private void PerlinSetOctaves()
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

    private void PerlinSetPersistence()
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

    private void PerlinSetOffset()
    {
        float maxSize = _terrainGenerator._mapSize > 0 ? (_terrainGenerator._mapSize - _perlin._scale - 1) : 0;
        float x = _perlin._offset.x;
        float y = _perlin._offset.y;
        if(float.TryParse(_perlinOffsetXField.text, out float i))
        {
            x = Mathf.Clamp(i, 0, maxSize);
            _perlinOffsetXField.text = x.ToString();
        }
        else
        {
            _perlinOffsetYField.text = x.ToString();
        }
        
        if(float.TryParse(_perlinOffsetYField.text, out float j))
        {
            y = Mathf.Clamp(j, 0, maxSize);
            _perlinOffsetYField.text = y.ToString();
        }
        else
        {
            _perlinOffsetYField.text = y.ToString();
        }
        
        _perlin._offset = new Vector2(x, y);
    }

    private void PerlinSetScale()
    {
        if(float.TryParse(_perlinScaleField.text, out float result))
        {
            // _perlin._scale = result;
            _perlinScaleField.text = _perlin.SetScale(result).ToString();
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

    private void DSSetOffset()
    {
        if(float.TryParse(_DSOffsetField.text, out float result))
        {
            _diamondSquare._offsetRange = result;
        }
        else
        {
            _DSOffsetField.text = _diamondSquare._offsetRange.ToString();
        }
    }

    private void DSSetSmoothness()
    {
        if(float.TryParse(_DSSmoothnessField.text, out float result))
        {
            _diamondSquare._smoothness = result;
        }
        else
        {
            _DSSmoothnessField.text = _diamondSquare._smoothness.ToString();
        }
    }

    private void WorleySetCount()
    {
        if(int.TryParse(_worleyCountField.text, out int result))
        {
            if(result >= 0)
            {
                _worley._pointsCount = result;
            }
            else
            {
                _worleyCountField.text = _worley._pointsCount.ToString();
            }
        }
        else
        {
            _worleyCountField.text = _worley._pointsCount.ToString();
        }
    }

    private void WorleySetDistance()
    {
        if(int.TryParse(_worleyDistanceField.text, out int result))
        {
            if(result >= 1 && result < _worley._pointsCount)
            {
                _worley._pointDistance = result;
            }
            else
            {
                _worleyDistanceField.text = _worley._pointDistance.ToString();
            }
        }
        else
        {
            _worleyDistanceField.text = _worley._pointDistance.ToString();
        }
    }

    public void SetErosion()
    {
        _terrainGenerator._activeErosion = (TerrainGenerator.ErosionType) _erosionDropdown.value;
        switch (_terrainGenerator._activeErosion)
        {
            case TerrainGenerator.ErosionType.Hydraulic:
                _hydraulicMenu.SetActive(true);
                _thermalMenu.SetActive(false);
                break;
            case TerrainGenerator.ErosionType.Thermal:
                _hydraulicMenu.SetActive(false);
                _thermalMenu.SetActive(true);
                break;
            case TerrainGenerator.ErosionType.Both:
                _hydraulicMenu.SetActive(true);
                _thermalMenu.SetActive(true);
                break;
            default:
                _hydraulicMenu.SetActive(false);
                _thermalMenu.SetActive(false);
                break;
        }
    }

    private void HydraulicSetIterations()
    {
        if (int.TryParse(_hydraulicIterationsField.text, out int result))
        {
            _hydraulic._iterations = result;
        }
        else
        {
            _hydraulicIterationsField.text = _hydraulic._iterations.ToString();
        }
    }

    private void HydraulicSetParticleCount()
    {
        if (int.TryParse(_hydraulicParticleCountField.text, out int result))
        {
            _hydraulic._particleCount = result;
        }
        else
        {
            _hydraulicParticleCountField.text = _hydraulic._particleCount.ToString();
        }
    }

    private void HydraulicSetGravity()
    {
        if (int.TryParse(_hydraulicGravityField.text, out int result))
        {
            _hydraulic._gravity = result;
        }
        else
        {
            _hydraulicGravityField.text = _hydraulic._gravity.ToString();
        }
    }

    private void HydraulicSetErosionRadius()
    {
        if (int.TryParse(_hydraulicErosionRadiusField.text, out int result))
        {
            _hydraulic._erosionRadius = result;
        }
        else
        {
            _hydraulicErosionRadiusField.text = _hydraulic._erosionRadius.ToString();
        }
    }

    private void HydraulicSetInertia()
    {
        if (float.TryParse(_hydraulicInertiaField.text, out float result))
        {
            _hydraulic._inertia = result;
        }
        else
        {
            _hydraulicInertiaField.text = _hydraulic._inertia.ToString();
        }
    }

    private void HydraulicSetCapacity()
    {
        if (float.TryParse(_hydraulicCapacityField.text, out float result))
        {
            _hydraulic._capacity = result;
        }
        else
        {
            _hydraulicCapacityField.text = _hydraulic._capacity.ToString();
        }
    }

    private void HydraulicSetEvaporationSpeed()
    {
        if (float.TryParse(_hydraulicEvaporationSpeedField.text, out float result))
        {
            _hydraulic._evaporationSpeed = result;
        }
        else
        {
            _hydraulicEvaporationSpeedField.text = _hydraulic._evaporationSpeed.ToString();
        }
    }
    
    private void HydraulicSetErosionSpeed()
    {
        if (float.TryParse(_hydraulicErosionSpeedField.text, out float result))
        {
            _hydraulic._erosionSpeed = result;
        }
        else
        {
            _hydraulicErosionSpeedField.text = _hydraulic._erosionSpeed.ToString();
        }
    }

    private void HydraulicSetDepositSpeed()
    {
        if (float.TryParse(_hydraulicDepositSpeedField.text, out float result))
        {
            _hydraulic._depositSpeed = result;
        }
        else
        {
            _hydraulicDepositSpeedField.text = _hydraulic._depositSpeed.ToString();
        }
    }

    private void HydraulicSetMinSlope()
    {
        if (float.TryParse(_hydraulicMinSlopeField.text, out float result))
        {
            _hydraulic._minSlope = result;
        }
        else
        {
            _hydraulicMinSlopeField.text = _hydraulic._minSlope.ToString();
        }
    }
    
    private void ThermalSetIterations()
    {
        if (int.TryParse(_thermalIterationsField.text, out int result))
        {
            _thermal._iterations = result;
        }
        else
        {
            _thermalIterationsField.text = _thermal._iterations.ToString();
        }
    }
    
    private void ThermalSetMinAngle()
    {
        if (float.TryParse(_thermalMinAngleField.text, out float result))
        {
            _thermal._minAngle = result;
        }
        else
        {
            _thermalMinAngleField.text = _thermal._minAngle.ToString();
        }
    }
    
    private void ThermalSetCapacity()
    {
        if (float.TryParse(_thermalCapacityField.text, out float result))
        {
            _thermal._capacity = result;
        }
        else
        {
            _thermalCapacityField.text = _thermal._capacity.ToString();
        }
    }

    private void SetPerlin()
    {
        PerlinSetOctaves();
        PerlinSetPersistence();
        PerlinSetOffset();
        PerlinSetScale();
    }

    private void SetDiamondSquare()
    {
        DSSetOffset();
        DSSetSmoothness();
    }

    private void SetWorley()
    {
        WorleySetCount();
        WorleySetDistance();
    }

    private void SetHydraulic()
    {
        HydraulicSetIterations();
        HydraulicSetParticleCount();
        HydraulicSetGravity();
        HydraulicSetErosionRadius();
        HydraulicSetInertia();
        HydraulicSetCapacity();
        HydraulicSetEvaporationSpeed();
        HydraulicSetErosionSpeed();
        HydraulicSetDepositSpeed();
        HydraulicSetMinSlope();
    }

    private void SetThermal()
    {
        ThermalSetIterations();
        ThermalSetMinAngle();
        ThermalSetCapacity();
    }

    public void Generate()
    {
        SetSeed();
        SetSize();
        SetHeight();
        switch (_terrainGenerator._activeNoise)
        {
            case TerrainGenerator.NoiseType.Perlin:
                SetPerlin();
                break;
            case TerrainGenerator.NoiseType.DiamondSquare:
                SetDiamondSquare();
                break;
            case TerrainGenerator.NoiseType.Worley:
                SetWorley();
                break;
            default:
                // perlin by default
                SetPerlin();
                break;
        }

        switch (_terrainGenerator._activeErosion)
        {
            case TerrainGenerator.ErosionType.Hydraulic:
                SetHydraulic();
                break;
            case TerrainGenerator.ErosionType.Thermal:
                SetThermal();
                break;
            case TerrainGenerator.ErosionType.Both:
                SetHydraulic();
                SetThermal();
                break;
            default:
                break;
        }

        _terrainGenerator._ridged = _ridgedToggle.isOn;
        
        _terrainGenerator.Generate();
    }
}

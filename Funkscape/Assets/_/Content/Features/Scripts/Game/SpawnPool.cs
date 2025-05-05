using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class SpawnPool : MonoBehaviour
{
    #region Private Variables
    
    [SerializeField] private GameObject _poolPrefab;
    [SerializeField] private int _poolSize;

    private List<GameObject> _poolList = new List<GameObject>();

    #endregion
    
    #region Unity API
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateInstances();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion
    
    #region Main Methods

    private void CreateInstances()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            var instance = Instantiate(_poolPrefab, transform);
            instance.SetActive(false);
            _poolList.Add(instance);
        }
    }
    
    public GameObject GetFirstAvailableProjectile()
    {
        foreach (var instance in _poolList)
        {
            if (instance.activeSelf == false)
            {
                return instance;
            }
        }
        var newInstance = Instantiate(_poolPrefab, transform);
        newInstance.SetActive(false);
        _poolList.Add(newInstance);
        return newInstance;
    }
    
    public int ActiveProjectileCount => _poolList.Count(x => x.activeSelf);
    
    #endregion
}

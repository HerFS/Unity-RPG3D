using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCustomize : MonoBehaviour
{
    [Header("Hair")]
    [SerializeField]
    private Transform _hairGroup;
    private List<GameObject> _hairList = new List<GameObject>();
    [SerializeField]
    private Button _hairRightButton;
    [SerializeField]
    private Button _hairLeftButton;
    private int _currentHairIndex = 0;

    [Header("Head")]
    [SerializeField]
    private Transform _headGroup;
    private List<GameObject> _headList = new List<GameObject>();
    [SerializeField]
    private Button _headRightButton;
    [SerializeField]
    private Button _headLeftButton;
    private int _currentHeadIndex = 0;

    [Header("Beard")]
    [SerializeField]
    private Transform _beardGroup;
    private List<GameObject> _beardList = new List<GameObject>();
    [SerializeField]
    private Button _beardRightButton;
    [SerializeField]
    private Button _beardLeftButton;
    private int _currentBeardIndex = 0;

    private void Start()
    {
        InitClothes();
    }

    private void InitClothes()
    {
        SetClothesList(_hairGroup, _hairList, _currentHairIndex);
        SetClothesList(_headGroup, _headList, _currentHeadIndex);
        SetClothesList(_beardGroup, _beardList, _currentBeardIndex);

        ChangeClothes(_currentHairIndex, _hairLeftButton, _hairRightButton, _hairList);
        ChangeClothes(_currentHeadIndex, _headLeftButton, _headRightButton, _headList);
        ChangeClothes(_currentBeardIndex, _beardLeftButton, _beardRightButton, _beardList);
    }

    private void SetClothesList(Transform clothesGroup, List<GameObject> clothesList, int dressNumber)
    {
        foreach (Transform clothes in clothesGroup)
        {
            clothes.gameObject.SetActive(false);
            clothesList.Add(clothes.gameObject);
        }

        clothesList[dressNumber].SetActive(true);
    }

    private void ChangeClothes(int value, Button leftButton, Button rightButton, List<GameObject> clothesList)
    {
        int listMaxNum = clothesList.Count - 1;

        leftButton.onClick.AddListener(() =>
        {
            clothesList[value].SetActive(false);
            --value;
            if (value < 0)
            {
                value = listMaxNum;
            }
            clothesList[value].SetActive(true);
            SaveCurrentDresses(value, clothesList);
        });

        rightButton.onClick.AddListener(() =>
        {
            clothesList[value].SetActive(false);
            ++value;
            if (listMaxNum < value)
            {
                value = 0;
            }
            clothesList[value].SetActive(true);
            SaveCurrentDresses(value, clothesList);
        });
    }

    private void SaveCurrentDresses(int value, List<GameObject> clothesList)
    {
        if (clothesList == _hairList)
        {
            PlayerPrefs.SetInt("Hair", value);
        }
        else if (clothesList == _headList)
        {
            PlayerPrefs.SetInt("Head", value);
        }
        else
        {
            PlayerPrefs.SetInt("Beard", value);
        }
    }
}

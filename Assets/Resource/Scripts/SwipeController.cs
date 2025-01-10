using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwipeController : MonoBehaviour//, IEndDragHandler
{
    [SerializeField] MemeGridGen memeGridGen;
    [SerializeField] int maxPage;
    int curPage;
    Vector3 targetPos;
    [SerializeField] Vector3 pageStep;
    [SerializeField] RectTransform levelPageRect;
    [SerializeField] float tweenTime;
    [SerializeField] LeanTweenType tweenType;
    float dragThreshould;

    [SerializeField] Button preButton, nextButton;
    [SerializeField] private Transform PageNumberParent;
    [SerializeField] private GameObject pageNumberPrefabs;
    private List<Button> pageNumberButtons = new List<Button>();

    void Awake()
    {
        curPage = 1;
        targetPos = levelPageRect.localPosition;
        dragThreshould = Screen.width / 15;
       // maxPage = Mathf.CeilToInt(memeGridGen.memeElements.Count / (float)memeGridGen.memePerPage);  // Tính số trang tối đa
        maxPage = memeGridGen.maxPage;
        GeneratePageNumber();
        updateArrowButton();
    }
    public void NextPage()
    {
        if (curPage < maxPage)
        {
            curPage++;
            memeGridGen.NextPage(); // gọi phương thức NextPage của MemeGridGen
            updateArrowButton();
        }
    }
    public void PrevPage() 
    {
        if (curPage > 1)
        {
            curPage--;
            memeGridGen.PrevPage(); // gọi phương thức PrevPage của MemeGridGen
            updateArrowButton();
        }
    }
    public void updateArrowButton()
    {
       // nextButton.interactable = true;
        preButton.interactable = curPage > 1;
        nextButton.interactable = curPage < maxPage;
        //preButton.interactable = true;
        //if(curPage == 1)
        //{
        //    preButton.interactable = false;
        //}
        //if(curPage == maxPage)
        //{
        //    nextButton.interactable = false;
        //}
    }
    public void GoToPage(int pageIndex)
    {
        if(pageIndex >=1 && pageIndex <= maxPage)
        {
            curPage = pageIndex;
            memeGridGen.curPage = pageIndex;

            memeGridGen.GenarateGrid();
            updateArrowButton();
        }
    }
    private void GeneratePageNumber()
    {
        foreach(Transform child in PageNumberParent)
        {
            Destroy(child.gameObject);
        }
        pageNumberButtons.Clear();
        for (int i = 1; i <= maxPage; i++)
        {
            GameObject newPageNumber = Instantiate(pageNumberPrefabs, PageNumberParent);
            Button button = newPageNumber.GetComponent<Button>();
            Text buttonText = newPageNumber.GetComponentInChildren<Text>();
            buttonText.text = i.ToString();

            int pageIndex = i; 
            button.onClick.AddListener(() => GoToPage(pageIndex));

            pageNumberButtons.Add(button);
        }
    }

    public void UpdateMaxPage()
    {
        maxPage = memeGridGen.maxPage;
        curPage = 1;
        updateArrowButton();
        GeneratePageNumber();
       
    }
}

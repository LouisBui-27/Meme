using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemeGridGen : MonoBehaviour
{
    //public List<AudioClip> memeClips;
    //public List<Sprite> memeImages;
    [SerializeField] SwipeController swipeController;
    [SerializeField] Dropdown DropdownCountry;
    [SerializeField] MemeManagerData memeManagerData;
    private Country selectedCountry;
    private bool isFilterByCountry = false;
    public List<MemeElement> memeElements;
    [SerializeField] GameObject buttonPrefabs;
    [SerializeField] GameObject textButtonPrefab;
    [SerializeField] Transform gridContainer;
    [SerializeField] Sprite favouriteIcon;
    [SerializeField] Sprite defaultIcon; 
    
    [SerializeField] Button showFav;
    [SerializeField] GameObject NextButton;
    [SerializeField] GameObject PreButton;
    [SerializeField] GameObject ShowUIEmpty;

    public int memePerPage;
    public int curPage;
    public int maxPage;

    private bool showMemeFav = false;

    private void Awake()
    {
        isFilterByCountry = false ;
        
        //if (memeElements != null && memeElements.Count > 0)
        //{
       
        memeElements = memeManagerData.LoadData();
      
        if (memeElements == null || memeElements.Count == 0)
        {
            Debug.LogWarning("No memes found. Using default meme list.");
        }

         memeManagerData.SaveData(memeElements);
        DropdownCountry.ClearOptions();
        List<string> countries = new List<string> { "All"};    
        foreach(var country in System.Enum.GetValues(typeof(Country)))
        {
            countries.Add(country.ToString());
        }
        DropdownCountry.AddOptions(countries);
        DropdownCountry.onValueChanged.AddListener(OnCountryFilterChange);

        assignIDAndGenarateGrid();
      
              //GenarateGrid();
        
    }
    public void assignIDAndGenarateGrid()
    {
        for (int i = 0; i < memeElements.Count; i++)
        {
            memeElements[i].id = i;
            //SaveFavourite(memeElements[i]);
        }
        LoadFavourites();
        GenarateGrid();
    }

    public void GenarateGrid()
    {
        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }

        List<MemeElement> memeShow;
        bool isFavEmpty = false;
        if (showMemeFav)
        {
            memeShow = memeElements.FindAll(meme => meme.isFavourite);
            if (isFilterByCountry)
            {
                memeShow = memeShow.FindAll(meme => meme.country == selectedCountry);
            }

            if (memeShow.Count == 0)
            {
                isFavEmpty = true;
            }
            UpdateFavFolder(favouriteIcon);
        }
        else if (isFilterByCountry)
        {
            memeShow = memeElements.FindAll(meme => meme.country == selectedCountry);
            Debug.Log(memeShow.Count);
            if (showMemeFav)
            {
                memeShow = memeElements.FindAll(meme => meme.isFavourite);
            }

            if(memeShow.Count == 0)
            {
                maxPage = 0;
                ShowEmptyFav(true);
                //return;
            }
            UpdateFavFolder(defaultIcon);
        }
        else
        {
            memeShow = memeElements;
            UpdateFavFolder(defaultIcon);
        }
        foreach (var meme in memeShow)
        {
            meme.LoadAssets(); // Gọi LoadAssets cho mỗi meme
        }
        maxPage = Mathf.CeilToInt(memeShow.Count / (float)memePerPage);

        if(memeShow.Count == 0)
        {
            ShowEmptyFav(isFavEmpty);
            return;
        }
        else
        {
            HideEmptyFav();
        }
        curPage = Mathf.Clamp(curPage, 1, maxPage);
        int startIdx = (curPage - 1) * memePerPage;
        int endIdx = Mathf.Min(startIdx + memePerPage, memeShow.Count);

        if (startIdx >= memeShow.Count)
        {
            Debug.Log("Start index out of meme list. check Code");
            return;
        }

        for (int i = startIdx; i < endIdx; i++)
        {
            GameObject newButton;
            MemeElement meme = memeShow[i];

            if (meme.image != null)
            {
                // Sử dụng prefab button có hình ảnh
                newButton = Instantiate(buttonPrefabs, gridContainer);

                Image buttonImage = newButton.transform.Find("Images").GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.sprite = meme.image;  // Gán hình ảnh meme vào button
                    meme.ApplyEffect(newButton);
                }
                Button favButton = newButton.transform.Find("FavouriteButton").GetComponent<Button>();
                if(favButton != null)
                {
                    Image favIcon = favButton.GetComponent<Image>();
                    UpdateFavButton(favIcon, meme.isFavourite);
                    favButton.onClick.AddListener(() =>
                    {
                        meme.isFavourite = !meme.isFavourite; 
                        UpdateFavButton(favIcon, meme.isFavourite);
                        SaveFavourite(meme);
                    });
                }
            }
            else
            {
                // Sử dụng prefab button chỉ có văn bản
                newButton = Instantiate(textButtonPrefab, gridContainer);

                Text buttonText = newButton.GetComponentInChildren<Text>();
                if (buttonText != null && !string.IsNullOrEmpty(meme.text))
                {
                    buttonText.text = meme.text;  // Gán văn bản vào button nếu có
                }
                Button favButton = newButton.transform.Find("FavouriteButton").GetComponent<Button>();
                if (favButton != null)
                {
                    Image favIcon = favButton.GetComponent<Image>();
                    UpdateFavButton(favIcon, meme.isFavourite);
                    favButton.onClick.AddListener(() =>
                    {
                        meme.isFavourite = !meme.isFavourite; 
                        UpdateFavButton(favIcon, meme.isFavourite);
                        SaveFavourite(meme);
                    });
                }
            }
            
            // Thêm clip và hiệu ứng
            Button button = newButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() =>
                {
                    if (meme.clip != null)
                    {
                        GameManage.instance.PlaySound(meme.clip);  // Phát clip âm thanh nếu có
                    }
                });
            }
        }
    }

    public void ShowFav()
    {
        showMemeFav = !showMemeFav;
        curPage = 1;
        //if (curPage > maxPage && maxPage > 0)
        //{
        //    curPage = 1;
        //}
        GenarateGrid();
        swipeController.UpdateMaxPage();
       
    }
    public void NextPage()
    {
        if (curPage < Mathf.CeilToInt(memeElements.Count / (float)memePerPage)) // Kiểm tra xem có phải là trang cuối không
        {
            curPage++;
            GenarateGrid();
        }
    }

    public void PrevPage()
    {
        if (curPage > 1)  // Kiểm tra xem có phải là trang đầu không
        {
            curPage--;
            GenarateGrid();
        }
    }
    public void PlayRandomSound()
    {
        if (memeElements.Count > 0)
        {
            int ranIndex = Random.Range(0, memeElements.Count);
            MemeElement randomMeme = memeElements[ranIndex];
            if (randomMeme.clip != null)
            {
                GameManage.instance.PlaySound(randomMeme.clip);
            }
        }
    }
    private void UpdateFavButton(Image icon, bool isFavourite)
    {
        if (isFavourite)
        {
            icon.sprite = favouriteIcon;
        }
        else
        {
            icon.sprite = defaultIcon;
        }
    } 
    private void UpdateFavFolder(Sprite icon)
    {
        if (showFav != null)
        {
            Image Image = showFav.GetComponent<Image>();
            if (Image != null)
            {
                Image.sprite = icon;
            }
        }
    }
    private void ShowEmptyFav(bool isEmpty)
    {
        if (isEmpty)
        {
            ShowUIEmpty.SetActive(true);
            NextButton.SetActive(false);
            PreButton.SetActive(false);
        }
            
    }
    private void HideEmptyFav()
    {
        ShowUIEmpty.SetActive(false);
        NextButton.SetActive(true);
        PreButton.SetActive(true);
    }
    public void SaveFavourite(MemeElement meme)
    {
        //PlayerPrefs.SetInt("meme_" + meme.id.ToString() + "_fav", meme.isFavourite ? 1 : 0);
        //PlayerPrefs.Save();
        memeManagerData.SaveData(memeElements);
        Debug.Log($"Saved meme {meme.id} favourite status: {meme.isFavourite}");
    }
    public void LoadFavourites()
    {
        //foreach (var meme in memeElements)
        //{
        //    int favStatus = PlayerPrefs.GetInt("meme_" + meme.id.ToString() + "_fav", 0);
        //    meme.isFavourite = favStatus == 1;
        //    memeManagerData.LoadData();
        //}
        memeElements = memeManagerData.LoadData();
    }
    private void OnCountryFilterChange(int index)
    {
        if(index == 0)
        {
            isFilterByCountry = false;
        }
        else
        {
            isFilterByCountry = true;
            selectedCountry = (Country)(index - 1);
        }
        curPage = 1;
        GenarateGrid();
        swipeController.UpdateMaxPage();
    }
}

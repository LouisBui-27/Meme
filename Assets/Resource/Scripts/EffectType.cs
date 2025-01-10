
using UnityEngine;
using UnityEngine.UIElements;

public enum EffectType
{
    ScaleBounce,
    RotateZ,
    RotateY, 
    MoveY,
    ShakeRotationZ,
    None
}
[System.Serializable]
public class MemeElement
{
    public int id;
    // public Sprite image;
    public string imagePath;
    [TextArea]
    public string text;
    //public AudioClip clip;
    public string audioPath;
    public EffectType effectType;
    public Country country;
    public float duration;
    public float rotationSpeed;
    public bool isFavourite = false;
    [System.NonSerialized] public Sprite image;
    [System.NonSerialized] public AudioClip clip;


    public MemeElement(int id,string imagePath, string text, string audioPath, EffectType effectType, Country country, float duration, float rotationSpeed, bool isFavourite)
    {
        this.id = id;
        this.imagePath = imagePath;
        this.text = text;
        this.imagePath = imagePath;
        this.effectType = effectType;
        this.country = country;
        this.duration = duration;
        this.rotationSpeed = rotationSpeed;
        this.isFavourite = isFavourite;
    }
    public void LoadAssets()
    {
        if (!string.IsNullOrEmpty(imagePath))
        {
            image = Resources.Load<Sprite>(imagePath); // Tải Sprite từ đường dẫn
        }
        if (!string.IsNullOrEmpty(audioPath))
        {
            clip = Resources.Load<AudioClip>(audioPath); // Tải AudioClip từ đường dẫn
        }
    }
    public void ApplyEffect(GameObject memeObject)
    {
        ImageEffect imageEffect = memeObject.transform.Find("Images").GetComponent<ImageEffect>();
        if (imageEffect != null)
        {
            imageEffect.EffectType = effectType;
            imageEffect.duration = duration;
            imageEffect.rotationSpeed = rotationSpeed;
           // imageEffect.TriggerEffect();
        }
    }
}
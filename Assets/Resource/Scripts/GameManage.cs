using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManage : MonoBehaviour
{
    public static GameManage instance;


    private AudioSource audioSource;
    //private List<AudioSource> spamAudios = new List<AudioSource>();
    [SerializeField]private List<AudioSource> availableSources = new List<AudioSource>();  // Danh sách các AudioSource có sẵn để tái sử dụng
    [SerializeField] private List<AudioSource> activeSources = new List<AudioSource>();
    private AudioClip currentClip;
    private bool isLooping = false;
    private bool isSpamming = false;
    [SerializeField] Slider progressSlider;

    private void Awake()
    {
        progressSlider.value = 0;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Đảm bảo AudioSource được gắn vào
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.Stop();
            currentClip = clip;
            audioSource.clip = currentClip;

            if (progressSlider != null)
            {
                progressSlider.value = 0f; // Reset slider khi âm thanh mới bắt đầu
                progressSlider.maxValue = audioSource.clip.length; // Cập nhật giá trị max của slider theo độ dài âm thanh
                // Bắt đầu Coroutine để cập nhật thanh chạy
                StartCoroutine(UpdateProgressBar());
            }  
            if (isSpamming)
            {
                PlaySpamAudio(clip);
            }


            if (isLooping )
            {
                PlaySoundLoop(clip); // Phát ngay trong chế độ loop nếu được bật
                if (!isSpamming)
                {
                    StartCoroutine(UpdateProgressBar());
                }
            }
            else
            {
                audioSource.loop = false; // Đảm bảo loop bị tắt nếu không cần
                audioSource.Play(); // Phát bình thường nếu không loop
                if (!isSpamming)
                {
                    StartCoroutine(UpdateProgressBar());
                }

            }
         
        }
        else
        {
            Debug.LogWarning("No audio clip provided to play.");
        }
        
    }

    private IEnumerator UpdateProgressBar()
    {
        while (audioSource.isPlaying)
        {
            progressSlider.value = audioSource.time; // Cập nhật giá trị slider với thời gian hiện tại của audio
            yield return null;  // Chờ frame tiếp theo
        }

        // Khi âm thanh kết thúc, thiết lập slider về 0 hoặc giá trị mặc định
        progressSlider.value = 0f;
    }

    public void PlaySoundLoop(AudioClip clip)
    {
        if (clip != null)
        {
            if (currentClip != clip || !audioSource.isPlaying)
            {
                // Nếu âm thanh khác hoặc không đang phát, phát lại từ đầu
                audioSource.Stop();
                audioSource.clip = clip;
                audioSource.loop = true;
                audioSource.Play();
            }
            else
            {
                // Nếu là âm thanh hiện tại, chỉ bật loop
                audioSource.loop = true;
            }
            currentClip = clip;
            isLooping = true;
        }
    }
    public void PlaySpamAudio(AudioClip clip)
    {
        // Tìm một AudioSource từ pool đã hoàn thành việc phát
        AudioSource availableSource = GetAvailableAudioSource();
        if (availableSource == null)
        {
            // Nếu không có nguồn âm thanh nào, tạo mới một nguồn
            availableSource = gameObject.AddComponent<AudioSource>();
        }
        // Gán clip và bắt đầu phát
        availableSource.clip = clip;
        
        availableSource.Play();

        // Thêm vào danh sách các AudioSource đang phát
        activeSources.Add(availableSource);

        // Theo dõi sự kết thúc âm thanh và trả lại nguồn khi hoàn tất
        StartCoroutine(MonitorAudioSource(availableSource));
    }

    private AudioSource GetAvailableAudioSource()
    {
        AudioSource availableSource = availableSources.Find(source => !source.isPlaying);

        return availableSource;
    }

    private IEnumerator MonitorAudioSource(AudioSource source)
    {
        // Kiểm tra âm thanh sau khi phát xong để trả lại vào pool
        yield return new WaitForSeconds(source.clip.length);

        // Sau khi âm thanh kết thúc, trả lại vào pool
        activeSources.Remove(source);
        availableSources.Add(source);
    }


    //public void PlaySpamAudio(AudioClip clip)
    //{
    //    AudioSource newSource = gameObject.AddComponent<AudioSource>();
    //    newSource.clip = clip;
    //    newSource.Play();
    //    spamAudios.Add(newSource);
    //    StartCoroutine(RemoveFinishSource());
    //}

    //private IEnumerator RemoveFinishSource()
    //{
    //    yield return new WaitForSeconds(1f);
    //    spamAudios.RemoveAll(source => !source.isPlaying);
    //}

    public void StopSound()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.loop = false;
            currentClip = null;
            //isLooping = false;
        }
    }

    public void ToggleLoop()
    {
        isLooping = !isLooping;

        if (isLooping)
        {
            // Nếu chưa có clip được gán, không làm gì
            if (currentClip != null && audioSource.isPlaying)
            {
                audioSource.loop = true;
            }
            else
            {
                Debug.LogWarning("No audio clip is set for looping.");
            }
        }
        else
        {
            StopLoop();
        }
    }

    public void ToggleSpam()
    {
        isSpamming = !isSpamming;
    }
    public void StopLoop()
    {
        audioSource.loop = false;
    
    }
    public bool IsLooping()
    {
        return isLooping;
    }
    public bool IsSpamming()
    {
        return isSpamming;
    }
}
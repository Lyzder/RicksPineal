using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-99)]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private MusicLooper mLooper;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource seSource;
    [SerializeField] SceneMusicMap sceneMusicMap;
    private AsyncOperationHandle<AudioClip>? currentClipHandle;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        mLooper = GetComponent<MusicLooper>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayBGM(string address, bool loop)
    {
        if (currentClipHandle.HasValue)
        {
            Addressables.Release(currentClipHandle.Value);
        }
        currentClipHandle = Addressables.LoadAssetAsync<AudioClip>(address);

        currentClipHandle.Value.Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                mLooper.SetClip(handle.Result, loop);
                mLooper.PlayClip();
            }
            else
            {
                Debug.LogError("Failed to load music: " + address);
            }
        };
    }

    public void PlayBGM(string address, int loopStart, int loopEnd, bool loop)
    {
        if (currentClipHandle.HasValue)
        {
            Addressables.Release(currentClipHandle.Value);
        }
        currentClipHandle = Addressables.LoadAssetAsync<AudioClip>(address);

        currentClipHandle.Value.Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                mLooper.SetClip(handle.Result, loopStart, loopEnd, loop);
                mLooper.PlayClip();
            }
            else
            {
                Debug.LogError("Failed to load music: " + address);
            }
        };
    }

    public void PlaySFX(AudioClip clip)
    {
        seSource.PlayOneShot(clip);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        mLooper.StopPlayback();
        SceneMusicEntry musicEntry = sceneMusicMap.GetMusicEntryForScene(scene.name);
        if (musicEntry != null)
        {
            if (musicEntry.customLoop)
            {
                PlayBGM(musicEntry.musicAddress, musicEntry.loopStartSample, musicEntry.loopEndSample, true);
            }
            else
            {
                PlayBGM(musicEntry.musicAddress, true);
            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
	public List<VideoClip> videos = null;

	public VideoEvent onPause = new VideoEvent();
	public VideoEvent onLoad = new VideoEvent();

	private bool isPaused = false;
	public bool IsPaused
	{
		get
		{
			return isPaused;
		}
		private set
		{
			isPaused = value;
			onPause.Invoke(isPaused);
		}
	}

	private bool isVideoReady = false;
	public bool IsVideoReady
	{
		get
		{
			return isVideoReady;
		}
		private set
		{
			isVideoReady = value;
			onLoad.Invoke(isVideoReady);
		}
	}

	private int index = 0;
	private VideoPlayer videoPlayer = null;

	public void Awake()
	{
		videoPlayer = GetComponent<VideoPlayer>();
		videoPlayer.seekCompleted += OnComplete;
		videoPlayer.prepareCompleted += OnComplete;
		videoPlayer.loopPointReached += OnLoop;
	}

	private void Start()
	{
		StartPrepare(index);
	}

	public void PauseToggle()
	{
		IsPaused = !videoPlayer.isPaused;

		print(IsPaused);

		if (IsPaused)
			videoPlayer.Pause();
		else
			videoPlayer.Play();
	}

	private void OnDestroy()
	{
		videoPlayer.seekCompleted -= OnComplete;
		videoPlayer.prepareCompleted -= OnComplete;
		videoPlayer.loopPointReached -= OnLoop;
	}

	public void SeekForward()
	{
		StartSeek(10.0f);
	}
	public void SeekBack()
	{
		StartSeek(-10.0f);
	}

	public void StartSeek(float seekAmount)
	{
		IsVideoReady = false;
		videoPlayer.time += seekAmount;
	}

	public void NextVideo()
	{
		index++;

		if (index == videos.Count)
			index = 0;

		StartPrepare(index);
	}
	public void PreviousVideo()
	{
		index--;

		if (index == -1)
			index = videos.Count - 1;

		StartPrepare(index);
	}

	private void StartPrepare(int clipIndex)
	{
		IsVideoReady = false;
		videoPlayer.clip = videos[clipIndex];
		videoPlayer.Prepare();
	}

	private void OnComplete(VideoPlayer videoPlayer)
	{
		IsVideoReady = true;
		videoPlayer.Play();
	}

	private void OnLoop(VideoPlayer videoPlayer)
	{
		NextVideo();
	}

	public class VideoEvent : UnityEvent<bool> { }
}

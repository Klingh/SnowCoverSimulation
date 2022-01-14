using UnityEngine;
using UnityEditor;
using TMPro;
using System.IO;
using System.Collections.Generic;

public class FrameRateCounter : MonoBehaviour
{

	[SerializeField]
	TextMeshProUGUI display = default;

	[SerializeField, Range(0.1f, 100f)]
	float sampleDuration = 1f;

	[SerializeField, Range(0.1f, 100f)]
	float totalTime = 100f;

	int frames;

	float duration;
	float totalDuration;
	float bestDuration = float.MaxValue;
	float worstDuration;

	List<float> best;
	List<float> worst;
	List<float> avg;

    private void Start()
    {
		best = new List<float>();
		worst = new List<float>();
		avg = new List<float>();
	}
    /// <summary>
    /// Calculates the best, average, and worst Framerate for the sample duration
    /// </summary>
    void Update()
	{
		if (totalDuration <= totalTime)
		{
			if (Time.fixedTime > 5.0f && duration <= sampleDuration)
			{
				float frameDuration = Time.unscaledDeltaTime;
				frames += 1;
				duration += frameDuration;
				totalDuration += frameDuration;

				if (frameDuration < bestDuration)
				{
					bestDuration = frameDuration;
				}
				if (frameDuration > worstDuration)
				{
					worstDuration = frameDuration;
				}

				if (duration >= sampleDuration)
				{

					display.SetText(
						"FPS\n Best: {0}\n Average: {1}\n Worst: {2}",
						1f / bestDuration,
						frames / duration,
						1f / worstDuration);

					best.Add(1f / bestDuration);
					worst.Add(1f / worstDuration);
					avg.Add(frames / duration);

					frames = 0;
					duration = 0f;
					bestDuration = float.MaxValue;
					worstDuration = 0f;
				}
			}
		}
		else
        {
			//Output data and exit application.
			OutputData();
			#if UNITY_EDITOR
						UnityEditor.EditorApplication.isPlaying = false;
			#else
					 Application.Quit();
			#endif
		}
	}

	/// <summary>
	/// Outputs the captured data into a txt file with a corresponding name.
	/// </summary>
	void OutputData()
    {
		//Output best FPS reached
        string path = "best.txt";

        StreamWriter writer = new StreamWriter(path, true);
		foreach(float i in best)
        {
			writer.WriteLine(i);
        }
        writer.Close();


		//Output average FPS  per frame
		path = "avg.txt";
		writer = new StreamWriter(path, true);
		foreach (float i in avg)
		{
			writer.WriteLine(i);
		}
		writer.Close();


		//Output worst FPS reached per frame
		path = "worst.txt";
		writer = new StreamWriter(path, true);
		foreach (float i in worst)
		{
			writer.WriteLine(i);
		}
		writer.Close();
	}
}
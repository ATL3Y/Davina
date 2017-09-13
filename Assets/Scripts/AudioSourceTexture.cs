using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceTexture : MonoBehaviour
{
    private int width; // texture width
    private int height; // texture height
    private Color backgroundColor = Color.black;
    //public Color waveformColor = Color.green;
    public int size = 1024; // size of sound segment displayed in texture

    private Color[] blank; // blank image array
    public Texture2D AudioTexture;
    public float[][] samples; // audio samples array
    public float[] lowRes;
    public int lowResSize;// = 256;

    public AudioSource[] audSources;
    private float amplify = 5f;

    public ComputeBuffer _buffer;

    void Start()
    {
        width = size;
        height = 1;

        //Make an array of the audioSources you want to include
        audSources = GetComponents<AudioSource>();

        // Create the array of samples arrays to align with the audSources
        samples = new float[audSources.Length][];
        for(int i=0; i < samples.Length; i++)
        {
            samples[i] = new float[size * 8];
        }

        lowRes = new float[64];
        lowResSize = 64;

        // Create the AudioTexture and assign to the guiTexture:
        AudioTexture = new Texture2D(width, height);
        

        // Create a 'blank screen' image
        blank = new Color[width * height];

        _buffer = new ComputeBuffer(size, 4 * sizeof(float));

        for (int i = 0; i < blank.Length; i++)
        {
            blank[i] = backgroundColor;
        }

        // Refresh the display each 100mS
    }

    void Update()
    {
        GetCurWave();
    }

    void GetCurWave()
    {
        // Clear the AudioTexture
        // AudioTexture.SetPixels (blank, 0);

        // Get samples from channel 0 (left)
        // GetComponent<AudioListener>().GetOutputData (samples, 0);

        for (int i = 0; i < audSources.Length; i++)
        {
            audSources[i].GetSpectrumData(samples[i], 0, FFTWindow.Triangle);
        }

        // Just add all samples into the first array
        for (int i = 1; i < samples.Length; i++)
        {
            for(int j = 0; j < samples[0].Length; j++)
            {
                samples[0][j] += samples[i][j];
            } 
        }

        Color[] pixels = AudioTexture.GetPixels(0, 0, width, 1);

        // Draw the waveform
        for (int i = 0; i < size; i++)
        {
            // Remember samples[0] is the sum of all samples 
            // og = pixels[i];//AudioTexture.GetPixel((int)(width * i / size), (int)(1 * (samples [i])) - 1 );
            pixels[i].r = pixels[i].r * .7f + samples[0][(int)(i * 4) + 0] * 128 * amplify;
            pixels[i].g = pixels[i].g * .7f + samples[0][(int)(i * 4) + 1] * 128 * amplify;
            pixels[i].b = pixels[i].b * .7f + samples[0][(int)(i * 4) + 2] * 128 * amplify;
            pixels[i].a = pixels[i].a * .7f + samples[0][(int)(i * 4) + 3] * 128 * amplify;

            // AudioTexture.SetPixel((int)(width * i / size), (int)(1 * (samples [i])) - 1, c );
        } 

        // Upload to the graphics card
        AudioTexture.SetPixels(pixels);
        AudioTexture.Apply();

        _buffer.SetData(samples[0]);
    }
}
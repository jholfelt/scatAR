using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class ConvolutionReverbUploadIR : MonoBehaviour
{
    [DllImport("AudioPluginDemo")]
    private static extern bool ConvolutionReverb_UploadSample(int index, float[] data, int numsamples, int numchannels, int samplerate, [MarshalAs(UnmanagedType.LPStr)] string name);

    public AudioClip[] impulse = new AudioClip[0];
    public int index;

    private bool[] uploaded = new bool [64];
    private AudioClip[] currImpulse = new AudioClip [64];

	public float[] IR;
	public float[] IR2;
	public float[] IR3;
	public float[] IR4;
	private string name = "ir";
	private string name2 = "ir2";
	private string name3 = "ir3";
	private string name4 = "ir4";
    
    void Start()
    {
        UploadChangedClips();
    }

    void Update()
    {
        UploadChangedClips();
    }
    
    void UploadChangedClips()
    {
		if (!gameObject.GetComponent<generateWGWIR> ().doUpload ()) {
			int currindex = index;
			foreach (var s in impulse) {
				if (currImpulse [currindex] != s)
					uploaded [currindex] = false;

				if (s != null && s.loadState == AudioDataLoadState.Loaded && !uploaded [currindex]) {
					Debug.Log ("Uploading impulse response " + s.name + " to slot " + currindex);
					float[] data = new float[s.samples];
					s.GetData (data, 0);
					ConvolutionReverb_UploadSample (currindex, data, data.Length / s.channels, s.channels, s.frequency, s.name);
					Debug.Log (currindex + "   " + data + "   " + data.Length / s.channels + "   " + s.channels + "   " + s.frequency + "   " + s.name);
					uploaded [currindex] = true;
					currImpulse [currindex] = s;
				}

				currindex++;
			}
		} else {
			Debug.Log ("Uploading impulse response ");
			IR = gameObject.GetComponent<generateWGWIR> ().getIR ();
			ConvolutionReverb_UploadSample (5, IR, IR.Length / 2, 2, 44100, name);
			IR2 = gameObject.GetComponent<generateWGWIR> ().getIR ();
			ConvolutionReverb_UploadSample (6, IR, IR.Length / 2, 2, 44100, name2);
			IR3 = gameObject.GetComponent<generateWGWIR> ().getIR ();
			ConvolutionReverb_UploadSample (7, IR, IR.Length / 1, 1, 44100, name3);
			IR4 = gameObject.GetComponent<generateWGWIR> ().getIR ();
			ConvolutionReverb_UploadSample (8, IR, IR.Length / 1, 1, 44100, name4);
			Debug.Log (IR.Length);
		}
	}
}

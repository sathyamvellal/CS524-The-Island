using LibPDBinding;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityMidi;
using AudioSynthesis.Midi;

public class AudioManager : MonoBehaviour {

    static string res;
    public const int MAXPNAMELEN = 32;
    public struct MidiOutCaps
    {
        public short wMid;
        public short wPid;
        public int vDriverVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXPNAMELEN)]
        public string szPname;
        public short wTechnology;
        public short wVoices;
        public short wNotes;
        public short wChannelMask;
        public int dwSupport;
    }
    static FileStream PdOutput;
    static int midiSize;
    static int midiCounter;
    static MidiPlayer mPlayer;

    // Update is called once per frame
    void Update() {

    }

    // MCI INterface
    [DllImport("winmm.dll")]
    private static extern long mciSendString(string command, StringBuilder returnValue, int returnLength, System.IntPtr winHandle);

    [DllImport("winmm.dll")]
    private static extern long mciGetErrorString(string error, string returnValue, int returnLength);

    // Midi API
    [DllImport("winmm.dll")]
    private static extern int midiOutGetNumDevs();


    [DllImport("winmm.dll")]
    private static extern int midiOutGetDevCaps(System.Int32 uDeviceID, ref MidiOutCaps lpMidiOutCaps, System.UInt32 cbMidiOutCaps);


    [DllImport("winmm.dll")]
    private static extern int midiOutOpen(ref int handle, int deviceID, MidiCallBack proc, int instance, int flags);

    [DllImport("winmm.dll")]
    private static extern int midiOutShortMsg(int handle, int message);

    [DllImport("winmm.dll")]
    private static extern int midiOutClose(int handle);

    private delegate void MidiCallBack(int handle, int msg, int instance, int param1, int param2);

    static string Mci(string command)
    {
        StringBuilder reply = new StringBuilder(256);
        mciSendString(command, reply, 256, System.IntPtr.Zero);
        return reply.ToString();
    }

    void Start()
    {
        var numDevs = midiOutGetNumDevs();
        MidiOutCaps myCaps = new MidiOutCaps();
        var res = midiOutGetDevCaps(0, ref myCaps, (System.UInt32)Marshal.SizeOf(myCaps));
        PureData.OpenPatch("IslandMusicPatch");
        mPlayer = GetComponent<MidiPlayer>();
        PlayMidi();
    }

    public static void receiveMidiByte(float value) {

        try
        {
            Debug.Log("C# MIDI: midi byte returned: " + value);
            byte[] s = new byte[1];
            s[0] = (byte)value;
            PdOutput.Write(s, 0, 1);
            midiCounter++;
        } catch (IOException)
        {
            Debug.Log("Error writing to file");
        }

        if (midiCounter >= midiSize)
        {
            Debug.Log("Loading MIDI from memory");
            //res = Mci("stop music");
            //res = Mci("open \"" + "temp.mid" + "\" alias music");
            //res = Mci("play music");
            /*string err = "";
            string errorMsg = "";
            mciGetErrorString(err, errorMsg, 1024);
            Debug.Log("Error: " + err + ", msg: " + errorMsg);*/
            MidiFile mfile = new MidiFile(PdOutput);
            mPlayer.LoadMidi(mfile);
            PlayAfterTime(3);
        }
       
    }

    static IEnumerator PlayAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        // Code to execute after the delay
        Debug.Log("Playing MIDI");
        mPlayer.Play();
    }

    static unsafe void PlayMidi()
    {
        //res = System.String.Empty;

        // set path to midi file here
        string filename = Application.dataPath + "/StreamingAssets/audio/" + "River Flows in You - Yiruma [MIDICollection.net].mid";
        //AudioManager.LoadMidiFile(filename);

        Debug.Log("Loading midi:" + filename);
        FileStream midiFileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None);

        //LibPD.WriteMessageToDebug = true;
        PdOutput = new FileStream("temp.mid", FileMode.Create, FileAccess.ReadWrite);
        int buffer;
        buffer = midiFileStream.ReadByte();
        PureData.Receive("mididata", new FloatReceiveCallback(AudioManager.receiveMidiByte), false);
        midiSize = 0;
        midiCounter = 0;
        while (buffer >= 0)
        {
            Debug.Log("Sending data: " + buffer);
            
            bool result = PureData.Send("midiIn", buffer);
            midiSize++;

            //yield return new WaitForSeconds(1);
            if (!result)
            {
                Debug.Log("Error sending MIDI!");
            }
            

            buffer = midiFileStream.ReadByte();
        }

        //Debug.Log("Loading MIDI from memory");
        //res = Mci("open \"" + "StreamingAssets/audio/" + "temp.mid" + "\" alias music");
        //res = Mci("play music");

        //Debug.Log("Original MIDI file.");
        //res = Mci("open \"" + filename + "\" alias music");
        //res = Mci("play music");
        /*MidiFile mfile = new MidiFile(midiFileStream);
         mPlayer.LoadMidi(mfile);
         mPlayer.Play();*/
    }

    void OnDestroy()
    {
        res = Mci("close music");
    }

    void OnDisable()
    {
        res = Mci("close music");
    }

}

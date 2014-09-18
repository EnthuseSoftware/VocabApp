using System;

public class AudioClipData
{
	public string Name = "Clip";
	public int Samples = 0;
	public int length = 0;
	public int Channels = 1;
	public int Frequency = 44100;
	public bool Sound3D = false;
	public bool Stream = false;
	public float[] AudioSamples;

	public void Compile (byte [] wav)
	{
		//to check if its a wave format first
		string type = BitConverter.ToString( wav, 8, 4 );
		if(!type.Equals("57-41-56-45"))
		{
			throw new NotSupportedException( "File: " + Name + ": Only WAV files are supported." );
		}

		//Make sure we have the correct number of channels
		Channels = BitConverter.ToInt16( wav, 22 );
		if ( Channels > 2 || Channels < 1)
		{
			throw new NotSupportedException( "File: " + Name + ": File formats with more than 2 channels or less then 1 is not supported." );
		}

		//Retrieve the frequency
		Frequency = BitConverter.ToInt32( wav, 24 );

		//Get number of bits per sample
		int bitsPerSample = BitConverter.ToInt16( wav, 34 );
		if ( bitsPerSample != 16 ) //Currently only works with 16
		{
			throw new NotSupportedException( 
				"File: " + Name + ": Only 16 bit WAV files are supported, this file is: " + bitsPerSample + " bits." 
				);
		}

		//Unity takes the number of frames instead of samples, so we need to do mathz to get it
		Int32 chunkSize2 = BitConverter.ToInt32( wav, 40 ); //The main data chunk
		int BytesPerSample = bitsPerSample / 8; //16 bit uses 2 bytes per, 32 is 4
		int bytesPerFrame = BytesPerSample * Channels; //Stereo vs Mono
		length = chunkSize2 / bytesPerFrame; //The final division to get the true length according to unity
		
		if ( bitsPerSample == 16 )
			Samples = wav.Length - 44; //The rest of the array besides the 44 byte header
		else
			//Wavs with more than 2 channels are pretty rare so just throw an exception (or if num channels was less than 1 for some reason)
			throw new NotSupportedException( "File: " + Name + ": This file format is not supported (too many channels or too few) Number of channels: " + Channels );
		
		//Create sampleblock
		Int16[] audioData = new short [ Samples ];
		//Data block
		byte[] block = new byte [ Samples ];
		for ( int i = 0; i < Samples; i++ )
		{
			block [ i ] = wav [ wav.Length - Samples + i ]; //Extract the bytes after the header
		}

		//Copy block over to audioData array
		Buffer.BlockCopy( block, 0, audioData, 0, Samples );
		block = null; //clear the block, we dont need it now

		//Convert int16 samples to floats samples
		AudioSamples = Int16ToFloats( audioData );
	}

	private static float [] Int16ToFloats ( Int16 [] array )
	{
		float[] floatArray = new float [ array.Length ];
		for ( int i = 0; i < floatArray.Length; i++ )
		{
			floatArray [ i ] = ( ( float ) array [ i ] / short.MaxValue );
		}
		return floatArray;
	}
}
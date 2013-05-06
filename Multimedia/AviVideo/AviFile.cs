using System;
using System.Collections.Generic;

namespace DeltaEngine.Multimedia.AviVideo
{
	public class AviFile
	{
		public AviFile(String fileName)
		{
			AviInterop.AVIFileInit();
			AviInterop.AVIFileOpen(ref aviFile, fileName, AviInterop.OfReadwrite, 0);
		}

		private readonly int aviFile;

		public VideoStream GetVideoStream()
		{
			IntPtr aviStream;
			int result = AviInterop.AVIFileGetStream(aviFile, out aviStream, AviInterop.StreamtypeVideo,
				0);
			if (result != 0)
				throw new Exception("Exception in AVIFileGetStream: " + AviErrors.GetError(result));

			var stream = new VideoStream(aviFile, aviStream);
			streams.Add(stream);
			return stream;
		}

		private readonly List<AviStream> streams = new List<AviStream>();

		public AudioStream GetAudioStream()
		{
			IntPtr aviStream;
			int result = AviInterop.AVIFileGetStream(aviFile, out aviStream, AviInterop.StreamtypeAudio,
				0);
			if (result != 0)
				throw new Exception("Exception in AVIFileGetStream: " + AviErrors.GetError(result));

			var stream = new AudioStream(aviFile, aviStream);
			streams.Add(stream);
			return stream;
		}

		public void Close()
		{
			foreach (AviStream stream in streams)
				stream.Close();

			AviInterop.AVIFileRelease(aviFile);
			AviInterop.AVIFileExit();
		}
	}
}
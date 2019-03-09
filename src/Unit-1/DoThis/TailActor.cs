using System.IO;
using System.Text;
using Akka.Actor;

namespace WinTail
{
    public class TailActor : UntypedActor
    {
        private readonly IActorRef _reporterActor;
        private readonly FileObserver _fileObserver;
        private readonly Stream _fileStream;
        private readonly StreamReader _fileStreamReader;

        public class FileWrite
        {
            public string FileName { get; }

            public FileWrite(string fileName)
            {
                FileName = fileName;
            }
        }

        public class FileError
        {
            public string FileName { get; }
            public string Reason { get; }

            public FileError(string fileName, string reason)
            {
                FileName = fileName;
                Reason = reason;
            }
        }

        public class InitialRead
        {
            public string FileName { get; }
            public string Text { get; }

            public InitialRead(string fileName, string text)
            {
                FileName = fileName;
                Text = text;
            }
        }
        
        public TailActor(IActorRef reporterActor, string filePath)
        {
            _reporterActor = reporterActor;

            _fileObserver = new FileObserver(Self, Path.GetFullPath(filePath));
            _fileObserver.Start();
            
            _fileStream = new FileStream(Path.GetFullPath(filePath), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _fileStreamReader = new StreamReader(_fileStream, Encoding.UTF8);

            var text = _fileStreamReader.ReadToEnd();
            Self.Tell(new InitialRead(filePath, text));
        }

        protected override void OnReceive(object message)
        {
            if (message is FileWrite fileWrite)
            {
                var text = _fileStreamReader.ReadToEnd();
                if (!string.IsNullOrEmpty(text))
                    _reporterActor.Tell(text);
            }
            else if (message is FileError fileError)
                _reporterActor.Tell($"Tail error: {fileError.Reason}");
            else if (message is InitialRead initialRead)
                _reporterActor.Tell(initialRead.Text);
        }
    }
}
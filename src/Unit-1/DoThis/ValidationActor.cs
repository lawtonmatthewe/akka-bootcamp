using Akka.Actor;

namespace WinTail
{
    public class ValidationActor : UntypedActor
    {
        private readonly IActorRef _consoleWriterActor;

        public ValidationActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }
        
        protected override void OnReceive(object message)
        {
            if (string.IsNullOrEmpty((string) message))
            {
                _consoleWriterActor.Tell(new Messages.NullInputError("No input received."));
            }
            else
            {
                var isValid = IsValid((string) message);
                if (isValid)
                    _consoleWriterActor.Tell(new Messages.InputSuccess("Thank you! Message was valid."));
                else
                    _consoleWriterActor.Tell(new Messages.InputError("Invalid: input had odd number of characters."));
            }
            
            Sender.Tell(new Messages.ContinueProcessing());
        }

        private static bool IsValid(string message)
        {
            return message.Length % 2 == 0;
        }
    }
}
using System;
using Akka.Actor;

namespace WinTail
{
    /// <summary>
    /// Actor responsible for reading FROM the console. 
    /// Also responsible for calling <see cref="ActorSystem.Terminate"/>.
    /// </summary>
    class ConsoleReaderActor : UntypedActor
    {
        private readonly IActorRef _validationActor;
        
        public const string StartCommand = "start";
        public const string ExitCommand = "exit";

        public ConsoleReaderActor(IActorRef validationActor)
        {
            _validationActor = validationActor;
        }

        protected override void OnReceive(object message)
        {
            if (message.Equals(StartCommand))
                DoPrintInstructions();

            GetAndValidateInput();
        }
        
        private void DoPrintInstructions()
        {
            Console.WriteLine("Please provide the URI of a log file on disk.\n");
        }

        private void GetAndValidateInput()
        {
            var message = Console.ReadLine();
            if (string.Equals(message, ExitCommand, StringComparison.OrdinalIgnoreCase))
                Context.System.Terminate();
            else
                _validationActor.Tell(message);
        }
    }
}
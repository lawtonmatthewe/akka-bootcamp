using System;
﻿using Akka.Actor;

namespace WinTail
{
    #region Program
    class Program
    {
        public static ActorSystem MyActorSystem;

        static void Main(string[] args)
        {
            MyActorSystem = ActorSystem.Create("MyActorSystem");

            var consoleWriterProps = Props.Create<ConsoleWriterActor>();
            var consoleWriterActor = MyActorSystem.ActorOf(consoleWriterProps);

            var tailCoordinatorProps = Props.Create<TailCoordinatorActor>();
            var tailCoordinatorActor = MyActorSystem.ActorOf(tailCoordinatorProps);

            var fileValidationProps = Props.Create<FileValidatorActor>(consoleWriterActor, tailCoordinatorActor);
            var fileValidationActor = MyActorSystem.ActorOf(fileValidationProps);
            
            var consoleReaderProps = Props.Create<ConsoleReaderActor>(fileValidationActor);
            var consoleReaderActor = MyActorSystem.ActorOf(consoleReaderProps);

            consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

            // blocks the main thread from exiting until the actor system is shut down
            MyActorSystem.WhenTerminated.Wait();
        }
    }
    #endregion
}

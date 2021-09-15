﻿using System;
using System.Collections.Generic;
using System.IO;

namespace tarsalgo
{
    class Program
    {

        static List<Event> Events { get; set; } = new List<Event>();
        static Dictionary<int, Entity> Entities { get; set; } = new Dictionary<int, Entity>();
        static Dictionary<int, TimeStamp> TimeStamps { get; set; } = new Dictionary<int, TimeStamp>();

        static void Main(string[] args)
        {
            // 1.
            ProcessFile(@"ajto.txt");

            // 2.
            Console.WriteLine("");
            Console.WriteLine("2. feladat");
            Console.WriteLine("Az első belépő: " + FindEntityFirstIn().Id);
            Console.WriteLine("Az utolsó kilépő: " + FindEntityLastOut().Id);


            // 3.
            WriteEntityAndNumOfEventsToFile();


            // 4.
            Console.WriteLine("");
            Console.WriteLine("4. feladat");
            Console.WriteLine("A végén a társalgóban voltak: ");
            Dictionary<int, Entity> entitiesStillIn = FindEntitiesStillIn();
            foreach(KeyValuePair<int, Entity> entity in entitiesStillIn)
            {
                Console.WriteLine(entity.Value.Id);
            }

            // 5.
            Console.WriteLine("");
            Console.WriteLine(FindTimeStampWithMostEntitiesIn().getTimeAsString() + "-kor voltak a legtöbben a társalgóban.");

            // 6.
            Console.WriteLine("");
            Console.WriteLine("6. feladat");
            Console.WriteLine("Adja meg a személy azonosítóját! ");
            int input = Int32.Parse(Console.ReadLine());

            // 7.
            Console.WriteLine("");
            Console.WriteLine("7. feladat");
            foreach(Event e in Entities[input].Events)
            {
                if(e.Direction == Direction.In)
                {
                    Console.Write(e.getTimeAsString() + "-");
                }
                else if (e.Direction == Direction.Out)
                {
                    Console.WriteLine(e.getTimeAsString());
                }
            }
            Console.WriteLine("");

            // 8.
            Console.WriteLine("");
            Console.WriteLine("8. feladat");
            TimeSpan totalTimeInOfEntity = Entities[input].CalculateTotalTimeIn();
            if (!entitiesStillIn.ContainsKey(input))
            {
                Console.WriteLine("A(z) " + input + "-es személy összesen " + totalTimeInOfEntity.TimeInMinutes + " percet volt bent, a megfigyelés végén nem volt a társalgóban.");
            }
            else
            {
                Console.WriteLine("A(z) " + input + "-es személy összesen " + totalTimeInOfEntity.TimeInMinutes + " percet volt bent, a megfigyelés végén a társalgóban volt.");
            }
        }

        private static void ProcessFile(string path)
        {
            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                Event @event = new(line);
                Events.Add(@event);

                if (!Entities.ContainsKey(@event.EntityId))
                {
                    Entity entity = new(@event.EntityId);
                    Entities[@event.EntityId] = entity;
                }

                if (!TimeStamps.ContainsKey(@event.TimeInMinutes))
                {
                    TimeStamp timeStamp = new(@event.TimeInMinutes);
                    TimeStamps[@event.TimeInMinutes] = timeStamp;
                }

                Entities[@event.EntityId].Events.Add(@event);
                TimeStamps[@event.TimeInMinutes].Events.Add(@event);
             
                if (@event.Direction == Direction.In)
                {
                    TimeStamps[@event.TimeInMinutes].EntitiesInside.Add(@event.EntityId, Entities[@event.EntityId]);
                }
                else if (@event.Direction == Direction.Out)
                {
                    TimeStamps[@event.TimeInMinutes].EntitiesInside.Remove(@event.EntityId);
                }
            
            }
        }

        public static Entity FindEntityFirstIn()
        {
            Entity entityFirstIn = new();
            foreach (Event e in Events)
            {
                if (e.Direction == Direction.In)
                {
                    entityFirstIn = Entities[e.EntityId];
                    break;
                }
            }
            return entityFirstIn;
        }

        public static Entity FindEntityLastOut()
        {
            Entity entityLastOut = new();
            foreach (Event e in Events)
            {
                if (e.Direction == Direction.Out)
                {
                    entityLastOut = Entities[e.EntityId];
                }
            }
            return entityLastOut;
        }

        public static async void WriteEntityAndNumOfEventsToFile()
        {
            List<string> lines = new();

            foreach (KeyValuePair<int, Entity> entity in Entities)
            {
                lines.Add(entity.Value.Id.ToString() + " " + entity.Value.Events.Count.ToString());
            }

            await File.WriteAllLinesAsync("athaladas.txt", lines.ToArray());
        }

        public static Dictionary<int, Entity> FindEntitiesStillIn()
        {
            Dictionary<int, Entity> entitiesStillIn = new();
            TimeStamp lastTimeStamp = new();

            foreach(KeyValuePair<int, TimeStamp> timeStamp in TimeStamps)
            {
                if (timeStamp.Value.TimeInMinutes > lastTimeStamp.TimeInMinutes)
                {
                    entitiesStillIn = timeStamp.Value.EntitiesInside;
                }
            }

            return entitiesStillIn;
        }

        public static TimeStamp FindTimeStampWithMostEntitiesIn()
        {
            TimeStamp timeStampWithMostEntitiesIn = new();
            foreach (KeyValuePair<int, TimeStamp> timeStamp in TimeStamps)
            {
                if (timeStamp.Value.EntitiesInside.Count > timeStampWithMostEntitiesIn.EntitiesInside.Count)
                {
                    timeStampWithMostEntitiesIn = TimeStamps[timeStamp.Value.TimeInMinutes];
                }
            }

            return timeStampWithMostEntitiesIn;
        }
    }
}

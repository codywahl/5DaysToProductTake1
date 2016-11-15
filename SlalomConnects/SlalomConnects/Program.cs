using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SlalomConnects
{
    internal class Program
    {
        private static List<EventSeeker> _eventSeekers;

        private static void Main(string[] args)
        {
            _eventSeekers = new List<EventSeeker>();
            string menuInput;

            do
            {
                Console.Write("Add a event seeker? Y/N: ");
                menuInput = Console.ReadLine();

                if (menuInput.ToLower().Equals("y"))
                {
                    AddEventSeeker();
                }
            } while (!menuInput.ToLower().Equals("n"));

            foreach (var eventSeeker in _eventSeekers)
            {
                Console.WriteLine(eventSeeker.ToString());
            }

            var match = SeekerMatcher.FindSeekerMatch(_eventSeekers);

            if (match == null)
            {
                Console.WriteLine("No match found.");
            }
            else
            {
                Console.WriteLine("Match Found! Person 1: " + match.Item1.Email + " Person 2:" + match.Item2.Email);
            }

            Console.ReadLine();
        }

        private static void AddEventSeeker()
        {
            Console.Write("Email: ");
            var email = Console.ReadLine();

            Console.WriteLine("Date Format: DD/MM/YYYY HH:MM");
            Console.Write("What time can you start?: ");
            var startTimeInput = Console.ReadLine();
            DateTime startTime;
            DateTime.TryParse(startTimeInput, out startTime);
#if DEBUG
            Console.WriteLine("DEBUG: (startTime) " + startTime + Environment.NewLine);
#endif

            Console.Write("What time do you want to end?: ");
            var endTimeInput = Console.ReadLine();
            DateTime endTime;
            DateTime.TryParse(endTimeInput, out endTime);
#if DEBUG
            Console.WriteLine("DEBUG: (endTime) " + endTime + Environment.NewLine);
#endif

            var newEventSeeker = new EventSeeker
            {
                Email = email,
                StartTime = startTime,
                EndTime = endTime
            };

            _eventSeekers.Add(newEventSeeker);
        }
    }

    public class EventSeeker
    {
        public string Email { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public override string ToString()
        {
            return "Email: " + Email + " StartTime: " + StartTime + " EndTime: " + EndTime;
        }
    }

    public static class SeekerMatcher
    {
        public static Tuple<EventSeeker, EventSeeker> FindSeekerMatch(List<EventSeeker> eventSeekers)
        {
            if (eventSeekers.Count < 2)
            {
                return null;
            }

            foreach (var eventSeeker1 in eventSeekers)
            {
                foreach (var eventSeeker2 in eventSeekers)
                {
                    if (eventSeeker1 == eventSeeker2) continue;
                    
                    var timesMatch = (
                    eventSeeker1.StartTime >= eventSeeker2.StartTime && eventSeeker1.EndTime <= eventSeeker2.EndTime);

                    if (timesMatch)
                    {
                        return new Tuple<EventSeeker, EventSeeker>(eventSeeker1, eventSeeker2);
                    }
                }
            }

            return null;
        }
    }
}
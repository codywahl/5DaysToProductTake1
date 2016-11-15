using System;
using System.Collections.Generic;

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
                Console.Write("Add an event seeker? Y/N: ");
                menuInput = Console.ReadLine();

                if (menuInput.ToLower().Equals("y"))
                {
                    AddEventSeeker();
                }
            } while (!menuInput.ToLower().Equals("n"));

            var eventItem = SeekerMatcher.FindSeekerMatch(_eventSeekers);

            if (eventItem == null)
            {
                Console.WriteLine("No match found.");
            }
            else
            {
                Console.WriteLine("Match Found!" + Environment.NewLine +
                                  "Meet on floor 15, near the shuffleboard, at " + eventItem.StartDate + " for " + eventItem.EventType + " until " + eventItem.EndDate);
                Console.WriteLine("You'll be joining...");

                foreach (var eventSeeker in eventItem.EventSeekers)
                {
                    Console.WriteLine(eventSeeker.Email);
                }
            }

            Console.ReadLine();
        }

        private static void AddEventSeeker()
        {
            var newEventSeeker = new EventSeeker();

            Console.Write("Email: ");
            newEventSeeker.Email = Console.ReadLine();

            var validEntry = false;

            do
            {
                Console.Write("Event type? 1:Lunch 2:Coffee 3:PingPong : ");
                var eventChoiceInput = Console.ReadLine();

                if (string.Equals(eventChoiceInput, "1") || string.Equals(eventChoiceInput, "2") || string.Equals(eventChoiceInput, "3"))
                {
                    validEntry = true;
                    newEventSeeker.EventType = (EventType)Convert.ToInt32(eventChoiceInput) - 1;
                }
            } while (!validEntry);

            Console.WriteLine("Date Format: DD/MM/YYYY HH:MM");
            Console.Write("What time can you start?: ");
            var startTimeInput = Console.ReadLine();
            DateTime startTime;
            DateTime.TryParse(startTimeInput, out startTime);
            newEventSeeker.StartTime = startTime;
#if DEBUG
            Console.WriteLine("DEBUG: (startTime) " + startTime + Environment.NewLine);
#endif

            Console.Write("What time do you want to end?: ");
            var endTimeInput = Console.ReadLine();
            DateTime endTime;
            DateTime.TryParse(endTimeInput, out endTime);
            newEventSeeker.EndTime = endTime;
#if DEBUG
            Console.WriteLine("DEBUG: (endTime) " + endTime + Environment.NewLine);
#endif

            _eventSeekers.Add(newEventSeeker);
        }
    }

    public class EventSeeker
    {
        public string Email { get; set; }
        public EventType EventType;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public override string ToString()
        {
            return "Email: " + Email + " Event Type: " + EventType + " StartTime: " + StartTime + " EndTime: " + EndTime;
        }
    }

    public class EventDetails
    {
        public Guid EventId { get; set; }
        public int MinimumGroupSize { get; set; }
        public int MaximumGroupSize { get; set; }
        public List<EventSeeker> EventSeekers { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public EventType EventType { get; set; }

        public EventDetails()
        {
            EventSeekers = new List<EventSeeker>();
        }
    }

    public enum EventType
    {
        Lunch,
        Coffee,
        PingPong
    }

    public static class SeekerMatcher
    {
        private const int LunchBufferTime = 45;
        private const int CoffeeBufferTime = 20;
        private const int PingPongBufferTime = 15;

        public static EventDetails FindSeekerMatch(List<EventSeeker> eventSeekers)
        {
            if (eventSeekers.Count < 2)
            {
                return null;
            }

            foreach (var eventSeeker1 in eventSeekers)
            {
                foreach (var eventSeeker2 in eventSeekers)
                {
                    // Check easy disqualifying factors.
                    if (eventSeeker1 == eventSeeker2) continue;

                    if (eventSeeker1.EventType != eventSeeker2.EventType) continue;

                    if (eventSeeker1.StartTime >= eventSeeker2.EndTime || eventSeeker1.EndTime <= eventSeeker2.StartTime) continue;

                    // Determine possible start time.
                    DateTime possibleStartTime;
                    if (eventSeeker1.StartTime >= eventSeeker2.StartTime)
                    {
                        possibleStartTime = eventSeeker1.StartTime;
                    }
                    else
                    {
                        possibleStartTime = eventSeeker2.StartTime;
                    }

                    // Determine possible end time.
                    DateTime possibleEndTime;
                    if (eventSeeker1.EndTime <= eventSeeker2.EndTime)
                    {
                        possibleEndTime = eventSeeker1.EndTime;
                    }
                    else
                    {
                        possibleEndTime = eventSeeker2.EndTime;
                    }

                    // Set Buffer based on event type
                    var bufferTime = Int32.MaxValue;
                    if (eventSeeker1.EventType == EventType.Lunch)
                    {
                        bufferTime = LunchBufferTime;
                    }
                    else if (eventSeeker1.EventType == EventType.Coffee)
                    {
                        bufferTime = CoffeeBufferTime;
                    }
                    else if (eventSeeker1.EventType == EventType.PingPong)
                    {
                        bufferTime = PingPongBufferTime;
                    }

                    if (possibleStartTime.AddMinutes(bufferTime) <= possibleEndTime)
                    {
                        eventSeekers.Remove(eventSeeker1);
                        eventSeekers.Remove(eventSeeker2);

                        var eventDetails = new EventDetails()
                        {
                            StartDate = possibleStartTime,
                            EndDate = possibleEndTime,
                            EventType = eventSeeker1.EventType
                        };

                        eventDetails.EventSeekers.Add(eventSeeker1);
                        eventDetails.EventSeekers.Add(eventSeeker2);

                        return eventDetails;
                    }
                }
            }

            return null;
        }
    }
}
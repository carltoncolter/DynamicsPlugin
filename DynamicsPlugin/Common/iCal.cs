using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;

namespace Plugins.Calendar
{
    public class iCal
    {
        public iCal()
        {
            EventTimeStamp = DateTime.Now;
            EventCreatedDateTime = EventTimeStamp;
            EventLastModifiedTimeStamp = EventTimeStamp;
        }

        public DateTime EventCreatedDateTime { get; set; }
        public DateTime EventEndDateTime { get; set; }
        public DateTime EventLastModifiedTimeStamp { get; set; }
        public DateTime EventStartDateTime { get; set; }
        public DateTime EventTimeStamp { get; set; }
        public string AlarmDescription { get; set; }
        public string AlarmDuration { get; set; }
        public string AlarmRepeat { get; set; }
        public string AlarmTrigger { get; set; }
        public string Application { get; set; }
        public string Company { get; set; }
        public string EventDescription { get; set; }
        public string EventLocation { get; set; }
        public string EventSummary { get; set; }
        public string Organizer { get; set; }
        public string OrganizerEmail { get; set; }
        public string UID { get; set; }

        public static List<string> ToStringList(List<iCal> iCals) => iCals.Select(i => i.ToString()).ToList();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            //Calendar
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("PRODID:-//" + Company + "//" + Application + "//EN");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("METHOD:REQUEST");
            //Event
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine("DTSTART:" + toUniversalTime(EventStartDateTime));
            sb.AppendLine("DTEND:" + toUniversalTime(EventEndDateTime));
            sb.AppendLine("DTSTAMP:" + toUniversalTime(EventTimeStamp));
            sb.AppendLine("ORGANIZER;CN=" + Organizer + ":" + OrganizerEmail);
            sb.AppendLine("UID:" + UID);
            //sb.AppendLine("ATTENDEE;CUTYPE=INDIVIDUAL;ROLE=REQ-PARTICIPANT;PARTSTAT=NEEDS-ACTION;RSVP=TRUE;CN=marydoe@company.com;X-NUM-GUESTS=0:mailto:marydoe@company.com");
            sb.AppendLine("CREATED:" + toUniversalTime(EventCreatedDateTime));
            sb.AppendLine("X-ALT-DESC;FMTTYPE=text/html:" + EventDescription);
            sb.AppendLine("LAST-MODIFIED:" + toUniversalTime(EventLastModifiedTimeStamp));
            sb.AppendLine("LOCATION:" + EventLocation);
            sb.AppendLine("SEQUENCE:0");
            sb.AppendLine("STATUS:CONFIRMED");
            sb.AppendLine("SUMMARY:" + EventSummary);
            sb.AppendLine("TRANSP:OPAQUE");
            //Alarm
            sb.AppendLine("BEGIN:VALARM");
            sb.AppendLine("TRIGGER:" + string.Format("-PT{0}M", AlarmTrigger));
            sb.AppendLine("REPEAT:" + AlarmRepeat);
            sb.AppendLine("DURATION:" + string.Format("PT{0}M", AlarmDuration));
            sb.AppendLine("ACTION:DISPLAY");
            sb.AppendLine("DESCRIPTION:" + AlarmDescription);
            sb.AppendLine("END:VALARM");
            sb.AppendLine("END:VEVENT");
            sb.AppendLine("END:VCALENDAR");
            return sb.ToString();


        }

        public string ToBase64String(out int length)
        {
            var data = Encoding.ASCII.GetBytes(ToString());
            length = data.Length;

            return Convert.ToBase64String(data);
        }

        public Entity ToAnnotation(EntityReference parentObject, EntityReference owner)
        {
            return new Entity("annotation")
            {
                ["ownerid"] = owner,
                ["objecttypecode"] = parentObject.LogicalName,
                ["objectid"] = parentObject,
                ["subject"] = "visitEvent.ics",
                ["mimetype"] = "text/Calendar",
                ["notetext"] = "eFolder Document",
                ["filename"] = "visitEvent.ics",
                ["documentbody"] = ToBase64String(out int length)
            };
        }

        public Entity ToEmailAttachment(EntityReference email)
        {
            var data = ToBase64String(out int length);

            return new Entity("activitymimeattachment")
            {
                ["subject"] = "visitEvent.ics",
                ["filename"] = "visitEvent.ics",
                ["body"] = data,
                ["filesize"] = length,
                ["mimetype"] = "text/plain",
                ["attachmentnumber"] = 1,
                ["objectid"] = email,
                ["objecttypecode"] = email.LogicalName
            };
        }

        public static string toUniversalTime(DateTime dt) => dt.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
    }
}
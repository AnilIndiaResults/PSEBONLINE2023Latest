using System;

namespace FilterSkipping.Models
{
    public class BaseModel
    {
        private DateTime _meetingDate;

        public bool ModelAlterationPerformed { get; set; }
        public DateTime MeetingDate
        {
            get
            {
                if (_meetingDate == default(DateTime))
                {
                    _meetingDate = new DateTime(2014, 1, 1);
                }
                return _meetingDate;
            }
            set
            {
                _meetingDate = value;
            }
        }


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlassixSharp.Users.Models
{
    public class UserInfoData
    {
        public Guid userId;
        public string userName;
        public List<UserDateData> dates;

        public UserInfoData(Guid userId, string userName, List<UserDateData> dates)
        {
            this.userId = userId;
            this.userName = userName;
            this.dates = dates;
        }
        public class UserDateData
        {
            public DateTime date;
            public TimeSpan totalOnlineTime;
            public TimeSpan totalBreakTime;
            public TimeSpan totalBreak2Time;
            public TimeSpan totalBreak3Time;
            public TimeSpan totalBreak4Time;
            public TimeSpan totalBreak5Time;
            public TimeSpan totalOfflineTime;
            public List<UserStatusLog> statusLogs;

            public UserDateData(DateTime date, TimeSpan totalOnlineTimesSec, TimeSpan totalBreakTimesSec, TimeSpan totalBreak2TimesSec, TimeSpan totalBreak3TimesSec, TimeSpan totalBreak4TimesSec, TimeSpan totalBreak5TimesSec, TimeSpan totalOfflineTimesSec, List<UserStatusLog> userStatusLogs)
            {
                this.date = date;
                this.totalOnlineTime = totalOnlineTimesSec;
                this.totalBreakTime = totalBreakTimesSec;
                this.totalBreak2Time = totalBreak2TimesSec;
                this.totalBreak3Time = totalBreak3TimesSec;
                this.totalBreak4Time = totalBreak4TimesSec;
                this.totalBreak5Time = totalBreak5TimesSec;
                this.totalOfflineTime = totalOfflineTimesSec;
                this.statusLogs = userStatusLogs;
            }

            public class UserStatusLog
            {
                public DateTime dateTime;
                public string nextStatus;
                public bool doNotDisturb;

                public UserStatusLog(DateTime statusChangeTimeStamp, string statusSet, bool doNotDisturb)
                {
                    this.dateTime = statusChangeTimeStamp;
                    this.nextStatus = statusSet;
                    this.doNotDisturb = doNotDisturb;

                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlassixSharp.Users.Models
{
    public class UserInfoData
    {
        public Guid userId { get; set; }
        public string userName { get; set; }
        public List<UserDateData> dates { get; set; }

        public UserInfoData()
        {
            // Parameterless constructor for deserialization
        }

        public UserInfoData(Guid userId, string userName, List<UserDateData> dates)
        {
            this.userId = userId;
            this.userName = userName;
            this.dates = dates;
        }

        public class UserDateData
        {
            public DateTime date { get; set; }
            public TimeSpan totalOnlineTime { get; set; }
            public TimeSpan totalBreakTime { get; set; }
            public TimeSpan totalBreak2Time { get; set; }
            public TimeSpan totalBreak3Time { get; set; }
            public TimeSpan totalBreak4Time { get; set; }
            public TimeSpan totalBreak5Time { get; set; }
            public TimeSpan totalOfflineTime { get; set; }
            public List<UserStatusLog> statusLogs { get; set; }

            public UserDateData()
            {
                // Parameterless constructor for deserialization
            }

            public UserDateData(DateTime date, TimeSpan totalOnlineTime, TimeSpan totalBreakTime,
                                TimeSpan totalBreak2Time, TimeSpan totalBreak3Time, TimeSpan totalBreak4Time,
                                TimeSpan totalBreak5Time, TimeSpan totalOfflineTime, List<UserStatusLog> statusLogs)
            {
                this.date = date;
                this.totalOnlineTime = totalOnlineTime;
                this.totalBreakTime = totalBreakTime;
                this.totalBreak2Time = totalBreak2Time;
                this.totalBreak3Time = totalBreak3Time;
                this.totalBreak4Time = totalBreak4Time;
                this.totalBreak5Time = totalBreak5Time;
                this.totalOfflineTime = totalOfflineTime;
                this.statusLogs = statusLogs;
            }

            public class UserStatusLog
            {
                public DateTime dateTime { get; set; }
                public string nextStatus { get; set; }
                public bool doNotDisturb { get; set; }

                public UserStatusLog()
                {
                    // Parameterless constructor for deserialization
                }

                public UserStatusLog(DateTime dateTime, string nextStatus, bool doNotDisturb)
                {
                    this.dateTime = dateTime;
                    this.nextStatus = nextStatus;
                    this.doNotDisturb = doNotDisturb;
                }
            }
        }
    }
}
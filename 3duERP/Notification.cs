using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace _3duERP
{
    public class Notification
    {
        public void RegisterNotification(DateTime CurrentTime)
        {
            string conString = ConfigurationManager.ConnectionStrings["conString"].ConnectionString;
            string SQLCommand = @"SELECT [ContactID],[ContactName],[ContactNo],[AddedOn] FROM [dbo].[Contacts] WHERE [AddedOn]>@AddedOn";
            using (SqlConnection con= new SqlConnection(conString))
            {
                using (SqlCommand cmd=new SqlCommand(SQLCommand, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@AddedOn",CurrentTime);
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    cmd.Notification = null;
                    SqlDependency SQLDep = new SqlDependency(cmd);
                    SQLDep.OnChange += SQLDep_OnChange;
                    using (SqlDataReader SQLReader = cmd.ExecuteReader())
                    {

                    }
                }

            }
        }

        private void SQLDep_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                SqlDependency _SqlDependency = sender as SqlDependency;
                _SqlDependency.OnChange -= SQLDep_OnChange;

                //send notification to client
                var _PushNotification = GlobalHost.ConnectionManager.GetHubContext<PushNotification>();
                _PushNotification.Clients.All.notify("added");
                //re register notification

            }
        }
    }
}
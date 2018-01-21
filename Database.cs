using System;
using System.Collections.Generic;
using System.Data.SQLite; //Install System.Data.SQLite.Core 1.0.106
using System.IO;
using WebApplication.Controllers;

namespace WebApplication
{
    public class Database
    {
         private SQLiteConnection dbConn;
        
        //Opens the connection and creates the table if it does not exist
        public Database(string name)
        {
            if (!File.Exists(name + ".db"))
            {
                connect(name);
                string sql = "CREATE TABLE Applications (user_id STRING, company_name STRING, email_id STRING, PRIMARY KEY (user_id,company_name,email_id))";
                SQLiteCommand command = new SQLiteCommand(sql, dbConn);
                command.ExecuteNonQuery();
                sql = "CREATE TABLE Emails (email_id STRING, sender STRING, internal_time INTEGER, status STRING, PRIMARY KEY (email_id))";
                command = new SQLiteCommand(sql, dbConn);
                command.ExecuteNonQuery();
            }
            else
            {
                connect(name);
            }
        }

        private void connect(string name)
        {
            dbConn = new SQLiteConnection("Data Source=" + name + ".db;Version=3;UseUTF8Encoding=True;");
            dbConn.Open();
        }

        //For a wordphrase with a pos tag, inserts its url into the database, if it already exists it updates the url
        public void newApplication(string user_id, string company_name, string email_id)
        {
            string sql = "INSERT INTO Applications (user_id, company_name, email_id) VALUES (\'" + user_id + "\',\'" + company_name + "\',\'" +
                         email_id + "\')";
            SQLiteCommand command = new SQLiteCommand(sql, dbConn);
            command.ExecuteNonQuery();
        }

        public void updateApplicationEmailId(string user_id, string company_name, string email_id)
        {
            string sql = "UPDATE Applications SET email_id=\"" + email_id + "\" WHERE company_name=\"" + company_name + "\" AND user_id=\"" + user_id +
                         "\";";
            SQLiteCommand command = new SQLiteCommand(sql, dbConn);
            command.ExecuteNonQuery();
        }

        public void newEmail(string email_id, string sender, long? internal_time, string status)
        {
            string sql = "INSERT INTO Emails (email_id, sender, internal_time, status) VALUES (\'" + email_id + "\',\'" + sender + "\'," +
                         internal_time + ",\'" + status + "\')";
            SQLiteCommand command = new SQLiteCommand(sql, dbConn);
            command.ExecuteNonQuery();
        }

        public List<string> getEmailIds()
        {
            List<string> emailIds = new List<string>();
            string sql = "SELECT email_id FROM Emails";
            SQLiteCommand command = new SQLiteCommand(sql,dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                emailIds.Add(reader["email_id"].ToString());   
            }
            return emailIds;
        }

        public List<string> getCompanyNames()
        {
            List<string> companyNames = new List<string>();
            string sql = "SELECT company_name FROM Applications";
            SQLiteCommand command = new SQLiteCommand(sql,dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                companyNames.Add(reader["company_name"].ToString());   
            }
            return companyNames;
        }

        public CompanyAndStatus getCompanyAndStatus()
        {
            CompanyAndStatus compAndStatus = new CompanyAndStatus();
            compAndStatus.list = new Dictionary<string, string>();
            string sql =
                "SELECT Applications.company_name, Emails.status FROM Applications INNER JOIN Emails ON Applications.email_id=Emails.email_id";
            SQLiteCommand command = new SQLiteCommand(sql,dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                compAndStatus.list.Add(reader["company_name"].ToString(),reader["status"].ToString());
            }
            return compAndStatus;
        }
    }
}
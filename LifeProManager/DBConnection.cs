/// <file>DBConnection.cs</file>
/// <author>Laurent Barraud, David Rossy and Julien Terrapon</author>
/// <version>1.8.2</version>
/// <date>May 29th, 2026</date>

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;


namespace LifeProManager
{
    /// This class uses a singleton‑like architecture: one static SQLiteConnection is created
    /// and kept open for the entire lifetime of the application.
    /// Using a single shared connection avoids file locking issues, concurrent access problems, and unnecessary
    /// reconnections, which maximizes stability when working with SQLite.
    public class DBConnection
    {
        // Declaration of a private attribute of type SQLiteConnection
        private static SQLiteConnection sqliteConn;

        // Constructor
        public DBConnection()
        {
            string dbPath = Path.Combine(Application.StartupPath, "LPM_DB.db");

            if (sqliteConn == null)
            {
                sqliteConn = new SQLiteConnection($"Data Source={dbPath}; Version=3; Compress=True;");
                sqliteConn.Open();
            }

            else if (sqliteConn.State == ConnectionState.Closed || sqliteConn.State == ConnectionState.Broken) 
            { 
                sqliteConn.Open(); 
            }
        }

        /// <summary>
        /// Approves a task, given by its id, with the status "done" in the database
        /// </summary>
        /// <param name="id">The id of the task</param>
        /// <param name="validationDate">The date when the task status was set to done</param>
        public void ApproveTask(int id, string validationDate)
        {
            // the id of value 2 is for "done" status
            SQLiteCommand cmd = sqliteConn.CreateCommand();
            string createSql = "UPDATE Tasks " +
                               "SET validationDate = '" + validationDate + "', " +
                               "Status_id = " + 2 + " " +
                               "WHERE id = " + id + ";";
            cmd.CommandText = createSql;
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Checks the database integrity
        /// </summary>
        /// <returns>The status of the database. 
        /// True means correct, false means corrupted.</returns>
        public bool CheckDBIntegrity()
        {
            // Checks the database integrity
            try
            {
                // Tries to do a transaction and at once rolls it back
                using (var transaction = sqliteConn.BeginTransaction())
                {
                    transaction.Rollback();
                }
            }

            // If the database is corrupted an error is generated
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates the database file in the app's installation folder
        /// </summary>
        public void CreateFile()
        {
            SQLiteConnection.CreateFile(@Environment.CurrentDirectory + "\\" + "LPM_DB.db");
        }

        /// <summary>
        /// Creates all database tables and inserts the initial data.
        /// This method is used when the database is missing or considered invalid.
        /// </summary>
        public void CreateTablesAndInsertInitialData()
        {
            using (SQLiteCommand cmd = sqliteConn.CreateCommand())
            {
                cmd.CommandText =
                @"
                    BEGIN TRANSACTION;

                    DROP TABLE IF EXISTS Status;
                    CREATE TABLE IF NOT EXISTS Status (
                        id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        denomination VARCHAR(50) NOT NULL
                    );

                    DROP TABLE IF EXISTS Priorities;
                    CREATE TABLE IF NOT EXISTS Priorities (
                        id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        denomination VARCHAR(50) NOT NULL
                    );

                    DROP TABLE IF EXISTS Lists;
                    CREATE TABLE IF NOT EXISTS Lists (
                        id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        title VARCHAR(50) NOT NULL
                    );

                    DROP TABLE IF EXISTS Tasks;
                    CREATE TABLE IF NOT EXISTS Tasks (
                        id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        title VARCHAR(70) NOT NULL,
                        description VARCHAR(500) DEFAULT NULL,
                        deadline DATE DEFAULT NULL,
                        validationDate DATE DEFAULT NULL,
                        Priorities_id INTEGER NOT NULL,
                        Lists_id INTEGER NOT NULL,
                        Status_id INTEGER NOT NULL,
                        FOREIGN KEY(Status_id) REFERENCES Status(id),
                        FOREIGN KEY(Priorities_id) REFERENCES Priorities(id),
                        FOREIGN KEY(Lists_id) REFERENCES Lists(id)
                    );

                    INSERT INTO Priorities(id, denomination) VALUES (0, '');
                    INSERT INTO Priorities(id, denomination) VALUES (1, 'Important');
                    INSERT INTO Priorities(id, denomination) VALUES (2, 'Repeatable');
                    INSERT INTO Priorities(id, denomination) VALUES (3, 'ImportantAndRepeatable');
                    INSERT INTO Priorities(id, denomination) VALUES (4, 'Birthday');

                    INSERT INTO Status (id, denomination) VALUES (1,'Open');
                    INSERT INTO Status (id, denomination) VALUES (2,'Done');

                    COMMIT;
                    ";

                cmd.ExecuteNonQuery();
            }
        }

        public int CountTotalTasksToComplete()
        {
            SQLiteCommand cmd = sqliteConn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Tasks WHERE Status_id = 1";

            int TotalTasksToComplete = Convert.ToInt32(cmd.ExecuteScalar());
            return TotalTasksToComplete;
        }

        /// <summary>
        /// Deletes all done tasks in the database
        /// </summary>
        public void DeleteAllDoneTasks()
        {
            SQLiteCommand cmd = sqliteConn.CreateCommand();
            string createSql = "Delete from Tasks WHERE Status_id = " + 2 + ";";
            cmd.CommandText = createSql;
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes a task, given by its id, in the database
        /// </summary>
        /// <param name="id">The id of the task to delete</param>
        public void DeleteTask(int id)
        {
            SQLiteCommand cmd = sqliteConn.CreateCommand();
            string createSql = "Delete from Tasks " +
                               "WHERE id = " + id + ";";
            cmd.CommandText = createSql;
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes a topic, given by its id, from the database
        /// </summary>
        /// <param name="id">The id number of the task</param>
        public void DeleteTopic(int id)
        {
            SQLiteCommand cmd = sqliteConn.CreateCommand();
            string createSql = "Delete from Tasks " +
                               "WHERE Lists_id = " + id + "; " +
                               "Delete from Lists " +
                               "WHERE id = " + id + ";";
            cmd.CommandText = createSql;
            cmd.ExecuteNonQuery();
        }


        /// <summary>
        /// Edits a task in the database
        /// </summary>
        public void EditTask(int id, string title, string description, string deadline, int priorities_id, int lists_id)
        {
            SQLiteCommand cmd = sqliteConn.CreateCommand();
            string createSql = "UPDATE Tasks " +
                               "SET title = '" + title.Replace("'", "''") + "', " +
                               "description = '" + description.Replace("'", "''") + "', " +
                               "deadline = '" + deadline + "', " +
                               "Priorities_id = " + priorities_id + ", " +
                               "Lists_id = " + lists_id + " " +
                               "WHERE id = " + id + ";";
            cmd.CommandText = createSql;
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Executes a raw SQL command on the shared single SQLite connection
        /// </summary>
        public void ExecuteRawSql(string sql)
        {
            using (SQLiteCommand cmd = sqliteConn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Inserts a task into the database
        /// </summary>
        /// <param name="title">The title of the task</param>
        /// <param name="description">The description of the task</param>
        /// <param name="deadline">The date for which the task is due</param>
        /// <param name="priorities_id">The level of priority for the task</param>
        /// <param name="lists_id">The id of the list to which the task was assigned</param>
        /// <param name="status_id">The id of the status to which the task was assigned</param>
        public void InsertTask(string title, string description, string deadline, int priorities_id, int lists_id, int status_id)
        {
            SQLiteCommand cmd = sqliteConn.CreateCommand();
            string createSql = "INSERT INTO Tasks VALUES(NULL, '" + title + "', '" + description + "', '" + deadline + "', NULL, " + priorities_id + ", " + lists_id + ", " + status_id + ")";
            cmd.CommandText = createSql;
            cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// Inserts a topic in the database
        /// </summary>
        public void InsertTopic(String title)
        {
            SQLiteCommand cmd = sqliteConn.CreateCommand();
            string createSql = "INSERT INTO Lists VALUES(NULL, '" + title.Replace("'", "''") + "')";
            cmd.CommandText = createSql;
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Extracts the finished tasks from the database
        /// </summary>
        /// <returns>Taskslist containing the result of the request</returns>
        public List<Tasks> ReadApprovedTask()
        {
            // Status "done" (2)
            return ReadTask("WHERE Status_id = " + 2 + " ;");
        }

        /// <summary>
        /// Reads and return the data of the table for all days that have deadlines assigned to one or more task(s) 
        /// of priority different from 4, as we don't want the birthdays to appear
        /// </summary>
        /// <returns>Tasklist containing the result of the request</returns>
        public List<string> ReadDataForDeadlines()
        {
            SQLiteCommand cmd = sqliteConn.CreateCommand();
            // Gets the list of the deadlines.
            // Since we only want the ones with status "To complete" (1), we add it here in the condition.
            cmd.CommandText = "SELECT DISTINCT deadline FROM Tasks WHERE Status_id = 1 AND Priorities_id != 4;";

            // Declaration and instanciation of the list of DateTime
            List<string> deadlinesList = new List<string>();

            // Declaration of a SQLiteDataReader object which contains the results list
            SQLiteDataReader dataReader = cmd.ExecuteReader();

            // Browses the results list
            while (dataReader.Read())
            {
                // Reads the value of the deadline column from the database and allocating it to a string variable
                string myReader = dataReader["deadline"].ToString();

                // Adds the values of the column deadline into the reader object
                deadlinesList.Add(myReader);

            }
            // Returns the list when it's built 
            return deadlinesList;
        }

        /// <summary>
        /// Reads the status denominations from the database
        /// </summary>
        /// <returns>Statuslist containing the result of the request</returns>
        public List<string> ReadStatusDenomination()
        {
            SQLiteCommand cmd = sqliteConn.CreateCommand();
            cmd.CommandText = "SELECT denomination FROM Status";
            List<string> statusList = new List<string>();
            SQLiteDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                string myReader = dataReader["denomination"].ToString();
                statusList.Add(myReader);
            }
            return statusList;
        }

        /// <summary>
        /// Extracts tasks from the database using an optional WHERE condition.
        /// </summary>
        /// <param name="whereCondition">
        /// SQL WHERE clause.
        /// If empty or null, all tasks are returned.
        /// </param>
        /// <returns>List of tasks matching the condition</returns>
        public List<Tasks> ReadTask(string whereCondition, List<SQLiteParameter> sqlParams = null)
        {
            if (sqlParams == null)
            {
                sqlParams = new List<SQLiteParameter>();
            }

            SQLiteCommand cmd = sqliteConn.CreateCommand();

            string strSql = "SELECT id, title, description, deadline, validationDate, Priorities_id, Lists_id, Status_id FROM Tasks ";

            if (!string.IsNullOrWhiteSpace(whereCondition))
            {
                strSql += " " + whereCondition;
            }

            else
            {
                strSql += " WHERE Status_id = 1 ";
            }

            cmd.CommandText = strSql;

            if (sqlParams != null && sqlParams.Count > 0)
            {
                cmd.Parameters.AddRange(sqlParams.ToArray());
            }

            List<Tasks> tasksList = new List<Tasks>();

            using (SQLiteDataReader dataReader = cmd.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    Tasks currentTask = new Tasks();

                    if (int.TryParse(dataReader["id"].ToString(), out int foundId))
                    {
                        currentTask.Id = foundId;
                    }

                    if (int.TryParse(dataReader["Priorities_id"].ToString(), out int foundPrioritiesId))
                    {
                        currentTask.Priorities_id = foundPrioritiesId;
                    }

                    if (int.TryParse(dataReader["Lists_id"].ToString(), out int foundListsId))
                    {
                        currentTask.Lists_id = foundListsId;
                    }

                    if (int.TryParse(dataReader["Status_id"].ToString(), out int foundStatusId))
                    {
                        currentTask.Status_id = foundStatusId;

                        // Only tasks with status "done" (2) have a validation date
                        if (foundStatusId == 2)
                        {
                            currentTask.ValidationDate = dataReader["validationDate"].ToString();
                        }
                    }

                    currentTask.Title = dataReader["title"].ToString();
                    currentTask.Description = dataReader["description"].ToString();
                    currentTask.Deadline = dataReader["deadline"].ToString();

                    tasksList.Add(currentTask);
                }
            }

            return tasksList;
        }

        /// <summary>
        /// Reads a single task from the database, given by its id
        /// </summary>
        /// <param name="idTask">The id of the task to read</param>
        public Tasks ReadTaskById(int idTask)
        {
            List<Tasks> taskFound = ReadTask("WHERE id = " + idTask, new List<SQLiteParameter>());

            if (taskFound.Count > 0)
            {
                return taskFound[0];
            }

            return null;
        }

        /// <summary>
        /// Extracts the tasks from the database for a specified day, given in argument.
        /// Shows overdue tasks only when the selected date is today.
        /// </summary>
        /// <param name="selectedDate">The date whose tasks are to be read (format yyyy-MM-dd)</param>
        /// <returns>Taskslist containing the result of the request</returns>
        public List<Tasks> ReadTaskForDate(string selectedDate)
        {
            bool isTodaySelected = (selectedDate == DateTime.Today.ToString("yyyy-MM-dd"));

            string sqlWhereCondition;

            if (isTodaySelected)
            {
                // Today selected: shows today's tasks, overdue tasks, and birthdays
                sqlWhereCondition =
                "WHERE Status_id = 1 " +
                "AND (" +
                "    deadline = @date " +
                "    OR deadline < date('now') " +
                "    OR (Priorities_id = 4 " +
                     "AND SUBSTR(deadline, 6, 5) = SUBSTR(@date, 6, 5) " +
                     "AND SUBSTR(deadline, 1, 4) = SUBSTR(@date, 1, 4) " +
                "    )" +
                ") " +
                "ORDER BY Priorities_id DESC;";
            }
            else
            {
                // Other day selected: shows only tasks for that day and birthdays
                sqlWhereCondition =
                "WHERE Status_id = 1 " +
                "AND (" +
                "    deadline = @date " +
                "    OR (Priorities_id = 4 " +
                "    AND SUBSTR(deadline, 6, 5) = SUBSTR(@date, 6, 5) " +
                "    AND SUBSTR(deadline, 1, 4) = SUBSTR(@date, 1, 4) " +
                "    )" +
                ") " +
                "ORDER BY Priorities_id DESC;";
            }

            // Builds the final SQL command
            SQLiteCommand cmd = sqliteConn.CreateCommand();
            cmd.CommandText = "SELECT id, title, description, deadline, validationDate, Priorities_id, Lists_id, Status_id FROM Tasks " + sqlWhereCondition;
            cmd.Parameters.AddWithValue("@date", selectedDate);

            // Executes and read results
            List<Tasks> tasksList = new List<Tasks>();

            using (SQLiteDataReader dataReader = cmd.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    Tasks currentTask = new Tasks();

                    if (int.TryParse(dataReader["id"].ToString(), out int id))
                    {
                        currentTask.Id = id;
                    }

                    if (int.TryParse(dataReader["Priorities_id"].ToString(), out int priorities_id))
                    {
                        currentTask.Priorities_id = priorities_id;
                    }

                    if (int.TryParse(dataReader["Lists_id"].ToString(), out int lists_id))
                    {
                        currentTask.Lists_id = lists_id;
                    }

                    if (int.TryParse(dataReader["Status_id"].ToString(), out int status_id))
                    {
                        currentTask.Status_id = status_id;

                        if (status_id == 2)
                        {
                            currentTask.ValidationDate = dataReader["validationDate"].ToString();
                        }
                    }

                    currentTask.Title = dataReader["title"].ToString();
                    currentTask.Description = dataReader["description"].ToString();
                    currentTask.Deadline = dataReader["deadline"].ToString();

                    tasksList.Add(currentTask);
                }
            }

            return tasksList;
        }

        /// <summary>
        /// Extracts the tasks for the next 7 days from the database 
        /// </summary>
        /// <returns>Taskslist containing the result of the request</returns>
        /// <param name="deadline">The date whose tasks are to be read</param>
        public List<Tasks> ReadTaskForDatePlusSeven(string[] nextSevenDays)
        {
            return ReadTask(
                "WHERE Status_id = 1 " +
                "AND (" +
                "    deadline IN ('" + nextSevenDays[0] + "', '" + nextSevenDays[1] + "', '" + nextSevenDays[2] + "', '" +
                                 nextSevenDays[3] + "', '" + nextSevenDays[4] + "', '" + nextSevenDays[5] + "', '" + nextSevenDays[6] + "') " +
                "    OR (" +
                "        Priorities_id = 4 " + // birthdays tasks
                "        AND SUBSTR(deadline, 6, 5) IN (" +
                "            SUBSTR('" + nextSevenDays[0] + "', 6, 5), " +
                "            SUBSTR('" + nextSevenDays[1] + "', 6, 5), " +
                "            SUBSTR('" + nextSevenDays[2] + "', 6, 5), " +
                "            SUBSTR('" + nextSevenDays[3] + "', 6, 5), " +
                "            SUBSTR('" + nextSevenDays[4] + "', 6, 5), " +
                "            SUBSTR('" + nextSevenDays[5] + "', 6, 5), " +
                "            SUBSTR('" + nextSevenDays[6] + "', 6, 5)" +
                "        )" +
                "    )" +
                ") " +
                "ORDER BY Priorities_id DESC;"
            );
        }
        /// <summary>
        /// Extracts the tasks from the database for a specified topic, given in argument by its Id
        /// </summary>
        /// <returns>Taskslist containing the result of the request</returns>
        /// <param name="topicId">The id of the topic whose tasks are to be read</param>
        public List<Tasks> ReadTaskForTopic(int topicId)
        {
            //Since we only want the status "To complete" (1) we add it here in the condition
            return ReadTask("WHERE Lists_id = " + topicId + " AND Status_id = 1 ORDER BY Priorities_id DESC;");
        }

        /// <summary>
        /// Reads the topics from the database
        /// </summary>
        /// <returns>Topiclist containing the result of the request</returns>
        public List<Lists> ReadTopics()
        {
            SQLiteCommand cmd = sqliteConn.CreateCommand();
            cmd.CommandText = "SELECT id, title FROM Lists";
            List<Lists> topicList = new List<Lists>();
            SQLiteDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                Lists currentList = new Lists();

                int id;

                if (int.TryParse(dataReader["id"].ToString(), out id))
                {
                    currentList.Id = id;
                }

                currentList.Title = dataReader["title"].ToString();

                topicList.Add(currentList);
            }
            return topicList;
        }
     
        /// <summary>
        /// Reads given topic id and returns the name of that topic
        /// </summary>
        /// <returns>The name of the topic</returns>
        public string ReadTopicName(int listId)
        {
            SQLiteCommand cmd = sqliteConn.CreateCommand();
            
            // Gets the name of the topic by its id
            cmd.CommandText = "SELECT title FROM Lists WHERE id = '" + listId + "';";

            // Declaration of a SQLiteDataReader object which contains the results list
            SQLiteDataReader dataReader = cmd.ExecuteReader();

            string nameTopic = "";

            // Browses the results list
            while (dataReader.Read())
            {
                nameTopic = dataReader["title"].ToString();
            }

            return nameTopic;
        }

        /// <summary>
        /// Retrieves all tasks matching the given SQL WHERE condition,
        /// used by the SmartSearch pipeline.
        /// </summary>
        /// <param name="whereCondition">The SQL condition without the WHERE keyword.</param>
        /// <returns>List of tasks matching the condition.</returns>
        public List<Tasks> SearchTasks(string whereCondition, List<SQLiteParameter> parameters)
        {
            string sqlCondition = string.Empty;

            if (string.IsNullOrWhiteSpace(whereCondition) == false)
            {
                sqlCondition = " WHERE " + whereCondition;
            }

            return ReadTask(sqlCondition, parameters);
        }

        /// <summary>
        /// Unapprove a task
        /// </summary>
        /// <param name="id">The id of the task to unapprove</param>
        public void UnapproveTask(int id)
        {
            // the id of value 1 is for "To do" status
            SQLiteCommand cmd = sqliteConn.CreateCommand();
            string createSql = "UPDATE Tasks " +
                               "SET validationDate = NULL, " +
                               "Status_id = " + 1 + " " +
                               "WHERE id = " + id + ";";
            cmd.CommandText = createSql;
            cmd.ExecuteNonQuery();
        }
    }
}

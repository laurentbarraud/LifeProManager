/// <file>DBConnection.cs</file>
/// <author>Laurent Barraud, David Rossy and Julien Terrapon</author>
/// <version>1.8.3</version>
/// <date>July 5th, 2026</date>

using System;
using System.Data;
using Microsoft.Data.Sqlite;

namespace LifeProManager
{
    /// This class uses a singleton‑like architecture: one static SQLiteConnection is created
    /// and kept open for the entire lifetime of the application.
    /// Using a single shared connection avoids file locking issues, concurrent access problems, and unnecessary
    /// reconnections, which maximizes stability when working with SQLite.
    public class DBConnection
    {
        // Declaration of a private attribute of type SQLiteConnection
        private static SqliteConnection sqliteConn = null!;

        // Constructor
        public DBConnection()
        {
            string dbFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "LifeProManager");

            Directory.CreateDirectory(dbFolder);

            string dbPath = Path.Combine(dbFolder, "LPM_DB.db");

            // Toujours ouvrir la connexion : SQLite crée le fichier si nécessaire
            if (sqliteConn == null)
            {
                sqliteConn = new SqliteConnection($"Data Source={dbPath}");
                sqliteConn.Open();
            }
            else if (sqliteConn.State != ConnectionState.Open)
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
            SqliteCommand cmd = sqliteConn.CreateCommand();
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
            string dbPath = Path.Combine(Application.StartupPath, "LPM_DB.db");

            if (!File.Exists(dbPath))
            {
                using var conn = new SqliteConnection($"Data Source={dbPath}");
                conn.Open();
            }
        }

        /// <summary>
        /// Creates all database tables and inserts the initial data.
        /// This method is used when the database is missing or considered invalid.
        /// </summary>
        public void CreateTablesAndInsertInitialData()
        {
            using (SqliteCommand cmd = sqliteConn.CreateCommand())
            {
                cmd.CommandText =
                @"
                    BEGIN TRANSACTION;

                    DROP TABLE IF EXISTS Status;
                    CREATE TABLE Status (
                        id INTEGER PRIMARY KEY,
                        denomination TEXT NOT NULL
                    );

                    DROP TABLE IF EXISTS Priorities;
                    CREATE TABLE Priorities (
                        id INTEGER PRIMARY KEY,
                        denomination TEXT NOT NULL
                    );

                    DROP TABLE IF EXISTS Lists;
                    CREATE TABLE Lists (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        title TEXT NOT NULL
                    );

                    DROP TABLE IF EXISTS Tasks;
                    CREATE TABLE Tasks (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        title TEXT NOT NULL,
                        description TEXT DEFAULT NULL,
                        deadline TEXT DEFAULT NULL,
                        validationDate TEXT DEFAULT NULL,
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

                    INSERT INTO Status(id, denomination) VALUES (1, 'Open');
                    INSERT INTO Status(id, denomination) VALUES (2, 'Done');

                    COMMIT;";

                cmd.ExecuteNonQuery();
            }
        }

        public int CountTotalTasksToComplete()
        {
            SqliteCommand cmd = sqliteConn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Tasks WHERE Status_id = 1";

            int TotalTasksToComplete = Convert.ToInt32(cmd.ExecuteScalar());
            return TotalTasksToComplete;
        }

        /// <summary>
        /// Deletes all done tasks in the database
        /// </summary>
        public void DeleteAllDoneTasks()
        {
            SqliteCommand cmd = sqliteConn.CreateCommand();
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
            SqliteCommand cmd = sqliteConn.CreateCommand();
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
            SqliteCommand cmd = sqliteConn.CreateCommand();
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
            SqliteCommand cmd = sqliteConn.CreateCommand();
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
        /// Executes a raw SQL script on the shared SQLite connection.
        /// </summary>
        public void ExecuteRawSql(string sqlStr)
        {
            using (var cmd = sqliteConn.CreateCommand())
            {
                cmd.CommandText = sqlStr;
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
            SqliteCommand cmd = sqliteConn.CreateCommand();
            string createSql = "INSERT INTO Tasks VALUES(NULL, '" + title + "', '" + description + "', '" + deadline + "', NULL, " + priorities_id + ", " + lists_id + ", " + status_id + ")";
            cmd.CommandText = createSql;
            cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// Inserts a topic in the database
        /// </summary>
        public void InsertTopic(String title)
        {
            SqliteCommand cmd = sqliteConn.CreateCommand();
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
        public List<string?> ReadDataForDeadlines()
        {
            SqliteCommand cmd = sqliteConn.CreateCommand();
            // Gets the list of the deadlines.
            // Since we only want the ones with status "To complete" (1), we add it here in the condition.
            cmd.CommandText = "SELECT DISTINCT deadline FROM Tasks WHERE Status_id = 1 AND Priorities_id != 4;";

            // Declaration and instanciation of the list of DateTime
            List<string?> deadlinesList = new List<string?>();

            // Declaration of a SQLiteDataReader object which contains the results list
            SqliteDataReader dataReader = cmd.ExecuteReader();

            // Browses the results list
            while (dataReader.Read())
            {
                // Reads the value of the deadline column from the database and allocating it to a string variable
                string? myReader = dataReader["deadline"].ToString();

                // Adds the values of the column deadline into the reader object
                deadlinesList.Add(myReader);

            }
            // Returns the list when it's built 
            return deadlinesList;
        }

        public List<Tasks> ReadTask(string whereCondition, List<SqliteParameter>? sqlParams = null)
        {
            // Always not-null to avoid null reference exceptions when adding parameters to the command
            sqlParams ??= new List<SqliteParameter>();

            using var cmd = sqliteConn.CreateCommand();

            var strSql = "SELECT id, title, description, deadline, validationDate, " +
                         "Priorities_id, Lists_id, Status_id FROM Tasks ";

            if (!string.IsNullOrWhiteSpace(whereCondition))
            {
                strSql += " " + whereCondition;
            }

            else
            {
                strSql += " WHERE Status_id = 1 ";
            }

            cmd.CommandText = strSql;

            if (sqlParams.Count > 0)
            {
                // Adds the parameters to the command if any are provided,
                // to prevent SQL injection and allow for parameterized queries
                cmd.Parameters.AddRange(sqlParams.ToArray());
            }

            var tasksList = new List<Tasks>();

            using var reader = cmd.ExecuteReader();
            
            while (reader.Read())
            {
                var currentTask = new Tasks
                {
                    Id = reader.GetInt32(0),
                    Title = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    Deadline = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    ValidationDate = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    Priorities_id = reader.GetInt32(5),
                    Lists_id = reader.GetInt32(6),
                    Status_id = reader.GetInt32(7)
                };

                tasksList.Add(currentTask);
            }

            return tasksList;
        }

        /// <summary>
        /// Reads a single task from the database, given by its id
        /// </summary>
        /// <param name="idTask">The id of the task to read</param>
        public Tasks? ReadTaskById(int idTask)
        {
            List<Tasks> taskFound = ReadTask("WHERE id = " + idTask, new List<SqliteParameter>());

            if (taskFound.Count > 0)
            {
                return taskFound[0];
            }

            return null;
        }

        /// <summary>
        /// Reads the tasks from the database for a specified date, given in argument by its string representation
        /// </summary>
        /// <param name="selectedDate"></param>
        /// <returns>The list of tasks for the specified date</returns>

        public List<Tasks> ReadTaskForDate(string selectedDate)
        {
            bool isTodaySelected = (selectedDate == DateTime.Today.ToString("yyyy-MM-dd"));

            string sqlWhereCondition;

            if (isTodaySelected)
            {
                sqlWhereCondition = "WHERE Status_id = 1 " +
                                    "AND (" +
                                    "    deadline = @date " +
                                    "    OR deadline < date('now') " +
                                    "    OR (Priorities_id = 4 " +
                                    "        AND SUBSTR(deadline, 6, 5) = SUBSTR(@date, 6, 5) " +
                                    "        AND SUBSTR(deadline, 1, 4) = SUBSTR(@date, 1, 4) " +
                                    "    )" +
                                    ") " +
                                    "ORDER BY Priorities_id DESC;";
            }
            else
            {
                sqlWhereCondition = "WHERE Status_id = 1 " +
                                    "AND (" +
                                    "    deadline = @date " +
                                    "    OR (Priorities_id = 4 " +
                                    "        AND SUBSTR(deadline, 6, 5) = SUBSTR(@date, 6, 5) " +
                                    "        AND SUBSTR(deadline, 1, 4) = SUBSTR(@date, 1, 4) " +
                                    "    )" +
                                    ") " +
                                    "ORDER BY Priorities_id DESC;";
            }

            using var cmd = sqliteConn.CreateCommand();
            cmd.CommandText = "SELECT id, title, description, deadline, validationDate, Priorities_id, Lists_id, Status_id " +
                              "FROM Tasks " + sqlWhereCondition;

            cmd.Parameters.AddWithValue("@date", selectedDate);

            var tasksList = new List<Tasks>();

            using var reader = cmd.ExecuteReader();
            
            while (reader.Read())
            {
                var currentTask = new Tasks
                {
                    Id = reader.GetInt32(0),
                    Title = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    Deadline = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    ValidationDate = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    Priorities_id = reader.GetInt32(5),
                    Lists_id = reader.GetInt32(6),
                    Status_id = reader.GetInt32(7)
                };

                tasksList.Add(currentTask);
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
            var topicList = new List<Lists>();

            using var cmd = sqliteConn.CreateCommand();
            cmd.CommandText = "SELECT id, title FROM Lists;";

            using var reader = cmd.ExecuteReader();
            
            while (reader.Read())
            {
                var currentList = new Lists
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1)
                };

                topicList.Add(currentList);
            }

            return topicList;
        }

        /// <summary>
        /// Reads given topic id and returns the name of that topic
        /// </summary>
        /// <returns>The name of the topic</returns>
        public string? ReadTopicName(int listId)
        {
            SqliteCommand cmd = sqliteConn.CreateCommand();
            
            // Gets the name of the topic by its id
            cmd.CommandText = "SELECT title FROM Lists WHERE id = '" + listId + "';";

            // Declaration of a SQLiteDataReader object which contains the results list
            SqliteDataReader dataReader = cmd.ExecuteReader();

            string? nameTopic = "";

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
        public List<Tasks> SearchTasks(string whereCondition, List<SqliteParameter> parameters)
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
            SqliteCommand cmd = sqliteConn.CreateCommand();
            string createSql = "UPDATE Tasks " +
                               "SET validationDate = NULL, " +
                               "Status_id = " + 1 + " " +
                               "WHERE id = " + id + ";";
            cmd.CommandText = createSql;
            cmd.ExecuteNonQuery();
        }
    }
}

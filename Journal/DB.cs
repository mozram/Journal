using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal
{
    public class DB
    {
        //Global variable for absolute path of db
        static string _db_path;
        public static string db_path
        {
            get { return _db_path; }
            set { _db_path = value; }
        }

        static string __DBconnectionString;
        public static string _DBconnectionString
        {
            get { return __DBconnectionString; }
            set { __DBconnectionString = value; }
        }

        //private static string _DBconnectionString = "Data Source=pnewels.db;Version=3;";// Password=lol;";

        //non-static members
        private SQLiteConnection __conn = null;

        public DB()
        {
            __conn = new SQLiteConnection(_DBconnectionString);
            try
            {
                __conn.Open();//_conn.ChangePassword("lol");
            }
            catch (SQLiteException)
            {
                __conn = null;
            }
        }

        /// <summary>
        /// Check database connectivity.
        /// </summary>
        /// <returns>Return TRUE if connection established. Otherwise, FALSE</returns>
        public static bool isDbOK()
        {

            bool _dbok = false;

            using (SQLiteConnection _conn = new SQLiteConnection(_DBconnectionString))
            {
                try
                {
                    _conn.Open();//_conn.ChangePassword("lol");
                    _dbok = true;

                }
                catch (SQLiteException) { }
            }
            return _dbok;
        }


        /// <summary>
        /// Execute SQL query returning number of rows affected. Only for INSERT, UPDATE and DELETE
        /// </summary>
        /// <param name="sql">the sql query to be executed</param>
        /// <returns>int - number of rows affected</returns>
        public static int Query(string sql)
        {
            int retval = -1;

            using (SQLiteConnection _conn = new SQLiteConnection(_DBconnectionString))
            {
                try
                {
                    _conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, _conn))
                    {
                        retval = cmd.ExecuteNonQuery();
                    }
                }
                catch (SQLiteException ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "DB error INT", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }

            return retval;
        }


        /// <summary>
        /// Execute SQL reader returning number of rows affected. Only for SELECT
        /// </summary>
        /// <param name="sql">the sql query to be executed</param>
        /// <returns>int - number of rows affected</returns>
        public static int Reader(string sql)
        {
            int retval = 0;

            using (SQLiteConnection _conn = new SQLiteConnection(_DBconnectionString))
            {
                try
                {
                    _conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, _conn))
                    {
                        SQLiteDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            retval++;
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "DB error reading", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }

            return retval;
        }


        /// <summary>
        /// Execute SQL SELECT returning first row, first column
        /// </summary>
        /// <param name="sql">the query to be executed</param>
        /// <returns>reader -1 on error or integer</returns>
        public static object QueryScalar(string sql)
        {
            int retval = -1;

            using (SQLiteConnection _conn = new SQLiteConnection(_DBconnectionString))
            {
                try
                {
                    _conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, _conn))
                    {
                        object x = cmd.ExecuteScalar();

                        if (x != null)
                        {
                            return x;
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "DB error OBJECT", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }

            return retval;
        }


        /// <summary>
        /// Execute SQL query returning records available
        /// e.g.
        /// rdr = .queryresult(...)
        /// while (rdr.Read())
        /// {
        ///     Console.WriteLine(rdr[0]+" -- "+rdr[1]);
        /// }
        /// rdr.Close();
        /// </summary>
        /// <param name="sql">the query to be executed</param>
        /// <returns>reader object to .read() results and to .close() once done</returns>
        public SQLiteDataReader QueryResult(string sql)
        {
            if (__conn == null)
                return null;

            SQLiteCommand cmd = new SQLiteCommand(sql, __conn);
            return cmd.ExecuteReader();
        }


        public static DataSet QueryDataset(string sql)
        {
            DataSet ds = new DataSet();

            using (SQLiteConnection _conn = new SQLiteConnection(_DBconnectionString))
            {
                try
                {
                    _conn.Open();
                    using (SQLiteDataAdapter fda = new SQLiteDataAdapter(sql, _conn))
                    {
                        fda.Fill(ds);
                    }
                }
                catch (SQLiteException)
                {
                    return null;
                }
            }

            return ds;
        }

        /// <summary>
        /// Check for database. If not available create new one.
        /// </summary>
        /// <param name="path">Database path</param>
        public static void CreateDbIfNotExists(string path)
        {
            if (!File.Exists(path))
            {
                SQLiteConnection.CreateFile(path);
                SQLiteConnection conn = new SQLiteConnection("Data Source=" + path + ";Version=3;");
                //encrypt the db using 128bit RSA
                //conn.SetPassword("beus385v9b8esuarfa8hhk8j42tjg82x");
                conn.Open();
                conn.Close();
                Query("CREATE TABLE `journal` (`date` TEXT, `content` TEXT, PRIMARY KEY(date))");
                //create default username 
                // Query("INSERT INTO `users` (login, pass) values ('admin', 'sha1:64000:18:jC+Gk4Av6/oOp27W7d0nzNkG8BRzdskA:n/XOD6J8xyauPuBbLVw5gB2v')");
                // Query("INSERT INTO `users` (login, pass) values ('operator', 'sha1:64000:18:hfh7KYsQBzdR4Qe8aM38QJjuMOGk1tXQ:tbDQ+dMeCgu/4M/NLJ/wQUIY')");

                //create directory
                //Directory.CreateDirectory(path + "\\data");
                //Directory.CreateDirectory(path + "\\log");
            }


        }
    }
}

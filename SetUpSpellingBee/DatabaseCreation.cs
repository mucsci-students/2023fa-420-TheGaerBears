using System;

public class DatabaseCreation
{
	public DatabaseCreation()
	{

    }
    //Starting with a single JSON file right now
    string cs = @"URI=file:";

    //Creating a SQLite connection object
    using var con = new SQLiteConnection(cs);
    //Open connection
    con.Open();

    //SQLiteCommmand is a obj used to execute query on db
    //param is connection object
    using var cmd = new SQLiteCommand(con);

    //If table already exists, do not create
    cmd.CommandText = "DROP TABLE IF EXISTS cars";
    //We do not want a result set for DROP/INSERT/DELETE statements.
    cmd.ExecuteNonQuery();

    cmd.CommandText = @"CREATE TABLE four letter words(id INTEGER PRIMARY KEY,
                word TEXT)";
    cmd.ExecuteNonQuery();

    cmd.CommandText = "INSERT INTO four letter words(word) VALUES(@word)";

    cmd.Parameters.AddWithValue("@word", "rock");
    cmd.Prepare();

    cmd.ExecuteNonQuery();

    Console.WriteLine("row inserted");
}

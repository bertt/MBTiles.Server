using System.Data.SQLite;
using System.Drawing;
using System.IO;

namespace MBTilesServer.Models
{
    public class MBTileProvider
    {
        private readonly string _connectionString;

        public MBTileProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Image GetTile(string level, int col, int row)
        {
            var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            Image image=null;
            using (var command = new SQLiteCommand(connection))
            {
                command.CommandText = "SELECT [tile_data] FROM [tiles] WHERE zoom_level = @zoom AND tile_column = @col AND tile_row = @row";
                command.Parameters.Add(new SQLiteParameter("zoom", level));
                command.Parameters.Add(new SQLiteParameter("col", col));
                command.Parameters.Add(new SQLiteParameter("row", row));
                var tileObj = command.ExecuteScalar();
                if (tileObj != null)
                {
                    var stream = new MemoryStream((byte[])tileObj);
                    var bitmap = new Bitmap(stream);
                    image = bitmap;
                }
            }
            connection.Close();
            return image;
        }

    }
}
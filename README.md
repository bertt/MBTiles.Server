MBTiles.Server
==============

Gets MBTiles as System.Drawing.Image

Usage:
```
var connectionString = string.Format("Data Source={0}", mbtilefile);
var mbTileProvider = new MBTileProvider(connectionString);
var image = mbTileProvider.GetTile(level, col, row);
```


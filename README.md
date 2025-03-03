# 5E SQL Tools

This is a collection of tools for importing 5e data into SQL Server using LinqPad, making it easy to query. It's a work in progress for my own use, but I'm uploading it here in case anyone else finds it useful.

Sources:

- Monsters.csv from https://github.com/nicolevanderhoeven/xios-guide-to-monsters
- Flee-mortals.csv from https://docs.google.com/spreadsheets/d/1h8u15S3_T3Kwcp1JuxuCy73mILCt0eM3m9f_mgtMq10/edit?gid=296844552#gid=296844552

Process:

1. Run script 0 in Structure to clear/create the database tables.
2. Open the files in Imports using [LinqPad](https://www.linqpad.net/) and connect it to the database where you created the files.
3. Run the remaining scripts in Structure to add other information to the database.
4. Scripts in Queries are examples of how to use the data.

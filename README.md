# EthernetTap
Student project assignment - industry network ethernet tap based on FPGA, coded in C#.

<h3>How to setup and test data exporting</h3>
- Have a MS SQL Server ready - preferably a local one managed by MS SQL Server Management Studio or maybe one created by Visual Studio.
- Your Windows user should have all the permissions by default but keep that in mind.
- Open the application and go to the export page.
- Tick the checkbox for database export and provide server address and database name. By default it's the local server address and "EthernetTap" database.
- Click the button to check the connection. You should be prompted to create the database if it doesn't exist.
- Click the button to confirm the address and the name.
- Go and configure filters and start to capture frames.
- Look for new entities in the three created tables in the database. For local server this can be checked in the Management Studio or in Visual Studio.
- Report all bugs to the SCRUM MATSRE
- ???
- Profit.
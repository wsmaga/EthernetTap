# EthernetTap
Student project assignment - industry network ethernet tap based on FPGA, coded in C#.

<h2>How to setup and test data exporting</h2>
<ol>
  <li>Have a MS SQL Server ready - preferably a local one managed by MS SQL Server Management Studio or maybe one created by Visual Studio.</li>
  <li>Your Windows user should have all the permissions by default but keep that in mind.</li>
  <li>Open the application and go to the export page.</li>
  <li>Tick the checkbox for database export and provide server address and database name. By default it's the local server address and "EthernetTap" database.</li>
  <li>Click the button to check the connection. You should be prompted to create the database if it doesn't exist.</li>
  <li>Click the button to confirm the address and the name.</li>
  <li>Go and configure filters and start to capture frames.</li>
  <li>Look for new entities in the three created tables in the database. For local server this can be checked in the Management Studio or in Visual Studio.</li>
  <li>Report all bugs to the SCRUM MATSRE</li>
  <li>???</li>
  <li>Profit.</li>
</ol>

<h2>An example filter to extract that one incrementing variable from the mockup pcap frames. Exporting was tested on that.</h2>
&lt;Filter&gt;<br>
&#09;&lt;Condition&gt;And([20]=88,[21]=a4)&lt;/Condition&gt;<br>
&#09;&lt;Targets&gt;<br>
&#09;&#09;&lt;Target&gt;<br>
&#09;&#09;&#09;&lt;Id&gt;69&lt;/Id&gt;<br>
&#09;&#09;&#09;&lt;Bytes&gt;50,51,52,53&lt;/Bytes&gt;<br>
&#09;&#09;&#09;&lt;Type&gt;integer&lt;/Type&gt;<br>
&#09;&#09;&#09;&lt;Name&gt;zmienna2&lt;/Name&gt;<br>
&#09;&#09;&#09;&lt;RegisterChanges&gt;true&lt;/RegisterChanges&gt;<br>
&#09;&#09;&#09;&lt;Threshold&gt;<br>
&#09;&#09;&#09;&#09;&lt;Type&gt;gt&lt;/Type&gt;<br>
&#09;&#09;&#09;&#09;&lt;Value&gt;100&lt;/Value&gt;<br>
&#09;&#09;&#09;&#09;&lt;Value2&gt;100&lt;/Value2&gt;<br>
&#09;&#09;&#09;&lt;/Threshold&gt;<br>
&#09;&#09;&lt;/Target&gt;<br>
&#09;&lt;/Targets&gt;<br>
&lt;/Filter&gt;<br>
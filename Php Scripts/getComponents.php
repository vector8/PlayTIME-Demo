<?php

    // o--------------------------------------------------------
    // | Input & Settings
    // o--------------------------------------------------------

    // These variables are sent from Unity, we access them via
    // $_POST and make sure to santitize the input to mysql.
    
    $rfidKey     = mysql_real_escape_string($_POST['rfidKey']);

    // These settings define where the server is located, and
    // which credentials we use to connect to that server.  
    
    $server   = "localhost";
    $username = "admin";
    $password = "admin";
    
    // This is the database and table we are going to access in
    // the mysql server. In this example, we assume that we have
    // the table 'highscores' in the database 'gamedb'.
    
    $database = "playtimedb";
                     
    $select   = "SELECT figurines.name AS figname, components.name AS compname, figcomponents.data1, figcomponents.data2, figcomponents.data3, figcomponents.data4, figcomponents.data5  
	FROM `figurines` 
	INNER JOIN figcomponents ON figcomponents.figid = figurines.ID 
	INNER JOIN components ON components.id = figcomponents.compid 
	WHERE rfidKey = '$rfidKey'";

    // o--------------------------------------------------------
    // | Access database
    // o--------------------------------------------------------

    // Connect to the server with our settings defined above.
    
    $connection = mysql_connect($server, $username, $password) or die(mysql_error());
    
    $result = mysql_select_db($database, $connection) or die(mysql_error()); 
    $result = mysql_query($select, $connection) or die($select."<br/><br/>".mysql_error());

    // Finally, go through top 10 players and return the result
    // back to Unity. The output format for each line will be: 
    // {game}:{player}:{score}
    
    while ($row = mysql_fetch_array($result))
        echo $row['figname'] . "," 
		. $row['compname'] . "," 
		. $row['data1'] . "," 
		. $row['data2'] . "," 
		. $row['data3'] . "," 
		. $row['data4'] . "," 
		. $row['data5'] . ",<br>";
  
    // Close the connection, we're done here.
    
    mysql_close($connection);
?>
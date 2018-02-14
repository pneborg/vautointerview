# vautointerview
.Net Core 2.0 C# Console Application.  
Compile and then navigate to \bin\Release\netcoreapp2.0  
This program utilizes asycnchronous threading to make parallel api calls to http://vautointerview.azurewebsites.net.  
The first batch of threads retrieves each vehicle's detail on a distinct asynchronous worker thread.  
The second batch of threads retrieves each dealer's detail using a distinct list of dealer ids obtained from the list of vehicles returned on a thread per dealer.
# Usage:
      dotnet vautointerview.dll
##### or to show verbose execution and threading detail messages
      dotnet vautointerview.dll true   
##### Output Example 1 without verbose messages  

   Starting 6:55:39 AM
   Congratulations. Answer result Success=True Total Milliseconds=8852

   Completed 6:55:48 AM  
  
   Press Any Key to Continue...     

##### Output Example 2 with verbose messages enabled  
Starting 6:56:35 AM  
DataSet Id ZYileIBz1Qg 6:56:35 AM  
Thread id 12 6:56:35 AM  
Thread id 10 6:56:35 AM  
Thread id 13 6:56:35 AM  
Thread id 8 6:56:35 AM  
Thread id 5 6:56:35 AM  
Thread id 4 6:56:35 AM  
Thread id 9 6:56:35 AM  
Thread id 11 6:56:35 AM  
Thread id 12 6:56:35 AM  
Thread id 10 Complete 6:56:37 AM  
Thread id 11 Complete 6:56:37 AM  
Thread id 4 Complete 6:56:37 AM  
Thread id 9 Complete 6:56:38 AM  
Thread id 8 Complete 6:56:38 AM  
Thread id 5 Complete 6:56:38 AM  
Thread id 4 Complete 6:56:40 AM  
Thread id 11 Complete 6:56:40 AM  
Thread id 12 Complete 6:56:40 AM  

Vehicles 6:56:40 AM  
1440474974 Bentley Mulsanne 2016 1264842036  
859553222 Ford F150 2009 1991116183  
1462180961 MINI Cooper 2004 1809636970  
462998300 Cheverolet Corvette 1979 1264842036  
1758722024 Honda Accord 2016 1809636970  
1563529265 Kia Soul 2016 1991116183  
1511986459 Mitsubishi Gallant 2013 1991116183  
429494110 Nissan Altima 2012 1264842036  
1600385012 Ford F150 2014 1809636970  
Thread id 9  
Thread id 10  
Thread id 8  
Thread id 11 Complete 6:56:42 AM  
Thread id 9 Complete 6:56:42 AM  
Thread id 4 Complete 6:56:44 AM  

Dealers 6:56:44 AM  
1264842036 Doug's Doozies  
1991116183 House of Wheels  
1809636970 Bob's Cars  
Congratulations. Answer result Success=True Total Milliseconds=8833  

Completed 6:56:44 AM  
  
Press Any Key to Continue...  
      

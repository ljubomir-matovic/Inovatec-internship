$taskName = "TempOrderScheduler"
$taskExists = Get-ScheduledTask | Where-Object {$_.TaskName -like $taskName }

if($taskExists) {
   echo "1"; 
} else {
   echo "0";
}
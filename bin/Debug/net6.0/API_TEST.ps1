$run = $true
$ip = "127.0.0.1"
$port = "8080"
$http = [System.Net.HttpListener]::new()
$http.Prefixes.Add("http://" + $ip + ":" + $port + "/")
$http.Start()

while ($run) {
    $context = $http.GetContext()
    
    if($context.Request.UserHostAddress -ne $null) {
        write-host "$($context.Request.UserHostAddress)  =>  $($context.Request.Url)" -f 'mag'
    }

    if ($context.Request.HttpMethod -eq 'POST' -and $context.Request.RawUrl -eq '/') {
    
        $FormContent = [System.IO.StreamReader]::new($context.Request.InputStream).ReadToEnd()
    
        write-host "$($context.Request.UserHostAddress)  =>  $($context.Request.Url)" -f 'mag'
        Write-Host $FormContent -f 'Green'
    
        [string]$html = "[API] POST SUCCESS" 
    
        $buffer = [System.Text.Encoding]::UTF8.GetBytes($html)
        $context.Response.ContentLength64 = $buffer.Length
        $context.Response.OutputStream.Write($buffer, 0, $buffer.Length)
        $context.Response.OutputStream.Close() 
    }

    if ($context.Request.HttpMethod -eq 'GET' -and $context.Request.RawUrl -eq '/taskkill') {
        $run = $false
    }
}
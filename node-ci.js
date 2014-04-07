var http = require('http');
var util = require('util');
var exec = require('child_process').exec;

http.createServer( function(req, res) {

   var child = exec('ls -la', function (error, stdout, stderr) {
        res.writeHead(200, {'Content-Type': 'text/plain' });

        res.write('Hello from the QueueSpy CI server.\n');

        res.write('stdout: ' + stdout + '\n');
        res.write('stderr: ' + stderr + '\n');
        if(error !== null) {
            res.write('exec error: ' + error);
        }
        
        res.end();
    });


}).listen(1337);

// log successful start
console.log('CI server running at http://queuespy.com:1337');

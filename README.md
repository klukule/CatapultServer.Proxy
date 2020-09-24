# CatapultServer.Proxy

CatapultServer.Proxy is MITM proxy which can be used to view and change ProtoBuf messages between Game server and Client

THIS IS NOT CUSTOM SERVER, THIS IS JUST PROGRAM TO SPOOF AND MONITOR TRAFFIC (In hope of making custom server one day)

This proxy uses the reality that unity client is connecting to xxx.fallguys.oncatapult.com which is an alias for xxx-prod.fallguys.oncatapult.com 

fun fact: there is 6 diferent publicly available endpoints for different branches - dev, staging etc...

## Installation

- Use windows hosts file to redirect fallguys server endpoints to local server, change second IP address to your local one which isn't 127.0.0.1 since the proxy needs to proxy two servers (login and gateway) which run on same ports
```bash
127.0.0.1           login.fallguys.oncatapult.com
192.168.1.6          gateway.fallguys.oncatapult.com
```
- After that you will need trusted certificate for proxied domains in pfx format (since communication is done via secured web sockets)
- Change address on line 25 in Program.cs and path to certificate and password on lines 20 and 27.

Now you're good to go

## Usage

Just launch and then start fallguys, you'll start to see bunch of messages being exchanged after clicking "Start"

## Used Modules
[FallGuys.Protocol](https://github.com/klukule/FallGuys.Protocol)
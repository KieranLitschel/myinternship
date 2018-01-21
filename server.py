import SimpleHTTPServer
import SocketServer

port = 8000
Handler = SimppleHTTPServer.SimpleHTTPRequestHandler

httpd = SocketServer.TCPServer(("",port), Handler)

httpd.serve_forever()
#!/usr/bin/python

import SocketServer
import time
import threading
import struct

BROADCASTING_TIMER = 0.05 # seconds
CLIENT_TIMEOUT = 300 # seconds
CHECK_CLIENT_AGE_INTERVAL = 30 # seconds

clients = []
data = ""

def send_data_to_everyone():
    global clients
    global data
    id = 0
    for client in clients:
        if (data[2] == '9'):
            data = str(id) + ',' + str(id) + data[3:]
        else:
            data = str(id) + data[1:]
        client.socket.sendto(data, client.address)
        print "sending", data + " to", client.address
        id += 1

def forget_old_clients():
    global clients
    for client in clients:
        if (time.clock() - client.last_sent_data) > CLIENT_TIMEOUT:
            clients.remove(client)


class Client():
    def __init__(self, socket, address, time):
        self.socket = socket
        self.address = address
        self.last_sent_data = time


class UDPHandler(SocketServer.BaseRequestHandler):

    def setup(self):
        pass

    def handle(self):
        "Called whenever a packet is received"
        global clients
        global data
        data = self.request[0].strip()

        self.client_address = (self.client_address[0], 5000)
        print("%d bytes received from %s:%d" % (
            len(data), self.client_address[0], self.client_address[1]))
        print data

        client_already_registered = False
        for client in clients:
            if self.client_address == client.address:
                client.last_sent_data = time.clock()
                client_already_registered = True
                break

        if not client_already_registered:
            print("Add new client: %s" % self.client_address[0])
            client = Client(self.request[1], self.client_address, time.clock())
            clients.append(client)
            print("Now have %d clients" % len(clients))  

        send_data_to_everyone()      


def main():
    # age_out_clients_timer = threading.Timer(CHECK_CLIENT_AGE_INTERVAL, forget_old_clients)
    # age_out_clients_timer.start()

    #print "thread?"
    #broadcasting_thread = threading.Timer(BROADCASTING_TIMER, send_data_to_everyone)
    #broadcasting_thread.start()
    #print "thread!"

    HOST, PORT = "10.250.235.162", 4000
    print "starting server"

    server = SocketServer.UDPServer((HOST, PORT), UDPHandler)
    server.serve_forever()

if __name__ == "__main__":
    main()

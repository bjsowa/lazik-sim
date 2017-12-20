import os

import socketio
import eventlet
import eventlet.wsgi

from flask import Flask


sio = socketio.Server()
app = Flask(__name__)

@sio.on('connect')
def connect(sid, environ):
    print("connect ", sid)
    send_control(1.0, 0.15)

def send_control(accel, steering):
    sio.emit(
        "steer",
        data={
            'accel': str(accel),
            'steering': str(steering)
        },
        skip_sid=True)
    


if __name__ == '__main__':

    # wrap Flask application with engineio's middleware
    app = socketio.Middleware(sio, app)

    # deploy as an eventlet WSGI server
    eventlet.wsgi.server(eventlet.listen(('', 4567)), app)
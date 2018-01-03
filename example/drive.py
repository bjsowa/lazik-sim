import os
import base64

import socketio
import eventlet
import eventlet.wsgi

import numpy as np

from flask import Flask
from io import BytesIO
from PIL import Image

sio = socketio.Server()
app = Flask(__name__)

@sio.on('connect')
def connect(sid, environ):
    print("connect ", sid)
    send_control(1.0, 0.15)

@sio.on('telemetry')
def telemetry(sid, data):
    if data:
        speed = data["speed"]
        angularSpeed = data["angularSpeed"]
        imgStr = data["image"]
        image = Image.open(BytesIO(base64.b64decode(imgStr)))
        image_array = np.asarray(image)
        print( speed, angularSpeed, image_array.shape )

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
import os
import base64

import socketio
import eventlet
import eventlet.wsgi

import numpy as np
import matplotlib.pyplot as plt

from random import uniform

from flask import Flask
from io import BytesIO
from PIL import Image

sio = socketio.Server()
app = Flask(__name__)

@sio.on('connect')
def connect(sid, environ):
    print("connect ", sid)

@sio.on('disconnect')
def disconnect(sid):
    print('disconnect ', sid)

@sio.on('telemetry')
def telemetry(sid, data):
    if data:
        speed = data["speed"]
        angularSpeed = data["angularSpeed"]
        imgStr = data["image"]
        image = Image.open(BytesIO(base64.b64decode(imgStr)))
        image_array = np.asarray(image)

        print( speed, angularSpeed )
        plt.imshow(image_array)
        plt.show()

        send_control(uniform(-1,1), uniform(-1,1))
        sio.emit("request_telemetry", data = {})


def send_control(accel, steering):
    sio.emit(
        "steer",
        data={
            'accel': str(accel),
            'steering': str(steering)
        })
    

if __name__ == '__main__':

    # wrap Flask application with engineio's middleware
    app = socketio.Middleware(sio, app)

    # deploy as an eventlet WSGI server
    eventlet.wsgi.server(eventlet.listen(('', 4567)), app)
#!/usr/bin/env python3

import flask
from flask import Flask

from flask_apscheduler import APScheduler

app = Flask(__name__)
scheduler = APScheduler()

@app.route("/")
def hello_world():
    return "Hello, World!"

@scheduler.task('interval', id='test_job', seconds=10)
def hello_job():
    print(hello_world())

if __name__ == '__main__':
    scheduler.init_app(app)
    scheduler.start()

    app.run()
